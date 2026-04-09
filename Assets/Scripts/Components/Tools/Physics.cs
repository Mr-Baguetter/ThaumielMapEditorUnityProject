using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using System;
using UnityEngine;

namespace Assets.Scripts.Components.Tools
{
    public class Physics : ToolBase
    {
        public override ToolType ToolType => ToolType.Physics;

        public float Weight;

        public float Drag;

        public float AngularDrag;

        public CollisionDetectionMode CollisionMode;

        public bool Enabled;

        public override void Compile()
        {
            base.Compile();
            base.Properties = new()
            {
                ["Weight"] = Weight,
                ["Enabled"] = Enabled,
                ["Drag"] = Drag,
                ["AngularDrag"] = AngularDrag,
                ["CollisionMode"] = CollisionMode,
            };
        }

        public override void Decompile()
        {
            Weight = Properties.TryGetValue("Weight", out object weightobj) ? Convert.ToSingle(weightobj) : 10f;
            Enabled = Properties.TryGetValue("Enabled", out object enablobj) ? Convert.ToBoolean(enablobj) : default;
            Drag = Properties.TryGetValue("Drag", out object dragobj) ? Convert.ToSingle(dragobj) : 5f;
            AngularDrag = Properties.TryGetValue("AngularDrag", out object angularDragobj) ? Convert.ToSingle(angularDragobj) : 5f;
            CollisionMode = Properties.TryGetValue("CollisionMode", out object collobj) ? YamlHelpers.ParseEnum<CollisionDetectionMode>(collobj) : default;
        }
    }
}