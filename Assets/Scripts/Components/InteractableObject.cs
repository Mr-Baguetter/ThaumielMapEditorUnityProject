using UnityEngine;
using Assets.Scripts.Enums;

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

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Shape"] = Shape,
                ["Duration"] = Duration,
                ["Locked"] = Locked,
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
    }
}