using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using System;

[InitializeOnLoad]
public class AutoUpdateSetup
{
    private const string GitWindowsUrl = "https://github.com/git-for-windows/git/releases/download/v2.44.0.windows.1/Git-2.44.0-64-bit.exe";
    private const string GitMacUrl = "https://sourceforge.net/projects/git-osx-installer/files/latest/download";
    private const string PrefKey = "AutoUpdate_Setup";
    private const string InstallerPath = "Temp/git_installer";

    static AutoUpdateSetup()
    {
        if (EditorPrefs.HasKey(PrefKey))
        {
            if (EditorPrefs.GetBool(PrefKey))
                EditorApplication.delayCall += CheckForUpdates;

            return;
        }

        EditorApplication.delayCall += PromptUser;
    }

    private static void PromptUser()
    {
        bool wantsAutoUpdate = EditorUtility.DisplayDialog(
            "Automatic Updates",
            "Would you like to enable update checking from GitHub?\n\nThis will install Git if it is not already installed.",
            "Yes, enable",
            "No thanks"
        );

        EditorPrefs.SetBool(PrefKey, wantsAutoUpdate);

        if (!wantsAutoUpdate)
            return;

        if (IsGitInstalled())
        {
            CheckForUpdates();
        }
        else
        {
            _ = DownloadAndInstallGitAsync();
        }
    }

    private static void CheckForUpdates()
    {
        if (!IsGitInstalled())
            return;

        try
        {
            string rootDir = Application.dataPath + "/../";
            RunGitCommand("fetch", rootDir);
            string status = RunGitCommand("rev-list HEAD..@{u} --count", rootDir).Trim();

            if (int.TryParse(status, out int count) && count > 0)
            {
                string commitMessages = RunGitCommand("log HEAD..@{u} --oneline -n 5", rootDir);

                bool update = EditorUtility.DisplayDialog(
                    "Update Available",
                    $"There are {count} new update(s) available.\n\nRecent changes:\n{commitMessages}\n\nWould you like to pull these updates now?",
                    "Update Now",
                    "Later"
                );

                if (update)
                {
                    TryRunGitPull();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AutoUpdate] Could not check for updates: {ex.Message}");
        }
    }

    private static string RunGitCommand(string args, string workingDir)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }

    private static bool IsGitInstalled()
    {
        try
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task DownloadAndInstallGitAsync()
    {
        string url = Application.platform == RuntimePlatform.WindowsEditor ? GitWindowsUrl : GitMacUrl;
        string extension = Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : ".pkg";
        string installerPath = InstallerPath + extension;

        EditorUtility.DisplayProgressBar("Auto Update Setup", "Downloading Git...", 0f);
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerFile(installerPath);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            EditorUtility.DisplayProgressBar("Auto Update Setup", "Downloading Git...", operation.progress);
            await Task.Delay(100);
        }

        EditorUtility.ClearProgressBar();
        if (request.result == UnityWebRequest.Result.Success)
            await InstallGitAsync(installerPath);
    }

    private static async Task InstallGitAsync(string installerPath)
    {
        try
        {
            ProcessStartInfo startInfo = Application.platform == RuntimePlatform.WindowsEditor ? new ProcessStartInfo { FileName = installerPath, Arguments = "/VERYSILENT /NORESTART", UseShellExecute = true } : new ProcessStartInfo { FileName = "sudo", Arguments = $"installer -pkg {installerPath} -target /", UseShellExecute = true };
            Process process = Process.Start(startInfo)!;
            await Task.Run(() => process.WaitForExit());

            if (File.Exists(installerPath))
            {
                File.Delete(installerPath);
            }

            if (process.ExitCode == 0)
            {
                CheckForUpdates();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoUpdate] Installation failed: {ex.Message}");
        }
    }

    private static void TryRunGitPull()
    {
        string output = RunGitCommand("pull", Application.dataPath + "/../");
        Debug.Log($"[AutoUpdate] Pull Result: {output}");
        AssetDatabase.Refresh();
    }

    [MenuItem("Thaumiel/Tools/Auto Update/Reset Setup")]
    private static void ResetSetup()
    {
        EditorPrefs.DeleteKey(PrefKey);
        Debug.Log("[AutoUpdate] Setup preference cleared.");
    }

    [MenuItem("Thaumiel/Tools/Auto Update/Check for Updates")]
    private static void ManualCheck()
    {
        CheckForUpdates();
    }
}