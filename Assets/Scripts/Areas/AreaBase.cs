using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Areas
{
    public class AreaBase : MonoBehaviour
    {
        public int ObjectId { get; set; }

        public int ParentId { get; set; }

        public virtual void Compile(Transform root)
        {
            
        }

        public virtual void Decompile(Transform root)
        {
            
        }

        public AreaType Type { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}