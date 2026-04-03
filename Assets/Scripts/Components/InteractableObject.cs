using UnityEngine;
using Assets.Scripts.Enums;
using System;
using Assets.Scripts.Yaml;

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

            Shape = Properties.TryGetValue("Shape", out object shape) ? YamlHelpers.ParseEnum<ColliderShape>(shape) : default;
            Duration = Properties.TryGetValue("Duration", out object duration) ? Convert.ToSingle(duration) : default;
            Locked = Properties.TryGetValue("Locked", out object locked) && Convert.ToBoolean(locked);
            Permissions = Properties.TryGetValue("Permissions", out object perms) ? YamlHelpers.ParseEnum<DoorPermissionFlags>(perms) : default;
        }
    }
}