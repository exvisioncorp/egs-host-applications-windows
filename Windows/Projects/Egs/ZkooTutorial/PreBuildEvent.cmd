REM DO NOT add double quotation to the folder paths!!
SET ResourceSourceFolderPath=%1..\..\..\..\Assets\Egs\ZkooTutorial\Resources
SET ResourceDestinationFolderPath=%1\Resources
SET CultureInfoName=



rmdir /S /Q "%ResourceDestinationFolderPath%\drawable-mdpi\"
rmdir /S /Q "%ResourceDestinationFolderPath%\raw\"
echo xcopy /E /I /Q /Y "%ResourceSourceFolderPath%\drawable-mdpi%CultureInfoName%" "%ResourceDestinationFolderPath%\drawable-mdpi\"
echo xcopy /E /I /Q /Y "%ResourceSourceFolderPath%\raw%CultureInfoName%" "%ResourceDestinationFolderPath%\raw\"
xcopy /E /I /Q /Y "%ResourceSourceFolderPath%\drawable-mdpi%CultureInfoName%" "%ResourceDestinationFolderPath%\drawable-mdpi\"
xcopy /E /I /Q /Y "%ResourceSourceFolderPath%\raw%CultureInfoName%" "%ResourceDestinationFolderPath%\raw\"



:End
pause
