using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class WaypointObject : ObjectBase
    {
        [field: SerializeField]
        public float Priority { get; set; }

        [field: SerializeField]
        public bool VisualizeBounds { get; set; }

        public override ObjectType ObjectType => ObjectType.Waypoint;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Priority"] = Priority,
                ["VisualizeBounds"] = VisualizeBounds,
                ["BoundsSize"] = transform.localScale
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Priority = Properties.TryGetValue("Priority", out object priority) ? Convert.ToSingle(priority) : default;
            VisualizeBounds = Properties.TryGetValue("VisualizeBounds", out object visualizeBounds) && Convert.ToBoolean(visualizeBounds);
            transform.localScale = Properties.TryGetValue("BoundsSize", out object boundsSize) ? YamlHelpers.ParseVector3(boundsSize) : default;
        }
    }
}