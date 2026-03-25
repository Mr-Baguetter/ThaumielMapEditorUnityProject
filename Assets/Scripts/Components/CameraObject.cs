using Assets.Scripts.Enums;
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
    }
}