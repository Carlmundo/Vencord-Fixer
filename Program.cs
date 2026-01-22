using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Vencord_Fixer
{
    internal class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        const uint MB_ICONEXCLAMATION = 0x30;
        const uint MB_YESNO = 0x04;
        const int IDYES = 6;
        static void Main(string[] args)
        {
            string workingDir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(workingDir);
            string vencordExe = Path.Combine(workingDir, "VencordInstallerCli.exe");

            // Check if VencordInstallerCli exists
            if (!File.Exists(vencordExe)) {
                var result = MessageBox(
                    IntPtr.Zero,
                    "VencordInstallerCli.exe is required but not found.\n\nWould you like to download it now?",
                    "Missing Dependency",
                    MB_YESNO
                );
                if (result != IDYES) {
                    MessageBox(
                        IntPtr.Zero,
                        "Cannot proceed without VencordInstallerCli. Exiting.",
                        "Error",
                        MB_ICONEXCLAMATION
                    );
                    return;
                }

                try {
                    using (var client = new WebClient()) {
                        client.DownloadFile(
                            "https://github.com/Vencord/Installer/releases/latest/download/VencordInstallerCli.exe",
                            vencordExe
                        );
                    }
                }
                catch {
                    MessageBox(
                        IntPtr.Zero,
                        "Failed to download VencordInstallerCli.exe.\nPlease check your connection and try again.",
                        "Error",
                        MB_ICONEXCLAMATION
                    );
                    return;
                }

                if (!File.Exists(vencordExe)) {
                    MessageBox(
                        IntPtr.Zero,
                        "Failed to download VencordInstallerCli.exe.",
                        "Error",
                        MB_ICONEXCLAMATION
                    );
                    return;
                }
            }
            
            // Locate Discord
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string discordPath = Path.Combine(localAppData, "Discord", "Update.exe");
            // Kill Discord
            RunHidden("taskkill", "/f /im Discord.exe");
            // Update Vencord CLI Installer
            RunHidden(vencordExe, "-update-self", true);
            // Install Vencord
            RunHidden(vencordExe, $"-location \"{Path.Combine(localAppData, "Discord")}\" -install", true);
            // Start Discord
            if (File.Exists(discordPath)) {
                Process.Start(new ProcessStartInfo {
                    FileName = discordPath,
                    Arguments = "--processStart Discord.exe",
                    UseShellExecute = false
                });
            }
        }
        static void RunHidden(string exePath, string arguments = null, Boolean cmd = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //VencordInstallerCLI waits for user input without CMD
            if (cmd) {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/c \"\"{exePath}\" {arguments}\"";
            }
            else {
                startInfo.FileName = exePath;
                startInfo.Arguments = arguments;
            }
            startInfo.CreateNoWindow = true; //don't create a window
            startInfo.RedirectStandardError = true; //redirect standard error
            startInfo.RedirectStandardOutput = true; //redirect standard output
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.ErrorDialog = false;

            using (Process p = new Process { StartInfo = startInfo, EnableRaisingEvents = true }) {
                p.ErrorDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data)) {
                        Debug.WriteLine("Error: " + e.Data);
                    }
                };
                p.OutputDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data)) {
                        Debug.WriteLine("Output: " + e.Data);
                    }
                };

                p.Start();
                p.BeginErrorReadLine(); //begin async reading for standard error
                p.BeginOutputReadLine(); //begin async reading for standard output
                p.WaitForExit();
            }
        }
    }
}
