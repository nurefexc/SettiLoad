using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace SettiLoad
{
    /// <summary>
    /// An abstract base class for creating configurations that can be loaded from a configuration file.
    /// Supports loading from JSON and XML files.
    /// </summary>
    public abstract class Config
    {
        /// <summary>
        /// Loads the configuration from the specified file.
        /// Supported file types: JSON (.json) and XML (.xml).
        /// </summary>
        /// <param name="filePath">The path to the configuration file.</param>
        /// <param name="throwException">Indicates whether to throw an exception if loading fails.</param>
        /// <returns>Returns true if the configuration was successfully loaded; otherwise, returns false.</returns>
        public bool Load(string filePath, bool throwException = false)
        {
            try
            {
                var fileExtension = Path.GetExtension(filePath).ToLower();
                bool success = false;

                if (fileExtension == ".json")
                {
                    // Attempt to load as JSON.
                    success = TryLoadJson(filePath);
                }
                else if (fileExtension == ".xml")
                {
                    // Attempt to load as XML.
                    success = TryLoadXml(filePath);
                }
                else
                {
                    // Attempt both JSON and XML.
                    success = TryLoadJson(filePath) || TryLoadXml(filePath);
                }

                if (!success)
                {
                    throw new NotSupportedException("The configuration file format is not supported or could not be parsed.");
                }

                return true;
            }
            catch (Exception)
            {
                if (throwException)
                {
                    // Rethrow the exception if specified.
                    throw;
                }
                // Return false if an exception occurred and throwException is false.
                return false;
            }
        }

        /// <summary>
        /// Tries to load the configuration from a JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON configuration file.</param>
        /// <returns>Returns true if the configuration was successfully loaded; otherwise, returns false.</returns>
        private bool TryLoadJson(string filePath)
        {
            try
            {
                // Read the entire text content from the specified JSON file.
                var json = File.ReadAllText(filePath);

                // Parse the JSON text into a JsonDocument.
                var jsonDoc = JsonDocument.Parse(json);

                // Apply settings from the JSON document to this configuration object.
                ApplyJsonSettings(this, jsonDoc.RootElement);

                return true;
            }
            catch
            {
                // Return false if parsing or applying settings failed.
                return false;
            }
        }

        /// <summary>
        /// Tries to load the configuration from an XML file.
        /// </summary>
        /// <param name="filePath">The path to the XML configuration file.</param>
        /// <returns>Returns true if the configuration was successfully loaded; otherwise, returns false.</returns>
        private bool TryLoadXml(string filePath)
        {
            try
            {
                // Read the entire text content from the specified XML file.
                var xml = File.ReadAllText(filePath);

                // Parse the XML text into an XmlDocument.
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                // Apply settings from the XML document to this configuration object.
                ApplyXmlSettings(this, xmlDoc.DocumentElement);

                return true;
            }
            catch
            {
                throw;
                // Return false if parsing or applying settings failed.
                return false;
            }
        }

        /// <summary>
        /// Recursively applies settings from a JSON element to the corresponding properties of the configuration object.
        /// </summary>
        /// <param name="propertyConfig">The configuration object whose properties are to be set.</param>
        /// <param name="jsonElement">The JSON element containing the settings to be applied.</param>
        private static void ApplyJsonSettings(object propertyConfig, JsonElement jsonElement)
        {
            // Iterate over each property in the JSON element.
            foreach (var property in jsonElement.EnumerateObject())
            {
                // Get the property information of the configuration object that matches the current JSON property name.
                var propInfo = propertyConfig.GetType().GetProperty(property.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // Check if the property exists and is a class (excluding strings), indicating that it might have nested properties.
                if (propInfo != null && propInfo.PropertyType.IsClass && propInfo.PropertyType != typeof(string))
                {
                    // Create an instance of the nested configuration class.
                    var nestedConfig = Activator.CreateInstance(propInfo.PropertyType);

                    // If the instance creation is successful, apply settings to this nested configuration object.
                    if (nestedConfig != null)
                    {
                        ApplyJsonSettings(nestedConfig, property.Value);

                        // Set the nested configuration object to the property of the parent configuration object.
                        propInfo.SetValue(propertyConfig, nestedConfig);
                    }
                }
                // If the property exists and is not a class (or is a string), set its value directly.
                else if (propInfo != null)
                {
                    // Get the raw JSON text of the property value.
                    var value = property.Value.GetRawText();

                    // Deserialize the JSON text into the appropriate property type.
                    var typedValue = JsonSerializer.Deserialize(value, propInfo.PropertyType);

                    // Set the deserialized value to the property of the configuration object.
                    propInfo.SetValue(propertyConfig, typedValue);
                }
            }
        }

        /// <summary>
        /// Recursively applies settings from an XML element to the corresponding properties of the configuration object.
        /// </summary>
        /// <param name="propertyConfig">The configuration object whose properties are to be set.</param>
        /// <param name="xmlElement">The XML element containing the settings to be applied.</param>
        private static void ApplyXmlSettings(object propertyConfig, XmlElement xmlElement)
        {
            // Iterate over each child node in the XML element.
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                if (node is XmlElement element)
                {
                    // Get the property information of the configuration object that matches the current XML element name.
                    var propInfo = propertyConfig.GetType().GetProperty(element.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    // Check if the property exists and is a class (excluding strings), indicating that it might have nested properties.
                    if (propInfo != null && propInfo.PropertyType.IsClass && propInfo.PropertyType != typeof(string))
                    {
                        // Create an instance of the nested configuration class.
                        var nestedConfig = Activator.CreateInstance(propInfo.PropertyType);

                        // If the instance creation is successful, apply settings to this nested configuration object.
                        if (nestedConfig != null)
                        {
                            ApplyXmlSettings(nestedConfig, element);

                            // Set the nested configuration object to the property of the parent configuration object.
                            propInfo.SetValue(propertyConfig, nestedConfig);
                        }
                    }
                    // If the property exists and is not a class (or is a string), set its value directly.
                    else if (propInfo != null)
                    {
                        propInfo.SetValue(propertyConfig, Convert.ChangeType(element.InnerText, propInfo.PropertyType));
                    }
                }
            }
        }
    }
}
