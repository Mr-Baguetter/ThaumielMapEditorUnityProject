using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Yaml
{
    public class YamlLOD
    {
        public Vector3 Bounds { get; set; }
        public List<PrimitiveType> Primitives { get; set; }
    }
}