@echo off
rem Builds bin\FORT1.EXE and bin\FORT2.EXE from modern\ with Free Pascal (fpc.exe must be on PATH).
setlocal
cd /d "%~dp0"
if not exist bin mkdir bin
for %%N in (1 2) do (
  if not exist obj\f%%N mkdir obj\f%%N
  fpc -Mtp -Ci- -Cr- -Co- -CR- -Sew- -FEbin -FUobj\f%%N -B modern\FORT%%N.PAS || exit /b 1
)
