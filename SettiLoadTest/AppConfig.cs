using SettiLoad;

public class AppConfig : Config
{
    public DatabaseConfig Database { get; set; }
    public LoggingConfig Logging { get; set; }
    public bool IsFeatureEnabled { get; set; }

    public class DatabaseConfig
    {
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public string ConnectionString =>
            $"Server={Server};Database={DatabaseName};User Id={UserId};Password={Password};";
        public int Timeout { get; set; }
    }


    public class LoggingConfig
    {
        public string LogLevel { get; set; }
        public string LogFilePath { get; set; }
    }
}