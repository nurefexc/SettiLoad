# SettiLoad

SettiLoad is a .NET library designed for loading configurations into C# objects. Currently, it supports loading configuration data from JSON files. Future versions of SettiLoad will aim to support additional formats, such as XML and YAML.

## Features

- **JSON Configuration Loading:** Load configurations from JSON files directly into C# objects.
- **Recursive Property Setting:** Supports nested configuration objects by recursively applying settings from JSON.
- **Flexible Error Handling:** Optionally throws exceptions on load failures or handles them gracefully.

## Usage

To use SettiLoad, follow these steps:

### 1. Create a Configuration Class:

Define a class that inherits from the Config base class. The properties of this class should match the structure of the JSON configuration.

```csharp

public class MyConfig : Config
{
    public string Setting1 { get; set; }
    public NestedConfig Setting2 { get; set; }
    
    public class NestedConfig
    {
        public int SubSetting1 { get; set; }
    }
}

```

### 2. Load Configuration:

Use the Load method to load the configuration from a JSON file into your configuration class.

```csharp

var config = new MyConfig();
bool success = config.Load("path/to/config.json");

if (success)
{
    // Use the loaded configuration
    Console.WriteLine(config.Setting1);
}
else
    // Handle the load failure
}
```

### 3. JSON File Structure:

Ensure that your JSON file matches the structure of your configuration classes. For the above example, a valid JSON file might look like this:

```json
{
    "Setting1": "Value1",
    "Setting2": {
        "SubSetting1": 123
    }
}
```

## Methods
```csharp
bool Load(string filePath, bool throwException=false)
```
Loads the configuration from the specified JSON file.

**Parameters:**
* filePath: The path to the JSON configuration file.
* throwException: Indicates whether to throw an exception if loading fails.

**Returns:**
* true if the configuration was successfully loaded; otherwise, false.

## Error Handling

If loading the configuration fails and throwException is set to true, an exception will be thrown. If throwException is false, the method will return false to indicate a failure without throwing an exception.

## License

This project is licensed under the MIT License - see the [MIT](LICENSE) file for details.

Happy coding!
