using System;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ClutterObject : ObjectBase
    {
        [HideInInspector]
        [field: SerializeField]
        public ClutterType ClutterType { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["ClutterType"] = ClutterType,
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            ClutterType = Properties.TryGetValue("ClutterType", out object clutterType) ? (ClutterType)Enum.Parse(typeof(ClutterType), clutterType.ToString()) : default;
        }
    }
}