@echo off
chcp 65001
setlocal enabledelayedexpansion

(
echo Option Explicit
echo Dim xl, wb, ws, fso, f, fileName, className, i
echo Set xl = CreateObject^("Excel.Application"^)
echo xl.DisplayAlerts = False
echo xl.Visible = False
echo Set wb = xl.Workbooks.Open^("%CD%\__tables__.xlsx"^)
echo Set ws = wb.Worksheets^(1^)
echo Set fso = CreateObject^("Scripting.FileSystemObject"^)

echo ' Clear all data from row 4 to the end
echo ws.Range^("A4:E" ^& ws.UsedRange.Rows.Count^).ClearContents
echo ws.Range^("A4:E" ^& ws.UsedRange.Rows.Count^).Delete

echo Dim row: row = 4
echo For Each f In fso.GetFolder^("%CD%"^).Files
echo     If LCase^(Right^(f.Name, 5^)^) = ".xlsx" Then
echo         fileName = Left^(f.Name, Len^(f.Name^) - 5^)
echo         If Left^(fileName, 2^) ^<^> "~$" And Left^(fileName, 1^) ^<^> "_" And fileName ^<^> "__tables__" Then
echo             className = UCase^(Left^(fileName, 1^)^) ^& Mid^(fileName, 2^)
echo             ws.Cells^(row, 1^).Value = ""
echo             ws.Cells^(row, 2^).Value = fileName ^& ".Tb" ^& className
echo             ws.Cells^(row, 3^).Value = className
echo             ws.Cells^(row, 4^).Value = "TRUE"
echo             ws.Cells^(row, 5^).Value = fileName ^& ".xlsx"
echo             row = row + 1
echo         End If
echo     End If
echo Next
echo wb.Save
echo wb.Close
echo xl.Quit
) > update_tables.vbs

cscript //nologo update_tables.vbs
del update_tables.vbs

echo Configuration updated to __tables__.xlsx

rem Set environment variables and execute Luban
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Luban\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %WORKSPACE%\Luban\MiniTemplate\luban.conf ^
    -x outputCodeDir=../Assets/Generate/Csharp ^
    -x outputDataDir=../Assets/Bundles/Configs

pause