using System.Collections.Generic;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Yaml
{
    public class YamlSchematic
    {
        public int RootObjectId { get; set; }
        public string FileName { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public bool ContainsAnimator { get; set; }
        public List<YamlCustomObject> Objects { get; set; } = new();
        public List<YamlArea> Areas { get; set; } = new();
    }
}