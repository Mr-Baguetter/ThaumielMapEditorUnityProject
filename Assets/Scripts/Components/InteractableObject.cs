using UnityEngine;
using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.Components
{
    public class InteractableObject : ObjectBase
    {
        [field: SerializeField]
        public ColliderShape Shape { get; set; }

        [field: SerializeField]
        public float Duration { get; set; }

        [field: SerializeField]
        public bool Locked { get; set; }

        [field: SerializeField]
        public DoorPermissionFlags Permissions { get; set; }

        public override ObjectType ObjectType => ObjectType.Interactable;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Shape"] = Shape,
                ["Duration"] = Duration,
                ["Locked"] = Locked,
                ["Permissions"] = Permissions
            };
        }

        private void OnValidate()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            switch (Shape)
            {
                case ColliderShape.Box:
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                    break;

                case ColliderShape.Sphere:
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                    break;

                case ColliderShape.Capsule:
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
                    break;
            }
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Shape = Properties.TryGetValue("Shape", out object shape) ? (ColliderShape)Enum.Parse(typeof(ColliderShape), shape.ToString()) : default;
            Duration = Properties.TryGetValue("Duration", out object duration) ? (float)duration : default;
            Locked = Properties.TryGetValue("Locked", out object locked) && (bool)locked;
            Permissions = Properties.TryGetValue("Permissions", out object perms) ? (DoorPermissionFlags)Enum.Parse(typeof(DoorPermissionFlags), perms.ToString()) : default;
        }
    }
}