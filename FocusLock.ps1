$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
try {
    Add-Type -Path (Join-Path $PSScriptRoot 'FocusLock.cs') -ReferencedAssemblies System.Windows.Forms,System.Drawing
    [FocusLock.Program]::Run()
} catch {
    [System.Windows.Forms.MessageBox]::Show($_.Exception.Message, 'Focus Lock could not start', 'OK', 'Error') | Out-Null
}
