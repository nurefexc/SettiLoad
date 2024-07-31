using SettiLoad;

public class AppConfig : Config
{
    public DatabaseConfig Database { get; set; }
    public LoggingConfig Logging { get; set; }
    public bool IsFeatureEnabled { get; set; }

    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public int Timeout { get; set; }
    }

    public class LoggingConfig
    {
        public string LogLevel { get; set; }
        public string LogFilePath { get; set; }
    }
}