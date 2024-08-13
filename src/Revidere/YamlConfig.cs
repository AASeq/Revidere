namespace Revidere;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.RepresentationModel;

/// <summary>
/// Simplified and premissive parsing of yaml file.
/// </summary>
internal sealed class YamlConfig {

    /// <summary>
    /// Returns instance if file is found or null otherwise.
    /// </summary>
    /// <exception cref="FileNotFoundException">No config file found.</exception>
    public static YamlConfig? FromConfigFile() {
        foreach (var filePath in new string[] { "config.yaml", "/config/config.yaml" }) {
            var info = new FileInfo(filePath);
            if (info.Exists) {
                return new YamlConfig(info.OpenRead());
            }
        }
        return null;
    }


    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public YamlConfig(Stream stream) {
        var yaml = new YamlStream();
        yaml.Load(new StreamReader(stream));
        RootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
    }


    private readonly YamlMappingNode RootNode;


    /// <summary>
    /// Returns the logging properties for the specified logging type.
    /// Match is done from 'logging' root level element.
    /// </summary>
    /// <param name="type">Logging type (e.g. console, file, etc.)</param>
    public FrozenDictionary<string, string> GetLoggingProperties(string type) {
        var result = new List<KeyValuePair<string, string>>();
        foreach (var child in RootNode.Children) {
            if (string.Equals(child.Key.ToString(), "logging", StringComparison.OrdinalIgnoreCase) && (child.Value is YamlMappingNode loggingMapping)) {
                foreach (var loggingChild in loggingMapping) {
                    if (loggingChild.Key.ToString() == type) {
                        if (loggingChild.Value is YamlScalarNode) {
                            result.Add(new KeyValuePair<string, string>("level", loggingChild.Value.ToString()));
                            break;
                        } else if (loggingChild.Value is YamlMappingNode logging) {
                            foreach (var prop in logging) {
                                result.Add(new KeyValuePair<string, string>(prop.Key.ToString(), prop.Value.ToString()));
                            }
                            break;
                        }
                    }
                }
            }
        }
        return FrozenDictionary.ToFrozenDictionary(result, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the list of properties for the specified root level element.
    /// Any element that is not a list will be flattened to a single element with an empty key.
    /// </summary>
    /// <param name="path">Root level element path.</param>
    public ReadOnlyCollection<FrozenDictionary<string, string>> GetSequenceProperties(string path) {
        var result = new List<FrozenDictionary<string, string>>();
        foreach (var child in RootNode.Children) {
            if (string.Equals(child.Key.ToString(), path, StringComparison.OrdinalIgnoreCase)) {
                if (child.Value is YamlSequenceNode sequence) {
                    foreach (var element in sequence) {
                        var props = new List<KeyValuePair<string, string>>();
                        if (element is YamlScalarNode) {
                            props.Add(new KeyValuePair<string, string>(string.Empty, element.ToString()));
                        } else if (element is YamlMappingNode mapping) {
                            foreach (var prop in mapping) {
                                props.Add(new KeyValuePair<string, string>(prop.Key.ToString(), prop.Value.ToString()));
                            }
                        }
                        result.Add(FrozenDictionary.ToFrozenDictionary(props, StringComparer.OrdinalIgnoreCase));
                    }
                }
            }
        }
        return result.AsReadOnly();
    }
    /// <summary>
    /// Returns the properties for the specified root level element.
    /// Any element that is not a list will be flattened to a single element with an empty key.
    /// </summary>
    /// <param name="path">Root level element path.</param>
    public FrozenDictionary<string, string> GetProperties(string path) {
        var props = new List<KeyValuePair<string, string>>();
        foreach (var child in RootNode.Children) {
            if (string.Equals(child.Key.ToString(), path, StringComparison.OrdinalIgnoreCase)) {
                if (child.Value is YamlMappingNode mapping) {
                    foreach (var prop in mapping) {
                        props.Add(new KeyValuePair<string, string>(prop.Key.ToString(), prop.Value.ToString()));
                    }
                }
            }
        }
        return FrozenDictionary.ToFrozenDictionary(props, StringComparer.OrdinalIgnoreCase);
    }

}
