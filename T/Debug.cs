﻿namespace T;

internal static class Debug
{
    static readonly object logLock = new();

    static List<string>? fileWriteQueue;

    public static async Task StartFileWriting()
    {
        if (fileWriteQueue != null)
        {
            LogError("File Writing is already started");
            return;
        }
        fileWriteQueue = [];

        if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

        string path = Path.Combine("Logs", DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");
        File.Create(path).Close();

        List<string> toWrite = [];

        while (true)
        {
            while (fileWriteQueue.Count > 0)
            {
                toWrite.AddRange(fileWriteQueue);
                fileWriteQueue.Clear();

                await File.WriteAllLinesAsync(path, toWrite);
                toWrite.Clear();

                await Task.Yield();
            }

            await Task.Delay(1);
        }
    }

    ///<summary>Logs message with timestamp.</summary>
    public static void Log(object? message, ConsoleColor color = ConsoleColor.White)
    {
        lock (logLock)
        {
            string msg = message?.ToString() ?? string.Empty;

            var time = DateTime.UtcNow;

            if (string.IsNullOrEmpty(msg))
            {
                LogWarning("[The message was empty.]");
                return;
            }

            string postMsg = time.ToString("[HH:mm:ss:fff] ") + msg;

            fileWriteQueue?.Add(postMsg);

            Console.ForegroundColor = color;
            Console.WriteLine(postMsg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }


    ///<summary>Logs message without any additional text.</summary>
    public static void Write(object? message, ConsoleColor color = ConsoleColor.White)
    {
        lock (logLock)
        {
            string msg = message?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(msg))
            {
                LogWarning("[The message was empty.]");
                return;
            }

            fileWriteQueue?.Add(msg);

            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    ///<summary>Logs a 'DEBUG' BUILD ONLY log</summary>
    public static void LogInfo(object msg, ConsoleColor color = ConsoleColor.Cyan)
    {
#if DEBUG
        Log(msg, color);
#endif
    }

    ///<summary>Logs message and stack trace</summary>
    public static void LogWarning(object msg)
    {
        Log(msg, ConsoleColor.Yellow);
        LogStack(ConsoleColor.DarkYellow);
    }

    ///<summary>Logs message and stack trace</summary>
    public static void LogError(object msg)
    {
        Log(msg, ConsoleColor.Red);
        LogStack(ConsoleColor.DarkRed);
    }

    ///<summary>Logs [message] (from exception) and [stack trace]</summary>
    public static void LogException(Exception ex, bool includeStackTrace = true)
    {
        Log($"[message]: {ex.Message}", ConsoleColor.Red);
        if (includeStackTrace) Log($"[stack trace]: {ex.StackTrace}", ConsoleColor.Red);
    }

    ///<summary>Logs message with [message] (from exception) and [stack trace]</summary>
    public static void LogException(object message, Exception ex, bool includeStackTrace = true)
    {
        Log(message, ConsoleColor.Red);
        Log($"[message]: {ex.Message}", ConsoleColor.Red);
        if (includeStackTrace) Log($"[stack trace]: {ex.StackTrace}", ConsoleColor.Red);
    }

#pragma warning disable
    public static void LogStack(ConsoleColor color = ConsoleColor.White)
    {
//#if DEBUG
        System.Diagnostics.StackTrace stackTrace = new(true);

        for (int i = 1; i < stackTrace.FrameCount; i++) // yes i is supposed to start with 1
        {
            System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(i);

            string methodName = callerFrame.GetMethod().Name;
            string className = callerFrame.GetMethod().DeclaringType.FullName;
            int line = callerFrame.GetFileLineNumber();

            Log($"{className}.{methodName}:{line}", color);
        }
//#endif
    }
#pragma warning restore
}
