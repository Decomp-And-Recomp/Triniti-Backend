using System.Diagnostics;

namespace T.TNet;

public class TNetInstance
{
    public readonly int port;
    public readonly int gameId;
    public string name = "Unnamed";

    public Process process;

    readonly EventHandler shutdownHandler;

    public bool active { get; private set; }

    public List<string?> logs = [];

    public TNetInstance(int port, int gameId)
    {
        this.port = port;
        this.gameId = gameId;

        process = new();
        process.StartInfo.FileName = "TNet.exe";
        process.StartInfo.Arguments = $"{port} {gameId}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.OutputDataReceived += OnOutputReceived;
        process.ErrorDataReceived += OnOutputReceived;

        shutdownHandler = (_, _) =>
        {
            if (process == null || !active) return;

            process.Kill(true);
            process?.Dispose();
        };
    }

    public void Start()
    {
        if (active) return;

        active = true;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        AppDomain.CurrentDomain.ProcessExit += shutdownHandler;
    }

    public void Stop()
    {
        if (!active) return;

        active = false;

        process.Kill(true);
        AppDomain.CurrentDomain.ProcessExit -= shutdownHandler;
    }

    public void OnOutputReceived(object sender, DataReceivedEventArgs e)
    {
        logs.Add(e.Data);
    }
}
