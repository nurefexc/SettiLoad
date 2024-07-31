using System.Reflection;
using System.Text.Json;

namespace SettiLoad;

/// <summary>
/// An abstract base class for creating configurations that can be loaded from a configuration file.
/// </summary>
public abstract class Config
{
    /// <summary>
    /// Loads the configuration from the specified file.
    /// </summary>
    /// <param name="filePath">The path to the configuration file.</param>
    /// <param name="throwException">Indicates whether to throw an exception if loading fails.</param>
    /// <returns>Returns true if the configuration was successfully loaded; otherwise, returns false.</returns>
    public bool Load(string filePath, bool throwException=false)
    {
        try
        {
            // Read the entire text content from the specified JSON file.
            var json = File.ReadAllText(filePath);

            // Parse the JSON text into a JsonDocument.
            var jsonDoc = JsonDocument.Parse(json);

            // Apply settings from the JSON document to this configuration object.
            ApplySettings(this, jsonDoc.RootElement);
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

        // Return true to indicate that the configuration was successfully loaded.
        return true;
    }

    /// <summary>
    /// Recursively applies settings from a JSON element to the corresponding properties of the configuration object.
    /// </summary>
    /// <param name="propertyConfig">The configuration object whose properties are to be set.</param>
    /// <param name="jsonElement">The JSON element containing the settings to be applied.</param>
    private static void ApplySettings(object propertyConfig, JsonElement jsonElement)
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
                    ApplySettings(nestedConfig, property.Value);

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
}
