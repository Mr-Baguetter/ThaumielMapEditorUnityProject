using System.Collections.Generic;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Yaml
{
    public class YamlArea
    {
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public string SchematicName { get; set; } = string.Empty;
        public AreaType AreaType { get; set; }
        public Dictionary<string, object> Values { get; set; } = new();
    }
}