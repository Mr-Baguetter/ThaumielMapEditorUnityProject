using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using System;
using System.Linq;

[InitializeOnLoad]
public class AutoUpdateSetup
{
    private const string RepoUrl = "https://github.com/Mr-Baguetter/ThaumielMapEditorUnityProject.git";
    private const string RemoteName = "origin";
    private const string BranchName = "master";

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
            EnsureRemoteAndCheck();
        }
        else
        {
            _ = DownloadAndInstallGitAsync();
        }
    }

    private static void EnsureRemoteAndCheck()
    {
        string rootDir = Application.dataPath + "/../";

        try
        {
            string gitDir = Path.GetFullPath(Path.Combine(rootDir, ".git"));
            if (!Directory.Exists(gitDir))
            {
                bool initRepo = EditorUtility.DisplayDialog(
                    "Not a Git Repository",
                    "This project is not a Git repository. Would you like to initialize it now?",
                    "Yes, initialize",
                    "Cancel"
                );

                if (!initRepo)
                {
                    Debug.LogWarning("[AutoUpdate] User cancelled git init. Auto-update disabled.");
                    EditorPrefs.SetBool(PrefKey, false);
                    return;
                }

                GitResult initResult = RunGitCommand("init", rootDir);
                if (initResult.ExitCode != 0)
                {
                    Debug.LogError($"[AutoUpdate] git init failed: {initResult.Error}");
                    return;
                }
                Debug.Log("[AutoUpdate] Initialized git repository.");
            }

            GitResult remoteResult = RunGitCommand($"remote get-url {RemoteName}", rootDir);
            string currentUrl = remoteResult.Output.Trim();

            if (string.IsNullOrEmpty(currentUrl))
            {
                RunGitCommand($"remote add {RemoteName} {RepoUrl}", rootDir);
                Debug.Log($"[AutoUpdate] Added remote '{RemoteName}' -> {RepoUrl}");
            }
            else if (!currentUrl.Equals(RepoUrl, StringComparison.OrdinalIgnoreCase))
            {
                RunGitCommand($"remote set-url {RemoteName} {RepoUrl}", rootDir);
                Debug.Log($"[AutoUpdate] Updated remote '{RemoteName}' from {currentUrl} -> {RepoUrl}");
            }
            else
            {
                Debug.Log($"[AutoUpdate] Remote '{RemoteName}' is already pointing to {RepoUrl}");
            }

            Debug.Log($"[AutoUpdate] Fetching from '{RemoteName}'...");
            GitResult fetchResult = RunGitCommand($"fetch {RemoteName}", rootDir);
            if (fetchResult.ExitCode != 0)
            {
                Debug.LogWarning($"[AutoUpdate] Fetch failed (exit {fetchResult.ExitCode}): {fetchResult.Error}");
                return;
            }

            GitResult branchCheck = RunGitCommand($"ls-remote --heads {RemoteName} {BranchName}", rootDir);
            if (string.IsNullOrWhiteSpace(branchCheck.Output))
            {
                Debug.LogWarning($"[AutoUpdate] Branch '{BranchName}' not found on remote. Available branches:");
                GitResult branches = RunGitCommand($"ls-remote --heads {RemoteName}", rootDir);
                Debug.Log(branches.Output);
                return;
            }

            GitResult localBranchCheck = RunGitCommand($"rev-parse --verify {BranchName}", rootDir);
            if (localBranchCheck.ExitCode != 0)
            {
                GitResult checkoutResult = RunGitCommand($"checkout -b {BranchName} {RemoteName}/{BranchName}", rootDir);
                if (checkoutResult.ExitCode != 0)
                {
                    Debug.LogWarning($"[AutoUpdate] Failed to create local branch: {checkoutResult.Error}");
                    return;
                }
                Debug.Log($"[AutoUpdate] Created and checked out local branch '{BranchName}' tracking remote.");
            }
            else
            {
                GitResult upstreamCheck = RunGitCommand($"rev-parse --abbrev-ref {BranchName}@{{upstream}}", rootDir);
                if (upstreamCheck.ExitCode != 0 || !upstreamCheck.Output.Trim().Equals($"{RemoteName}/{BranchName}", StringComparison.OrdinalIgnoreCase))
                {
                    RunGitCommand($"branch --set-upstream-to={RemoteName}/{BranchName} {BranchName}", rootDir);
                    Debug.Log($"[AutoUpdate] Set upstream for '{BranchName}' to {RemoteName}/{BranchName}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AutoUpdate] Could not configure remote: {ex.Message}");
        }

        CheckForUpdates();
    }

    private static void CheckForUpdates()
    {
        if (!IsGitInstalled())
        {
            Debug.LogWarning("[AutoUpdate] Git is not installed or not on PATH. Cannot check for updates.");
            return;
        }

        try
        {
            string rootDir = Application.dataPath + "/../";

            GitResult currentBranchResult = RunGitCommand("rev-parse --abbrev-ref HEAD", rootDir);
            string currentBranch = currentBranchResult.Output.Trim();

            if (!currentBranch.Equals(BranchName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"[AutoUpdate] Currently on branch '{currentBranch}', switching to '{BranchName}'...");
                GitResult switchResult = RunGitCommand($"checkout {BranchName}", rootDir);
                if (switchResult.ExitCode != 0)
                {
                    Debug.LogWarning($"[AutoUpdate] Failed to switch branch: {switchResult.Error}");
                    return;
                }
            }

            Debug.Log($"[AutoUpdate] Fetching from '{RemoteName}'...");
            GitResult fetchResult = RunGitCommand($"fetch {RemoteName}", rootDir);
            if (fetchResult.ExitCode != 0)
            {
                Debug.LogWarning($"[AutoUpdate] Fetch failed (exit {fetchResult.ExitCode}): {fetchResult.Error}");
                return;
            }

            GitResult revListResult = RunGitCommand($"rev-list HEAD..{RemoteName}/{BranchName} --count", rootDir);
            if (revListResult.ExitCode != 0)
            {
                Debug.LogWarning($"[AutoUpdate] Could not compare revisions (exit {revListResult.ExitCode}): {revListResult.Error}");
                return;
            }

            string status = revListResult.Output.Trim();

            if (!int.TryParse(status, out int count))
            {
                Debug.LogWarning($"[AutoUpdate] Unexpected output from rev-list: '{status}'");
                return;
            }

            if (count <= 0)
            {
                Debug.Log("[AutoUpdate] Project is up to date.");
                return;
            }

            GitResult logResult = RunGitCommand($"log HEAD..{RemoteName}/{BranchName} --oneline -n 5", rootDir);
            string commitMessages = logResult.Output;

            bool update = EditorUtility.DisplayDialog(
                "Update Available",
                $"There are {count} new update(s) available.\n\nRecent changes:\n{commitMessages}\n\nWould you like to pull these updates now?",
                "Update Now",
                "Later"
            );

            if (update)
                TryRunGitPull();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AutoUpdate] Could not check for updates: {ex.Message}");
        }
    }

    private readonly struct GitResult
    {
        public readonly string Output;
        public readonly string Error;
        public readonly int ExitCode;

        public GitResult(string output, string error, int exitCode)
        {
            Output = output;
            Error = error;
            ExitCode = exitCode;
        }
    }

    private static GitResult RunGitCommand(string args, string workingDir)
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
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return new GitResult(output, error, process.ExitCode);
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

        Directory.CreateDirectory("Temp");

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
        {
            await InstallGitAsync(installerPath);
        }
        else
            Debug.LogError($"[AutoUpdate] Download failed: {request.error}");
    }

    private static async Task InstallGitAsync(string installerPath)
    {
        try
        {
            ProcessStartInfo startInfo = Application.platform == RuntimePlatform.WindowsEditor
                ? new ProcessStartInfo { FileName = installerPath, Arguments = "/VERYSILENT /NORESTART", UseShellExecute = true }
                : new ProcessStartInfo { FileName = "sudo", Arguments = $"installer -pkg {installerPath} -target /", UseShellExecute = true };

            Process process = Process.Start(startInfo)!;
            await Task.Run(() => process.WaitForExit());

            if (File.Exists(installerPath))
                File.Delete(installerPath);

            if (process.ExitCode == 0)
            {
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH"));
                EnsureRemoteAndCheck();
            }
            else
            {
                Debug.LogError($"[AutoUpdate] Git installer exited with code {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoUpdate] Installation failed: {ex.Message}");
        }
    }

    private static void TryRunGitPull()
    {
        string rootDir = Application.dataPath + "/../";

        GitResult statusResult = RunGitCommand("status --porcelain", rootDir);
        bool hasLocalChanges = !string.IsNullOrWhiteSpace(statusResult.Output);

        if (hasLocalChanges)
        {
            bool forcePull = EditorUtility.DisplayDialog(
                "Local Changes Detected",
                "You have uncommitted local changes. Pulling may cause merge conflicts.\n\nWould you like to stash your changes and pull?",
                "Stash and Pull",
                "Cancel"
            );

            if (!forcePull)
            {
                Debug.Log("[AutoUpdate] Pull cancelled by user due to local changes.");
                return;
            }

            GitResult stashResult = RunGitCommand("stash push -m \"AutoUpdate stash\"", rootDir);
            if (stashResult.ExitCode != 0)
            {
                Debug.LogWarning($"[AutoUpdate] Stash failed: {stashResult.Error}");
                return;
            }
            Debug.Log("[AutoUpdate] Local changes stashed.");
        }

        GitResult result = RunGitCommand($"pull {RemoteName} {BranchName}", rootDir);

        if (result.ExitCode != 0)
        {
            Debug.LogWarning($"[AutoUpdate] Pull failed (exit {result.ExitCode}): {result.Error}");
            return;
        }

        Debug.Log($"[AutoUpdate] Pull result: {result.Output}");

        GitResult verifyResult = RunGitCommand($"rev-parse --short HEAD", rootDir);
        Debug.Log($"[AutoUpdate] Now at commit: {verifyResult.Output.Trim()}");

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Update Complete",
            "The project has been updated to the latest version.\n\nUnity will now refresh assets.",
            "OK"
        );
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
        EnsureRemoteAndCheck();
    }

    [MenuItem("Thaumiel/Tools/Auto Update/Force Pull Latest")]
    private static void ForcePull()
    {
        TryRunGitPull();
    }
}