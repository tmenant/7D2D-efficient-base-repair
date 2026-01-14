using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;


public class ModConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : Attribute { }

    private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

    private readonly XmlDocument document;

    public readonly string modName;

    public readonly string modPath;

    public readonly string userDataConfigPath;

    public readonly string modConfigPath;

    public readonly int version;

    public ModConfig(int version = 0, bool save = false)
    {
        var assembly = Assembly.GetCallingAssembly();
        var mod = ModManager.GetModForAssembly(assembly);

        this.modPath = Path.GetFullPath(mod.Path);
        this.modName = mod.Name;
        this.version = version;

        this.userDataConfigPath = $"{GameIO.GetUserGameDataDir()}/{modName}.ModConfig.xml";
        this.modConfigPath = Path.GetFullPath($"{modPath}/ModConfig.xml");

        if (!File.Exists(modConfigPath))
            throw new FileNotFoundException($"Can't find ModConfig.xml for mod '{modName}'");

        if (File.Exists(userDataConfigPath))
            document = ReadXmlDocument(userDataConfigPath);

        if (document is null || GetVersion(document) < version)
        {
            document = ReadXmlDocument(modConfigPath);

            if (File.Exists(userDataConfigPath))
            {
                File.Delete(userDataConfigPath);
            }
        }

        if (save)
        {
            SaveXmlDocument(userDataConfigPath);
        }

        properties = ParseProperties(document);
    }

    public int GetVersion(XmlDocument document)
    {
        if (document.DocumentElement.TryGetAttribute("version", out var version))
        {
            return int.Parse(version);
        }

        return 0;
    }

    public Dictionary<string, string> ParseProperties(XmlDocument document)
    {
        var properties = new Dictionary<string, string>();

        foreach (XmlNode property in document.GetElementsByTagName("property"))
        {
            string name = property.Attributes["name"]?.Value;
            string value = property.Attributes["value"]?.Value;

            if (name != null && value != null)
            {
                properties[name] = value;
            }
        }

        return properties;
    }

    public string GetString(string name)
    {
        if (properties.TryGetValue(name, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException(name);
    }

    public float GetFloat(string name)
    {
        return float.Parse(GetString(name));
    }

    public int GetInt(string name)
    {
        return int.Parse(GetString(name));
    }

    public bool GetBool(string name)
    {
        return bool.Parse(GetString(name));
    }

    public Vector2 GetVector2(string name)
    {
        string[] values = GetString(name).Split(',');

        return new Vector2(
            float.Parse(values[0].Trim()),
            float.Parse(values[1].Trim())
        );
    }

    public Vector3 GetVector3(string name)
    {
        string[] values = GetString(name).Split(',');

        return new Vector3(
            float.Parse(values[0].Trim()),
            float.Parse(values[1].Trim()),
            float.Parse(values[2].Trim())
        );
    }

    public Vector2i GetVector2i(string name)
    {
        string[] values = GetString(name).Split(',');

        return new Vector2i(
            int.Parse(values[0].Trim()),
            int.Parse(values[1].Trim())
        );
    }

    public Vector3i GetVector3i(string name)
    {
        string[] values = GetString(name).Split(',');

        return new Vector3i(
            int.Parse(values[0].Trim()),
            int.Parse(values[1].Trim()),
            int.Parse(values[2].Trim())
        );
    }

    public LoggingLevel GetLoggingLevel(string name)
    {
        var loggingLevel = GetString(name);

        switch (loggingLevel.ToLower())
        {
            case "debug":
                return LoggingLevel.DEBUG;

            case "info":
                return LoggingLevel.INFO;

            case "warning":
                return LoggingLevel.WARNING;

            case "error":
                return LoggingLevel.ERROR;

            case "none":
                return LoggingLevel.NONE;

            default:
                throw new KeyNotFoundException(loggingLevel);
        }
    }

    public T GetEnum<T>(string name, bool ignoreCase = false) where T : struct
    {
        string value = GetString(name);

        if (Enum.TryParse<T>(value, ignoreCase, out var result))
        {
            return result;
        }

        throw new InvalidCastException($"value '{value}' cannot be parsed as a '{typeof(T).Name}'");
    }

    public XmlDocument ReadXmlDocument(string path)
    {
        var xmlDocument = new XmlDocument();

        using (var reader = new StreamReader(path))
        {
            xmlDocument.LoadXml(reader.ReadToEnd());
        }

        Logging.Info($"read '{path}', version={GetVersion(xmlDocument)}");

        return xmlDocument;
    }

    public void SaveXmlDocument(string path)
    {
        document.DocumentElement.SetAttribute("version", version.ToString());

        using (var writer = new StreamWriter(path))
        {
            document.Save(writer);
        }

        Logging.Info($"'{Path.GetFileName(path)}' saved");
    }

    public static void SetField<T>(string fieldName, string fieldValue, bool save = false)
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);

        if (field.IsDefined(typeof(ReadOnlyAttribute), false))
        {
            Logging.Error($"field '{fieldName}' is readOnly");
            return;
        }

        field.SetValue(null, Convert.ChangeType(fieldValue, field.FieldType));

        if (save)
        {
            throw new NotImplementedException();
        }
    }

    public static object GetField<T>(string fieldName = null)
    {
        if (fieldName is null)
        {
            foreach (var f in typeof(T).GetFields())
            {
                Log.Out($"{f.Name} ......... {f.GetValue(null)}");
            }

            return null;
        }

        return typeof(T)
            .GetField(fieldName, BindingFlags.Public | BindingFlags.Static)
            .GetValue(null);
    }

}
