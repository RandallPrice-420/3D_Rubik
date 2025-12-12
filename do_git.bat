@echo off

echo ----------------------------------------------------------------------------
echo IMPORTANT:
echo ----------------------------------------------------------------------------
echo   1.  Close Unity and Visual Studio BEFORE running this batch file.
echo.
echo   2.  Do NOT add a .gitignore or a README.md file in github:
echo       a.  Add the .gitignore file to you Unity project.
echo       b.  The README.md file is created from this batch file.
echo ----------------------------------------------------------------------------
echo.
set GIT_TRACE_PACKET=1
set GIT_TRACE=1
set GIT_CURL_VERBOSE=1

rem ----------------------------------------------------------------------------
rem  Configure some git settings.
rem ----------------------------------------------------------------------------
set project_name=3D_Rubik
set editor_version=2019.4.40f1
set local_directory=C:/Repos/Unity/%editor_version%/%project_name%
set remote_origin=https://github.com/RandallPrice-420/%project_name%

rem ----------------------------------------------------------------------------
rem  Display the project information.
rem ----------------------------------------------------------------------------
echo project_name.....:  %project_name%
echo editor_version...:  %editor_version%
echo local_directory..:  %local_directory%
echo remote_origin....:  %remote_origin%

rem ----------------------------------------------------------------------------
rem  Check if the remote repository is already created in GitHub.
rem ----------------------------------------------------------------------------
:Check_Repository
echo.
set /p check="Is the %project_name% repository in GitHub? (Enter=Y, Y, N)  "
if not defined check ( set "check=y" )
if /I "%check%"=="y" goto Repository_Ready
if /I "%check%"=="n" goto Prompt_Create_Repository
echo You must enter Y or N.
goto Check_Repository

rem ----------------------------------------------------------------------------
rem  Prompt to create the remote repository in GitHub.
rem ----------------------------------------------------------------------------
:Prompt_Create_Repository
echo.
set /p open="Create the %project_name% repository in GitHub? (Enter=Y, Y, N)  "
if not defined open ( set "open=y" )
if /I "%open%"=="y" goto Create_Repository
if /I "%open%"=="n" goto No_Repository
echo.
echo You must enter Y or N.
goto Prompt_Create_Repository

rem ----------------------------------------------------------------------------
rem  Create the remote repository in GitHub.
rem   https://github.com/RandallPrice-420?tab=repositories
rem ----------------------------------------------------------------------------
:Create_Repository
echo.
echo Creating %project_name% repository in GitHub...
rem gh auth login
gh repo create %remote_origin% --public
start msedge %remote_origin%

rem ----------------------------------------------------------------------------
rem  Repository in GitHub, prompt for the step to perform.
rem ----------------------------------------------------------------------------
:Repository_Ready
echo.
set /p step= "Enter the step to perform (F = First time, A = ADD changes and commit, Q = Quit):  "
rem echo You entered:  %step%
if /I "%step%"=="f" goto First_Time
if /I "%step%"=="a" goto Add_And_Commit
if /I "%step%"=="q" goto Done
echo You must enter F, A or Q.
pause
goto Repository_Ready

:First_Time
rem ----------------------------------------------------------------------------
rem One-time configuration for this project.
rem
rem  - Configure some github global settings
rem  - Delete the README.md file if it exists
rem  - Create the README.md file
rem  - Initialize the repository
rem  - Add and commit the README.md file
rem  - Refresh the local files from the master branch
rem  - Set the remote origin  
rem  - Push to the remote repository
rem  - Show the status
rem ----------------------------------------------------------------------------
echo.
git config --global --add safe.directory %local_directory%
git config --global user.email "randall_price@hotmail.com"
git config --global user.name  "Randall Price"

rem ----------------------------------------------------------------------------
rem  These settings are to resolve the following error:
rem    error: RPC failed; HTTP 408 curl 22 The requested URL returned error: 408
rem    send-pack: unexpected disconnect while reading sideband packet
rem    fatal: the remote end hung up unexpectedly
rem 157286400
rem ----------------------------------------------------------------------------
git config --global http.postBuffer 524288000
git config --global core.compression 0
echo.

set filePath=README.md
if exist %filePath% (
     del %filePath%
    echo %filePath% file deleted.
)
echo %project_name%>> %filePath%
echo.>> %filePath%
echo %project_name% game using Unity %editor_version%.>> %filePath%
echo.>> %filePath%

git init
git add %filePath%
git commit -m "Initial project upload."
git branch -M master
git remote add origin %remote_origin%
git push -u origin master
git status

echo.
echo First time configuration:
echo   - %filePath% created and commited.
echo.


:Add_And_Commit
rem ----------------------------------------------------------------------------
rem  - Prompt for the commit message
rem      Example:  Added Part 1 - Spaceship Controls and Part 2 - Bullets.
rem  - Add and commit the changes
rem  - Push to the remote repository
rem ----------------------------------------------------------------------------
set "defaultValue=Initial project upload."
set /p "commit_message=Enter commit message (Enter = <%defaultValue%>, Q = Quit):  "
if /I "%commit_message%"=="q" goto Done
if not defined commit_message ( set "commit_message=%defaultValue%" )
echo You entered: %commit_message%

git pull origin master
git add .
git commit -m "%commit_message%"
git push -u origin master

echo.
echo - Changed files successfully committed and pushed to remote repository .
echo.
pause
exit


rem ----------------------------------------------------------------------------
rem  Repository in GitHub.
rem ----------------------------------------------------------------------------
:No_Repository
echo.
echo - Repository is NOT in GitHub and will NOT be created.
echo.
pause
exit

rem ----------------------------------------------------------------------------
rem  Finished!
rem ----------------------------------------------------------------------------
:Done
echo.
echo Done
pause
exit
