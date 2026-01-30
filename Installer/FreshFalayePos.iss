[Setup]
AppId={{D8B43F88-FA0D-4A41-9B18-FRESHPOS001}}
AppName=FreshFalaye POS
AppVersion=1.0.0
DefaultDirName={autopf}\FreshFalayePOS
DefaultGroupName=FreshFalaye POS
OutputBaseFilename=FreshFalayePOS-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\FreshFalaye POS"; Filename: "{app}\FreshFalaye.Pos.Maui.exe"
Name: "{commondesktop}\FreshFalaye POS"; Filename: "{app}\FreshFalaye.Pos.Maui.exe"

[Run]
Filename: "{app}\FreshFalaye.Pos.Maui.exe"; Description: "Start POS"; Flags: nowait postinstall skipifsilent