using UnityEngine;
using Assets.Scripts.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.Components.Tools
{
    public class ToolBase : MonoBehaviour
    {
        public virtual ToolType ToolType { get; }

        public virtual Dictionary<string, object> Properties { get; set; } = new();

        public virtual void Compile()
        {

        }

        public virtual void Decompile()
        {
            
        }
    }
}