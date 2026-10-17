# ��� 1: ��ࢠ� ����� �����

���� ���� � �� ࠧ���� ��� ������� �⨣� �� C# � ��� ������� � ������� ���⭮ �� ��࠭�. SQL ��ꧪ�� � ᫥����� ��ꯪ�.

## ����࠭�

1. �⢮� TrainingLog.sln �� Visual Studio.
2. ����� ��䨫� http � ����ࠩ � Ctrl+F5.
3. ���᭨ Log an exercise � ����� Bench press, 3 �ਨ, 5 ����७��, 100 kg.
4. ���⠩ ����� ��� ��� � ����� � 0 �ਨ. �ਫ������� ��� �� �� ����૨.

## ��� � ������ ������

1. Create.cshtml ������� �����.
2. ������� ����� POST ��� �� Workouts/Create.
3. MVC �ॢ��� ������ � ����� WorkoutEntry.
4. ModelState.IsValid �஢�� ���� ������ �⣮����� �� �ࠢ����.
5. WorkoutLog.Add ������ �������� ����� � ᯨ�ꪠ.
6. RedirectToAction ���� ����� �� ��ୠ��, ���� ���� � ������� ᯨ�ꪠ.

## ����� �� ����� �� ���᭨�

- ����� �।�⠢�� Exercise, Sets, Reps � WeightKg ��� ᢮��⢠ �� ����?
- ��� WeightKg � decimal?
- ��� �஢���� ������ � � C#, ��� HTML ������ �� ���� min � max?
- ��� ������ � ���� �� ����� � ⠧� �����?
- ��� GET � POST ��⮤�� Create ���� ࠧ��筨 ஫�?

## ����� ��ࢠ �஬ﭠ

���� ��� ������ �஥��, ������ �� ���� ॠ��� �����: Squat, 3 �ਨ �� 5 ����७��. ��᫥ ����� � ���� �����, ���� ��।��� ।� �� ������. ���᭨ ����� �ࠢ� OrderByDescending, �।� �� �஬���� �஥��.

## Git � GitHub

Commit � ������� ᭨��� �� �����襭� �஬ﭠ. Push ��碠 ⥧� commit-� � GitHub. ��碠�� ࠡ��� ���⥯����.

��� ��� ��� ������� Git repository, ࠧ������� ������ �:

    git log --oneline
    git status

�� ��ࢮ �㡫��㢠�� �ꧤ�� �ࠧ�� �㡫�筮 repository � GitHub � ��� training-log. �� ������ ��� README �� �ꧤ������. ��᫥ � ������ �� �஥��:

��� ������ �� ࠧ��娢�࠭� ����� ��� Git �����, ��ࢮ ���ꫭ�:

    git init -b main
    git add .
    git commit -m "Add initial workout journal with form validation"

���� ⮢� ��ন �஥�� � �ࠧ��� GitHub repository:

    git remote add origin https://github.com/ViktorAtanasov08/training-log.git
    git push -u origin main

��� Git ���᪠ �室, ��������� ��ଠ���� ����७ �室 �� Git Credential Manager. �� ���⠢� ��஫� ��� ⮪�� � ����.

�� ᫥���� �����襭� �஬ﭠ:

    git add .
    git commit -m "Describe the completed change"
    git push

��ࢮ ���� ����� ������� git diff. ��� commit �ꮡ饭��, ���� ���ᢠ� ����� ����⢨⥫�� � ��⮢�.
