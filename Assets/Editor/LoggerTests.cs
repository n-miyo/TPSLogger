// -*- mode:csharp -*-
//
// MIT License.

using TPS;

using System.IO;

using NUnit.Framework;

[TestFixture]
public class LoggerTest
{
    Logger logger;

    [SetUp]
    public void SetUp()
    {
        logger = Logger.DefaultLogger;
        logger.AcceptLogLevel = Logger.LogLevel.INFO;
        logger.StoreLineNumber = 3;
        logger.EnableFileLogging = false;
    }

    [TearDown]
    public void TearDown()
    {
    }

    [TestCase()]
    public void StoredLinesTest()
    {
        string e;

        logger.LogAsAlert("Alert.");
        logger.LogAsError("Error.");
        logger.LogAsWarning("Warning.");
        logger.LogAsInfo("Info.");

        e = @"EROR: Error.
WARN: Warning.
INFO: Info.";
	Assert.AreEqual(e, logger.StoredLines);

        logger.LogAsInfo("Info2.");
        logger.LogAsWarning("Warning2.");
        logger.LogAsError("Error2.");
        logger.LogAsAlert("Alert2.");
        logger.StoreLineNumber = 2;

        e = @"EROR: Error2.
ALRT: Alert2.";
	Assert.AreEqual(e, logger.StoredLines);

	Assert.AreEqual("", logger.StoredLines);
    }

    [TestCase()]
    public void LogFilterTest()
    {
        string e;

        logger.ClearStoredLines();
	Assert.AreEqual("", logger.StoredLines);
        logger.AcceptLogLevel = Logger.LogLevel.ALERT;

        logger.LogAsAlert("Alert.");
        logger.LogAsError("Error.");
        logger.LogAsWarning("Warning.");
        logger.LogAsInfo("Info.");

        e = @"ALRT: Alert.";
	Assert.AreEqual(e, logger.StoredLines);

        logger.AcceptLogLevel = Logger.LogLevel.ERROR;
        logger.LogAsInfo("Info2.");
        logger.LogAsWarning("Warning2.");
        logger.LogAsError("Error2.");
        logger.LogAsAlert("Alert2.");

        e = @"EROR: Error2.
ALRT: Alert2.";
	Assert.AreEqual(e, logger.StoredLines);
    }

    [TestCase()]
    public void FileLogTest()
    {
        logger.LogFileName = "loggerTest";
        logger.EnableFileLogging = true;

        if (File.Exists(logger.LogFilePath)) {
            File.Delete(logger.LogFilePath);
        }

        logger.LogAsAlert("Alert.");
        logger.LogAsError("Error.");
        logger.LogAsWarning("Warning.");
        logger.LogAsInfo("Info.");

        string e = @"ALRT: Alert.
EROR: Error.
WARN: Warning.
INFO: Info.
";
        string s = null;
        using (StreamReader sr = new StreamReader(logger.LogFilePath)) {
            s = sr.ReadToEnd();
        }
	Assert.AreEqual(e, s);
    }
}

// EOF
