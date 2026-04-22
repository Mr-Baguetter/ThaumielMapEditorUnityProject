using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics;
using System.IO;
using System.Collections;
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
                TryRunGitPull();

            return;
        }

        EditorApplication.delayCall += PromptUser;
    }

    private static void PromptUser()
    {
        bool wantsAutoUpdate = EditorUtility.DisplayDialog(
            "Automatic Updates",
            "Would you like to enable automatic updates from GitHub?\n\nThis will install Git if it is not already installed.",
            "Yes, enable updates",
            "No thanks"
        );

        EditorPrefs.SetBool(PrefKey, wantsAutoUpdate);

        if (!wantsAutoUpdate)
            return;

        if (IsGitInstalled())
        {
            Debug.Log("[AutoUpdate] Git already installed, proceeding.");
            TryRunGitPull();
        }
        else
            _ = DownloadAndInstallGitAsync();
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

        if (request.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError($"[AutoUpdate] Failed to download Git: {request.error}");
            return;
        }

        UnityEngine.Debug.Log("[AutoUpdate] Git downloaded, launching installer...");
        await InstallGitAsync(installerPath);
    }

    private static async Task InstallGitAsync(string installerPath)
    {
        try
        {
            ProcessStartInfo startInfo = Application.platform == RuntimePlatform.WindowsEditor
                ? new ProcessStartInfo
                {
                    FileName = installerPath,
                    Arguments = "/VERYSILENT /NORESTART /NOCANCEL /SP- /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS",
                    UseShellExecute = true
                }
                : new ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = $"installer -pkg {installerPath} -target /",
                    UseShellExecute = true
                };

            Process process = Process.Start(startInfo)!;
            EditorUtility.DisplayProgressBar("Auto Update Setup", "Installing Git, please wait...", 1f);
            await Task.Run(() => process.WaitForExit());
            EditorUtility.ClearProgressBar();

            if (File.Exists(installerPath))
                File.Delete(installerPath);

            if (process.ExitCode == 0)
            {
                Debug.Log("[AutoUpdate] Git installed successfully.");
                TryRunGitPull();
            }
            else
                Debug.LogError($"[AutoUpdate] Git installer exited with code {process.ExitCode}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoUpdate] Failed to launch Git installer: {ex.Message}");
        }
    }

    private static IEnumerator WaitForInstall(Process process)
    {
        EditorUtility.DisplayProgressBar("Auto Update Setup", "Installing Git, please wait...", 1f);

        while (!process.HasExited)
            yield return null;

        EditorUtility.ClearProgressBar();

        if (File.Exists(InstallerPath))
            File.Delete(InstallerPath);

        if (process.ExitCode == 0)
        {
            Debug.Log("[AutoUpdate] Git installed successfully.");
            TryRunGitPull();
        }
        else
            Debug.LogError($"[AutoUpdate] Git installer exited with code {process.ExitCode}.");
    }

    private static void TryRunGitPull()
    {
        try
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "pull",
                    WorkingDirectory = Application.dataPath + "/../",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Debug.Log($"[AutoUpdate] {output}");
                AssetDatabase.Refresh();
            }
            else
                Debug.LogError($"[AutoUpdate] Git pull failed: {error}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoUpdate] Exception during git pull: {ex.Message}");
        }
    }

    [MenuItem("Thaumiel/Tools/Auto Update/Reset Setup")]
    private static void ResetSetup()
    {
        EditorPrefs.DeleteKey(PrefKey);
        Debug.Log("[AutoUpdate] Setup preference cleared. Will prompt on next Unity load.");
    }

    [MenuItem("Thaumiel/Tools/Auto Update/Pull Now")]
    private static void PullNow()
    {
        TryRunGitPull();
    }
}