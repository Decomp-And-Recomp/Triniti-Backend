using System.Net;
using System.Text;
using TNet.HeadConnection;
using TNet.Server;

namespace TNet;

internal class Program
{
    static async Task Main(string[] args)
    {
        // basic console initing
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        TaskScheduler.UnobservedTaskException += OnTaskException;

        Console.WriteLine("TNet Backend, made by overmet15.");

        _ = Debug.StartFileWriting();

        //await Head.Start(int.Parse(args[2]), int.Parse(args[3]));

        await Lobby.Run(IPAddress.Any, InitPort(args), (Game)int.Parse(args[1]));
    }

    static void OnTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Debug.Log("UNOBSERVED EXCEPTION(S):", ConsoleColor.DarkRed);
        foreach (Exception ex in args.Exception.InnerExceptions)
        {
            Debug.LogException(ex);
        }

        args.SetObserved();
    }

    static int InitPort(string[] args)
    {
        int parse;

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out parse)) return parse;
        }

        throw new Exception("Port not found in argument 0");
    }
}
