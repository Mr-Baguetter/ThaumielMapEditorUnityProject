using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PrimitiveObject : ObjectBase
    {
        [field: SerializeField]
        public Color Color { get; set; }

        [field: SerializeField]
        public PrimitiveType PrimitiveType { get; set; }

        [field: SerializeField]
        public PrimitiveFlags PrimitiveFlags { get; set; }

        private void OnValidate()
        {
            ApplyColor();
        }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Color"] = Color,
                ["PrimitiveType"] = PrimitiveType,
                ["PrimitiveFlags"] = (PrimitiveFlags & (PrimitiveFlags.Collidable | PrimitiveFlags.Visible)).ToString()
            };
        }

        private void ApplyColor()
        {
            if (!TryGetComponent<MeshRenderer>(out var renderer))
                return;

            renderer.material.color = Color;
        }
    }
}