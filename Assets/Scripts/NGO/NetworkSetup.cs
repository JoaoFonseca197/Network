using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Linq;
using TMPro;



#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor;
#endif

#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
using System.Diagnostics;
using Unity.VisualScripting;
#endif


public class NetworkSetup : MonoBehaviour
{

    [SerializeField] private Character[] _character;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private UI _UI;

    private bool _isServer;
    private int  _playerCount;
    private void Awake()
    {
        _isServer = false;
        _playerCount = 0;
    }

    IEnumerator Start()
    {
        // Parse command line arguments
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.23f1\\Editor\\Unity.exe")
            {
                // --server found, this should be a server application
                _isServer = true;
            }
        }
        if (_isServer)
            yield return StartAsServerCR();
        else
            yield return StartAsClientCR();
    }


    IEnumerator StartAsServerCR()
    {
        SetWindowTitle("BitBullet - Server");
        var networkManager = GetComponent<NetworkManager>();
        networkManager.enabled = true;
        var transport = GetComponent<UnityTransport>();
        transport.enabled = true;
        // Wait a frame for setups to be done
        yield return null;


        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        if (networkManager.StartServer())
        {
            Debug.Log($"Serving on port {transport.ConnectionData.Port}...");
        }
        else
        {
            Debug.LogError($"Failed to serve on port {transport.ConnectionData.Port}...");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Player {clientId} connected, prefab index = {_playerCount}!");
        // Check a free spot for this player
        
        var spawnedPlayer = Instantiate(_character[_playerCount], _spawnPoints[0].position, Quaternion.identity);


        
        NetworkObject nto = spawnedPlayer.GetComponent<NetworkObject>();
        nto.SpawnAsPlayerObject(clientId);


        _textMeshProUGUI.text = "Spawned Character";
        _playerCount = (_playerCount+ 1) % _character.Length;
    }
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Player {clientId} disconnected!");
    }


    IEnumerator StartAsClientCR()
    {
        SetWindowTitle("BitBullet - Client");
        var networkManager = GetComponent<NetworkManager>();
        networkManager.enabled = true;
        var transport = GetComponent<UnityTransport>();
        transport.enabled = true;
        // Wait a frame for setups to be done
        yield return null;
        if (networkManager.StartClient())
        {
            Debug.Log($"Connecting on port {transport.ConnectionData.Port}...");
        }
        else
        {
            Debug.LogError($"Failed to connect on port {transport.ConnectionData.Port}...");
        }
    }
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowText(IntPtr hWnd, string lpString);
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")]
    static extern IntPtr EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    // Delegate to filter windows
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    private static IntPtr FindWindowByProcessId(uint processId)
    {
        IntPtr windowHandle = IntPtr.Zero;
        EnumWindows((hWnd, lParam) =>
        {
            uint windowProcessId;
            GetWindowThreadProcessId(hWnd, out windowProcessId);
            if (windowProcessId == processId)
            {
                windowHandle = hWnd;
                return false; // Found the window, stop enumerating
            }
            return true; // Continue enumerating
        }, IntPtr.Zero);
        return windowHandle;
    }
    static void SetWindowTitle(string title)
    {
#if !UNITY_EDITOR
        uint processId = (uint)Process.GetCurrentProcess().Id;
        IntPtr hWnd = FindWindowByProcessId(processId);
        if (hWnd != IntPtr.Zero)
        {
            SetWindowText(hWnd, title);
        }
#endif
    }
#else
    static void SetWindowTitle(string title)
    {
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Tools/Build Windows (x64)", priority = 0)]
    public static bool BuildGame()
    {
        // Specify build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = EditorBuildSettings.scenes
          .Where(s => s.enabled)
          .Select(s => s.path)
          .ToArray();
        buildPlayerOptions.locationPathName = Path.Combine("Builds", "MPWyzard.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        // Perform the build
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        // Output the result of the build
        Debug.Log($"Build ended with status: {report.summary.result}");
        // Additional log on the build, looking at report.summary
        return report.summary.result == BuildResult.Succeeded;
    }
    [MenuItem("Tools/Build and Launch (Server)", priority = 10)]
    public static void BuildAndLaunch1()
    {
        CloseAll();
        if (BuildGame())
        {
            Launch1();
        }
    }
    [MenuItem("Tools/Build and Launch (Server + Client)", priority = 20)]
    public static void BuildAndLaunch2()
    {
        CloseAll();
        if (BuildGame())
        {
            Launch2();
        }
    }
    [MenuItem("Tools/Launch (Server) _F11", priority = 30)]
    public static void Launch1()
    {
        Run("Builds\\MPWyzard.exe", "--server");
    }
    [MenuItem("Tools/Launch (Server + Client)", priority = 40)]
    public static void Launch2()
    {
        Run("Builds\\MPWyzard.exe", "--server");
        Run("Builds\\MPWyzard.exe", "");
    }

    [MenuItem("Tools/Close All", priority = 100)]
    public static void CloseAll()
    {
        // Get all processes with the specified name
        Process[] processes = Process.GetProcessesByName("MPWyzard");
        foreach (var process in processes)
        {
            try
            {
                // Close the process
                process.Kill();
                // Wait for the process to exit
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Handle exceptions, if any
                // This could occur if the process has already exited or you don't have permission to kill it
                Debug.LogWarning($"Error trying to kill process {process.ProcessName}: {ex.Message}");
            }
        }
    }



    private static void Run(string path, string args)
    {
        // Start a new process
        Process process = new Process();
        // Configure the process using the StartInfo properties
        process.StartInfo.FileName = path;
        process.StartInfo.Arguments = args;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal; // Choose the window style: Hidden, Minimized, Maximized, Normal
        process.StartInfo.RedirectStandardOutput = false; // Set to true to redirect the output (so you can read it in Unity)
        process.StartInfo.UseShellExecute = true; // Set to false if you want to redirect the output
                                                  // Run the process
        process.Start();
    }
#endif
}


