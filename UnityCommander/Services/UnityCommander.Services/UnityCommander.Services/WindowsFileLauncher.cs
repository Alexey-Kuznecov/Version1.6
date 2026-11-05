
using System.Diagnostics;
using UnityCommander.Abstractions;

namespace UnityCommander.Services
{
    public sealed class WindowsFileLauncher : IFileLauncher
    {
        public void Open(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        public void OpenWith(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = $"shell32.dll,OpenAs_RunDLL \"{path}\"",
                UseShellExecute = true
            });
        }

        public void OpenLocation(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{path}\"",
                UseShellExecute = true
            });
        }
    }
}
