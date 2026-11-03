"""Isolated HTTP CRUD/restart checks. Never touches the developer's journal."""
from contextlib import contextmanager
from datetime import date, timedelta
import html
import http.cookiejar
import os
from pathlib import Path
import re
import socket
import subprocess
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request

ROOT = Path(__file__).resolve().parents[1]
with socket.socket() as probe:
    probe.bind(("127.0.0.1", 0))
    PORT = probe.getsockname()[1]
BASE = f"http://127.0.0.1:{PORT}"
client = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))

def request(path, data=None):
    encoded = None if data is None else urllib.parse.urlencode(data).encode()
    try:
        response = client.open(BASE + path, data=encoded, timeout=5)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        return response.status, response.read().decode()

def post(path, data, form):
    status, page = request(form)
    assert status == 200
    token = re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"', page)
    assert token
    return request(path, {**data, "__RequestVerificationToken": html.unescape(token[1])})

@contextmanager
def running(database):
    env = {**os.environ, "ASPNETCORE_ENVIRONMENT": "Development", "Database__Path": str(database)}
    with tempfile.TemporaryFile() as output:
        process = subprocess.Popen(["dotnet", str(ROOT / "bin/Release/net10.0/TrainingLog.dll"),
                                    "--urls", BASE], cwd=ROOT, env=env, stdout=output, stderr=output)
        try:
            for _ in range(100):
                if process.poll() is not None:
                    output.seek(0)
                    raise RuntimeError(output.read().decode())
                try:
                    if request("/")[0] == 200:
                        break
                except (OSError, urllib.error.URLError):
                    pass
                time.sleep(.1)
            else:
                raise RuntimeError("App did not start")
            yield
        finally:
            process.terminate()
            process.wait(timeout=15)

with tempfile.TemporaryDirectory() as temporary:
    database = Path(temporary) / "journal.db"
    fields = {"Exercise":"Restart bench", "Date":date.today().isoformat(), "Sets":"3", "Reps":"5", "WeightKg":"82.55"}
    with running(database):
        status, page = post("/Workouts/Create", fields, "/Workouts/Create")
        assert status == 200 and "Restart bench" in page
    with running(database):
        assert "Restart bench" in request("/")[1]
        fields.update(Id="1", Exercise="Updated bench", WeightKg="83.25")
        status, page = post("/Workouts/Edit/1", fields, "/Workouts/Edit/1")
        assert status == 200 and "Updated bench" in page and "83.25 kg" in page
        status, page = post("/Workouts/Edit/1", {**fields, "Sets":"0"}, "/Workouts/Edit/1")
        assert status == 200 and "Sets must be" in page
        assert "83.25 kg" in request("/")[1]
        assert request("/Workouts/Edit/1", fields)[0] == 400
        assert request("/Workouts/Delete/1", {})[0] == 400
        assert request("/Workouts/Edit/999")[0] == 404
        assert request("/Workouts/Delete/999")[0] == 404
        assert "Delete this entry?" in request("/Workouts/Delete/1")[1]
        assert "Updated bench" in request("/")[1], "GET delete must not mutate"
    with running(database):
        assert "Updated bench" in request("/")[1]
        assert post("/Workouts/Delete/1", {}, "/Workouts/Delete/1")[0] == 200
    with running(database):
        assert "Updated bench" not in request("/")[1]
        assert request("/Workouts/Edit/1")[0] == 404
    print("PASS: create/edit/delete survive actual process restarts; invalid edit, missing IDs and CSRF checks")
