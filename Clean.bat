@ECHO off
cls

ECHO Deleting all BIN and OBJ & .vs folders...
ECHO.

rd /s/q .vs

FOR /d /r . %%d in (bin,obj) DO (
	IF EXIST "%%d" (		 	 
		ECHO %%d | FIND /I "\node_modules\" > Nul && ( 
			ECHO.Skipping: %%d
		) || (
			ECHO.Deleting: %%d
			rd /s/q "%%d"
		)
	)
)

ECHO.
ECHO. Press any key to exit.
pause > nul