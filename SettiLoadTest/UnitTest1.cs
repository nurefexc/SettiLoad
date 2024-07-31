namespace SettiLoad.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        // Paths to the configuration files used in the tests
        private string appConfigFilePath;
        private string alternativeConfigFilePath;
        private string invalidJsonFilePath;
        private string partialConfigFilePath;
        private string missingConfigFilePath;
        private string typeTestConfigFilePath;

        [SetUp]
        public void SetUp()
        {
            // Get the path to the current test directory
            var testDirectory = TestContext.CurrentContext.TestDirectory;
            
            // Set paths to configuration files located in the TestConfigs directory
            appConfigFilePath = Path.Combine(testDirectory, "TestConfigs", "appConfig.json");
            alternativeConfigFilePath = Path.Combine(testDirectory, "TestConfigs", "alternativeConfig.json");
            invalidJsonFilePath = Path.Combine(testDirectory, "TestConfigs", "invalidConfig.json");
            partialConfigFilePath = Path.Combine(testDirectory, "TestConfigs", "partialConfig.json");
            missingConfigFilePath = Path.Combine(testDirectory, "TestConfigs", "missingConfig.json");
            typeTestConfigFilePath = Path.Combine(testDirectory, "TestConfigs", "typeTestConfig.json");
        }

        [Test]
        public void Load_ShouldLoadAppConfigSettingsFromJson()
        {
            // Create a new instance of AppConfig and load settings from the appConfig.json file
            var config = new AppConfig();
            var result = config.Load(appConfigFilePath);

            // Verify that the load operation was successful
            Assert.IsTrue(result);

            // Verify that the loaded configuration matches expected values
            Assert.IsNotNull(config.Database);
            Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
            Assert.AreEqual(30, config.Database.Timeout);
            Assert.IsNotNull(config.Logging);
            Assert.AreEqual("Info", config.Logging.LogLevel);
            Assert.AreEqual("/var/log/app.log", config.Logging.LogFilePath);
            Assert.IsTrue(config.IsFeatureEnabled);
        }

        [Test]
        public void Load_ShouldLoadAlternativeConfigSettingsFromJson()
        {
            // Create a new instance of AppConfig and load settings from the alternativeConfig.json file
            var config = new AppConfig();
            var result = config.Load(alternativeConfigFilePath);

            // Verify that the load operation was successful
            Assert.IsTrue(result);

            // Verify that the loaded configuration matches expected values for the alternative configuration
            Assert.IsNotNull(config.Database);
            Assert.AreEqual("Server=anotherServerAddress;Database=anotherDataBase;User Id=anotherUsername;Password=anotherPassword;", config.Database.ConnectionString);
            Assert.AreEqual(60, config.Database.Timeout);
            Assert.IsNotNull(config.Logging);
            Assert.AreEqual("Debug", config.Logging.LogLevel);
            Assert.AreEqual("/var/log/anotherapp.log", config.Logging.LogFilePath);
            Assert.IsFalse(config.IsFeatureEnabled);
        }

        [Test]
        public void Load_ShouldHandleInvalidJsonGracefully()
        {
            // Create a new instance of AppConfig and load settings from the invalidConfig.json file
            var config = new AppConfig();
            var result = config.Load(invalidJsonFilePath);

            // Verify that the load operation returns false due to invalid JSON
            Assert.IsFalse(result);

            // Verify that no settings were loaded due to invalid JSON
            Assert.IsNull(config.Database);
            Assert.IsNull(config.Logging);
            Assert.IsFalse(config.IsFeatureEnabled);
        }

        [Test]
        public void Load_ShouldHandleMissingFieldsInJson()
        {
            // Create a new instance of AppConfig and load settings from the partialConfig.json file
            var config = new AppConfig();
            var result = config.Load(partialConfigFilePath);

            // Verify that the load operation was successful despite missing fields
            Assert.IsTrue(result);

            // Verify that the loaded configuration has the fields that were present
            Assert.IsNotNull(config.Database);
            Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
            Assert.IsNull(config.Logging.LogLevel); // Logging should be null as it was not specified in the JSON
            Assert.IsFalse(config.IsFeatureEnabled); // Default value is false if not specified
        }

        [Test]
        public void Load_ShouldReturnFalseWhenConfigFileIsMissing()
        {
            // Create a new instance of AppConfig and attempt to load settings from a missing configuration file
            var config = new AppConfig();
            var result = config.Load(missingConfigFilePath);

            // Verify that the load operation returns false as the file is missing
            Assert.IsFalse(result);

            // Verify that no settings were loaded due to the missing file
            Assert.IsNull(config.Database);
            Assert.IsNull(config.Logging);
            Assert.IsFalse(config.IsFeatureEnabled);
        }

        [Test]
        public void Load_ShouldHandleDifferentJsonDataTypes()
        {
            // Create a new instance of AppConfig and load settings from the typeTestConfig.json file
            var config = new AppConfig();
            var result = config.Load(typeTestConfigFilePath);

            // Verify that the load operation was successful
            Assert.IsTrue(result);

            // Verify that the loaded configuration matches expected values with different data types
            Assert.IsNotNull(config.Database);
            Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
            Assert.AreEqual(45, config.Database.Timeout);
            Assert.IsNotNull(config.Logging);
            Assert.AreEqual("Verbose", config.Logging.LogLevel);
            Assert.AreEqual("/var/log/complex.log", config.Logging.LogFilePath);
            Assert.IsTrue(config.IsFeatureEnabled);
        }
    }
}
