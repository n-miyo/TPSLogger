// -*- mode:csharp -*-
//
// MIT License.
//

using UnityEngine;

using System;
using System.IO;
using System.Linq;

namespace TPS
{

public class Logger
{
    static Logger defaultLogger = new Logger();

    static string ALERT_PREFIX = "ALRT: ";
    static string ERROR_PREFIX = "EROR: ";
    static string WARNING_PREFIX = "WARN: ";
    static string INFO_PREFIX = "INFO: ";

    static int DefaultSoreLineNumber = 10;

    [System.Flags]
    public enum LogLevel
    {
        ALERT = 0x1,
        ERROR = 0x2,
        WARNING = 0x3,
        INFO = 0x4
    }

    // Properties
    public bool EnableLogging
    {
        get; set;
    }

    public bool EnableFileLogging
    {
        get; set;
    }

    public LogLevel AcceptLogLevel
    {
        get; set;
    }

    public int StoreLineNumber
    {
        get; set;
    }

    string storedLines;
    public string StoredLines
    {
        get
        {
            if (StoreLineNumber == 0 || storedLines == "") {
                storedLines = "";
                return storedLines;
            }

            string[] r =
                storedLines.Split(new string[] {Environment.NewLine},
                                  StringSplitOptions.RemoveEmptyEntries);
            string s = r.Skip(Math.Max(0, r.Count() - StoreLineNumber))
                .Take(StoreLineNumber)
                .Aggregate((a, b) => a + Environment.NewLine + b);
            storedLines = "";

            return s;
        }
    }

    string logFileName;
    public string LogFileName
    {
        get
        {
            return logFileName;
        }

        set
        {
            logFileName = value;

            logFilePath = null;
            if (!string.IsNullOrEmpty(logFileName)) {
                logFilePath = Application.temporaryCachePath + "/" + value;
            }
        }
    }

    string logFilePath;
    public string LogFilePath
    {
        get
        {
            return logFilePath;
        }
    }

    // Static
    public static Logger DefaultLogger
    {
        get
        {
            return defaultLogger;
        }
    }

    // Constructor
    Logger()
    {
        EnableLogging = true;
        EnableFileLogging = false;
        AcceptLogLevel = LogLevel.INFO;
        StoreLineNumber = DefaultSoreLineNumber;
        storedLines = "";
        logFileName = null;
    }

    // Public
    public void LogAsAlert(string message) {
        Log(LogLevel.ALERT, message);
    }

    public void LogAsError(string message) {
        Log(LogLevel.ERROR, message);
    }

    public void LogAsWarning(string message) {
        Log(LogLevel.WARNING, message);
    }

    public void LogAsInfo(string message) {
        Log(LogLevel.INFO, message);
    }

    public void Log(LogLevel level, string message)
    {
        if (!EnableLogging
            || level > AcceptLogLevel) {
            return;
        }

        string p = null;
        switch (level)
        {
            case LogLevel.ALERT:
                p = ALERT_PREFIX;
                break;
            case LogLevel.ERROR:
                p = ERROR_PREFIX;
                break;
            case LogLevel.WARNING:
                p = WARNING_PREFIX;
                break;
            case LogLevel.INFO:
                p = INFO_PREFIX;
                break;
            default:
                break;                  // NOTHING TO DO.
        }

        string s = "";
        if (p != null) {
            s = p + message;
        }
        WriteToFile(s);
        AppendToStoredLines(s);
    }

    public void ClearStoredLines()
    {
        storedLines = "";
    }

    // Private
    void WriteToFile(string message)
    {
        if (EnableFileLogging == false || string.IsNullOrEmpty(LogFilePath)) {
            return;
        }

        using (StreamWriter sw = File.AppendText(LogFilePath))
        {
            sw.WriteLine(message);
        }
    }

    void AppendToStoredLines(string message)
    {
        if (StoreLineNumber <= 0) {
            return;
        }

        storedLines += message;
        if (!storedLines.EndsWith(Environment.NewLine)) {
            storedLines += Environment.NewLine;
        }
    }
}

}

// EOF
