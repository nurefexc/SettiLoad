using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace SettiLoad.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        public enum FileType
        {
            Json,
            Xml
        }

        public enum TestType
        {
            AppConfig,
            AlternativeConfig,
            InvalidConfig,
            PartialConfig,
            MissingConfig,
            TypeTestConfig
        }

        
        private Dictionary<TestType, Dictionary<FileType, string>> filePaths;

        [SetUp]
        public void SetUp()
        {
            var testDirectory = TestContext.CurrentContext.TestDirectory;

            filePaths = new Dictionary<TestType, Dictionary<FileType, string>>
            {
                [TestType.AppConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "appConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "appConfig.xml") }
                },
                [TestType.AlternativeConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "alternativeConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "alternativeConfig.xml") }
                },
                [TestType.InvalidConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "invalidConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "invalidConfig.xml") }
                },
                [TestType.PartialConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "partialConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "partialConfig.xml") }
                },
                [TestType.MissingConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "missingConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "missingConfig.xml") }
                },
                [TestType.TypeTestConfig] = new Dictionary<FileType, string>
                {
                    { FileType.Json, Path.Combine(testDirectory, "TestConfigs", "json", "typeTestConfig.json") },
                    { FileType.Xml, Path.Combine(testDirectory, "TestConfigs", "xml", "typeTestConfig.xml") }
                }
            };
        }

        private void LoadAndVerifyConfig(TestType configType, FileType fileType, Action<AppConfig> verifyConfig)
        {
            var config = new AppConfig();
            var result = config.Load(filePaths[configType][fileType], true);

            Assert.IsTrue(result);
            verifyConfig(config);
        }

        [TestCase(TestType.AppConfig, FileType.Json)]
        [TestCase(TestType.AppConfig, FileType.Xml)]
        public void Load_ShouldLoadAppConfigSettings(TestType configType, FileType fileType)
        {
            LoadAndVerifyConfig(configType, fileType, config =>
            {
                Assert.IsNotNull(config.Database);
                Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
                Assert.AreEqual(30, config.Database.Timeout);
                Assert.IsNotNull(config.Logging);
                Assert.AreEqual("Info", config.Logging.LogLevel);
                Assert.AreEqual("/var/log/app.log", config.Logging.LogFilePath);
                Assert.IsTrue(config.IsFeatureEnabled);
            });
        }

        [TestCase(TestType.AlternativeConfig, FileType.Json)]
        [TestCase(TestType.AlternativeConfig, FileType.Xml)]
        public void Load_ShouldLoadAlternativeConfigSettings(TestType configType, FileType fileType)
        {
            LoadAndVerifyConfig(configType, fileType, config =>
            {
                Assert.IsNotNull(config.Database);
                Assert.AreEqual("Server=anotherServerAddress;Database=anotherDataBase;User Id=anotherUsername;Password=anotherPassword;", config.Database.ConnectionString);
                Assert.AreEqual(60, config.Database.Timeout);
                Assert.IsNotNull(config.Logging);
                Assert.AreEqual("Debug", config.Logging.LogLevel);
                Assert.AreEqual("/var/log/anotherapp.log", config.Logging.LogFilePath);
                Assert.IsFalse(config.IsFeatureEnabled);
            });
        }

        [TestCase(TestType.InvalidConfig, FileType.Json)]
        [TestCase(TestType.InvalidConfig, FileType.Xml)]
        public void Load_ShouldHandleInvalidConfigGracefully(TestType configType, FileType fileType)
        {
            var config = new AppConfig();
            var result = config.Load(filePaths[configType][fileType]);

            Assert.IsFalse(result);
            Assert.IsNull(config.Database);
            Assert.IsNull(config.Logging);
            Assert.IsFalse(config.IsFeatureEnabled);
        }

        [TestCase(TestType.PartialConfig, FileType.Json)]
        [TestCase(TestType.PartialConfig, FileType.Xml)]
        public void Load_ShouldHandleMissingFieldsInConfig(TestType configType, FileType fileType)
        {
            LoadAndVerifyConfig(configType, fileType, config =>
            {
                Assert.IsNotNull(config.Database);
                Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
                Assert.IsNull(config.Logging.LogLevel); // Logging should be null as it was not specified in the config
                Assert.IsFalse(config.IsFeatureEnabled); // Default value is false if not specified
            });
        }

        [TestCase(TestType.MissingConfig, FileType.Json)]
        [TestCase(TestType.MissingConfig, FileType.Xml)]
        public void Load_ShouldReturnFalseWhenConfigFileIsMissing(TestType configType, FileType fileType)
        {
            var config = new AppConfig();
            var result = config.Load(filePaths[configType][fileType]);

            Assert.IsFalse(result);
            Assert.IsNull(config.Database);
            Assert.IsNull(config.Logging);
            Assert.IsFalse(config.IsFeatureEnabled);
        }

        [TestCase(TestType.TypeTestConfig, FileType.Json)]
        [TestCase(TestType.TypeTestConfig, FileType.Xml)]
        public void Load_ShouldHandleDifferentDataTypes(TestType configType, FileType fileType)
        {
            LoadAndVerifyConfig(configType, fileType, config =>
            {
                Assert.IsNotNull(config.Database);
                Assert.AreEqual("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;", config.Database.ConnectionString);
                Assert.AreEqual(45, config.Database.Timeout);
                Assert.IsNotNull(config.Logging);
                Assert.AreEqual("Verbose", config.Logging.LogLevel);
                Assert.AreEqual("/var/log/complex.log", config.Logging.LogFilePath);
                Assert.IsTrue(config.IsFeatureEnabled);
            });
        }
    }
}
