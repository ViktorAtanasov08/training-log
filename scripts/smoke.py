"""Exercise a fresh local TrainingLog instance using Python's standard library."""

from datetime import date, timedelta
from html.parser import HTMLParser
from http.cookiejar import CookieJar
import time
from urllib.error import HTTPError, URLError
from urllib.parse import urlencode, urlparse
from urllib.request import HTTPCookieProcessor, Request, build_opener


BASE_URL = "http://127.0.0.1:5084"
client = build_opener(HTTPCookieProcessor(CookieJar()))


def get(path):
    with client.open(BASE_URL + path, timeout=5) as response:
        return response.read().decode("utf-8")


class FormToken(HTMLParser):
    token = None

    def handle_starttag(self, tag, attrs):
        values = dict(attrs)
        if tag == "input" and values.get("name") == "__RequestVerificationToken":
            self.token = values.get("value")


def post(values, csrf=True):
    payload = dict(values)
    if csrf:
        parser = FormToken()
        parser.feed(get("/Workouts/Create"))
        assert parser.token, "The exercise form must include a CSRF token"
        payload["__RequestVerificationToken"] = parser.token
    request = Request(BASE_URL + "/Workouts/Create", data=urlencode(payload).encode())
    with client.open(request, timeout=5) as response:
        return response.read().decode("utf-8"), response.geturl()


def main():
    for attempt in range(60):
        try:
            page = get("/")
            break
        except (URLError, TimeoutError):
            if attempt == 59:
                raise
            time.sleep(0.5)
    assert "Your first entry starts here." in page, "Use a fresh app instance"
    assert "Log an exercise" in get("/Workouts/Create")
    assert "entry-form" in get("/css/training.css")

    valid = {
        "Exercise": "  Smoke bench press  ",
        "Date": date.today().isoformat(),
        "Sets": "3",
        "Reps": "5",
        "WeightKg": "82.5",
    }
    page, url = post(valid)
    assert urlparse(url).path in ("/", "/Workouts", "/Workouts/Index")
    assert "<strong>Smoke bench press</strong>" in page
    assert "82.5 kg" in page

    cases = [
        ({"Exercise": "   "}, "Enter an exercise name."),
        ({"Sets": "0"}, "Sets must be between 1 and 20."),
        ({"Reps": "101"}, "Reps must be between 1 and 100."),
        ({"WeightKg": "-1"}, "Weight must be between 0 and 1000 kg."),
        ({"Date": (date.today() + timedelta(days=1)).isoformat()}, "Choose today or an earlier date."),
    ]
    for changes, error in cases:
        page, url = post({**valid, "Exercise": "Rejected smoke entry", **changes})
        assert url.endswith("/Workouts/Create"), "Invalid entries must stay on the form"
        assert error in page, error
        assert "Rejected smoke entry" not in get("/"), "Invalid data must not be saved"
    assert get("/").count("<strong>Smoke bench press</strong>") == 1

    page, _ = post({**valid, "Exercise": "Smoke bodyweight squat", "WeightKg": "0"})
    assert "Smoke bodyweight squat" in page and "0 kg" in page

    page, _ = post({**valid, "Exercise": "<script>alert(1)</script>"})
    assert "&lt;script&gt;alert(1)&lt;/script&gt;" in page
    assert "<script>alert(1)</script>" not in page

    try:
        post(valid, csrf=False)
    except HTTPError as error:
        assert error.code == 400, "A missing CSRF token must return HTTP 400"
    else:
        raise AssertionError("An unprotected POST was accepted")

    print("PASS: pages, CSS, valid entries, decimal and zero weights, five invalid cases, HTML escaping and CSRF")


if __name__ == "__main__":
    main()
