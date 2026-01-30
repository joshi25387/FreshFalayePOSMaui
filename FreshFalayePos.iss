[Setup]
AppName=FreshFalaye POS
AppVersion=1.0.0
DefaultDirName={pf}\FreshFalayePOS
DefaultGroupName=FreshFalaye POS
OutputBaseFilename=FreshFalayePOS-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\FreshFalaye POS"; Filename: "{app}\FreshFalaye.Pos.Shared.exe"
Name: "{commondesktop}\FreshFalaye POS"; Filename: "{app}\FreshFalaye.Pos.Shared.exe"

Name: "{commondesktop}\FreshFalaye POS (Kiosk)"; Filename: "C:\Program Files\Google\Chrome\Application\chrome.exe" ; Parameters: "--kiosk --kiosk-printing --disable-print-preview --disable-print-header-footer http://localhost:5050"; WorkingDir: "{app}"

[Run]
Filename: "{app}\FreshFalaye.Pos.Shared.exe"; Description: "Start POS"; Flags: nowait postinstall skipifsilent