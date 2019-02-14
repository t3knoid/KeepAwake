# define name of installer
!include "MUI2.nsh"

# Installer properties
!define NAME "Keep Awake"
!define COMPANYNAME "Frank Refol"
!define DESCRIPTION "KeepAwake is a simple C# application that keeps windows session from sleeping."
!define HELPURL "https://github.com/t3knoid/KeepAwake/issues"
OutFile "KeepAwake_setup.exe"
Caption "$(^Name)"
 
# define installation directory
InstallDir "$PROGRAMFILES64\Keep Awake"
 
# For removing Start Menu shortcut in Windows 7
RequestExecutionLevel admin
 
;--------------------------------
;Interface Configuration
!define MUI_PAGE_HEADER_TEXT "$(^Name) Setup"
!define MUI_DIRECTORYPAGE_TEXT_TOP "Select a Destination Folder."
!define MUI_INSTFILESPAGE_FINISHHEADER_TEXT "$(^Name) Installed"
!define MUI_FINISHPAGE_RUN "$INSTDIR\KeepAwake.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch $(^Name)"
!define MUI_FINISHPAGE_TITLE "Setup Complete"
!define MUI_FINISHPAGE_TEXT "$(^Name) is now installed. Click Close to complete setup."
!define MUI_FINISHPAGE_BUTTON "Close"
!define MUI_UNCONFIRMPAGE_TEXT_TOP "Uninstalling $(^Name)"
!define MUI_UNCONFIRMPAGE_TEXT_LOCATION "Uninstalling $(^Name)"

!insertmacro MUI_LANGUAGE "English"


# Pages
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Create the shared function.
!macro MYMACRO un
  Function ${un}killapp
	StrCpy $0 "KeepAwake.exe"
	DetailPrint "Searching for processes called '$0'"
	KillProc::FindProcesses
	StrCmp $1 "-1" wooops
	DetailPrint "-> Found $0 processes"
 
   StrCmp $0 "0" completed
   Sleep 1500
 
   StrCpy $0 "KeepAwake.exe"
   DetailPrint "Killing all processes called '$0'"
   KillProc::KillProcesses
   StrCmp $1 "-1" wooops
   DetailPrint "-> Killed $0 processes, failed to kill $1 processes"
   Sleep 1500
 
   Goto completed
 
   wooops:
   DetailPrint "-> Error: Something went wrong :-("
   Abort

   completed:
   DetailPrint "Everything went okay :-D"
  FunctionEnd
!macroend

Function .onInit
	DetailPrint "Checking if ${NAME} is installed."
	ReadRegStr $R0 HKLM "SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "UninstallString"
	DetailPrint "Uninstall string read is $R0"
	StrCmp $R0 "" NotInstalled
	MessageBox MB_YESNO|MB_TOPMOST "$(^Name) is already installed. Uninstall?" IDYES Yes IDNO No
	No:
		DetailPrint "$(^Name) is installed. Quitting install."
		Quit
	Yes:
		DetailPrint "Uninstalling ${NAME}."
		ExecWait $R0
 	NotInstalled:
	DetailPrint "$(^Name) not installed. Continuing with installation."
	# start install
FunctionEnd

; Insert function as an installer and uninstaller function.
!insertmacro MYMACRO ""
!insertmacro MYMACRO "un."

# start default section
Section "Installation"

	# start install
	call killapp
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR
 
    # create the uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"
	
	# files to copy
	File /r "..\KeepAwake\bin\Release\"
	
	# Add add/remove entry
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"DisplayName" "Keep Awake"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"DisplayIcon" "$\"$INSTDIR\KeepAwake.exe$\""		
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"Publisher" \"${COMPANYNAME}$\""		
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"HelpLink" "$\"${HELPURL}$\""		
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
		"DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}$\""
 
SectionEnd

Section "Start Menu Shortcuts"

	CreateDirectory "$SMPROGRAMS\Keep Awake"
	CreateShortCut "$SMPROGRAMS\Keep Awake\Keep Awake.lnk" "$INSTDIR\KeepAwake.exe"
    CreateShortCut "$SMPROGRAMS\Keep Awake\Uninstall.lnk" "$INSTDIR\uninstall.exe"
SectionEnd

 
# uninstaller section start
Section "uninstall"

	call un.killapp
    # first, delete the uninstaller
    Delete "$INSTDIR\uninstall.exe"

    # second, remove the link from the start menu
	Delete "$SMPROGRAMS\\Uninstall.lnk"
    Delete "$SMPROGRAMS\Keep Awake\Keep Awake.lnk"
	RMDir "$SMPROGRAMS\Keep Awake"
	
	# third, delete all files
	Delete "$APPDATA\\Keep Awake\\config.yml"
	
	# fourth, delete installation folder
	RMDir /r  "$INSTDIR"
	
	# Remove add/remove registry entry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)"
 
# uninstaller section end
SectionEnd