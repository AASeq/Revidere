namespace Revidere;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

/// <summary>
/// Simplified and premissive parsing of yaml file.
/// </summary>
internal class YamlConfig {

    public YamlConfig() {
        foreach (var filePath in new string[] { "config.yaml", "/config/config.yaml" }) {
            var info = new FileInfo(filePath);
            if (info.Exists) {
                var yaml = new YamlStream();
                yaml.Load(new StreamReader(info.OpenRead()));
                RootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
                break;
            }
        }
        if (RootNode == null) throw new FileNotFoundException("No config file found.");
    }

    public YamlConfig(Stream stream) {
        var yaml = new YamlStream();
        yaml.Load(new StreamReader(stream));
        RootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
    }

    private readonly YamlMappingNode RootNode;

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

}
