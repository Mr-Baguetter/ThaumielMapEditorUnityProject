using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;
using CameraType = Assets.Scripts.Enums.CameraType;

namespace Assets.Scripts.Components
{
    public class CameraObject : ObjectBase
    {
        [field: SerializeField]
        [HideInInspector]
        public CameraType CameraType { get; set; }

        [field: SerializeField]
        public string Label { get; set; }

        [field: SerializeField]
        public RoomName Room { get; set; }

        [field: SerializeField]
        public Vector2 VerticalConstraint { get; set; }

        [field: SerializeField]
        public Vector2 HorizontalConstraint { get; set; }

        [field: SerializeField]
        public Vector2 ZoomConstraint { get; set; }

        public override ObjectType ObjectType => ObjectType.Camera;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["CameraType"] = CameraType,
                ["Label"] = Label,
                ["Room"] = Room,
                ["VerticalConstraint"] = VerticalConstraint,
                ["HorizontalConstraint"] = HorizontalConstraint,
                ["ZoomConstraint"] = ZoomConstraint,
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            CameraType = Properties.TryGetValue("CameraType", out object cameraType) ? YamlHelpers.ParseEnum<CameraType>(cameraType) : default;
            Label = Properties.TryGetValue("Label", out object label) ? Convert.ToString(label) : string.Empty;
            Room = Properties.TryGetValue("Room", out object room) ? YamlHelpers.ParseEnum<RoomName>(room) : default;
            VerticalConstraint = Properties.TryGetValue("VerticalConstraint", out object verticalConstraint) ? YamlHelpers.ParseVector2(verticalConstraint) : default;
            HorizontalConstraint = Properties.TryGetValue("HorizontalConstraint", out object horizontalConstraint) ? YamlHelpers.ParseVector2(horizontalConstraint) : default;
            ZoomConstraint = Properties.TryGetValue("ZoomConstraint", out object zoomConstraint) ? YamlHelpers.ParseVector2(zoomConstraint) : default;
        }
    }
}