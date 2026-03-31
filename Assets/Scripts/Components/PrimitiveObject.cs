using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
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

        private Material _materialInstance;
        private MeshRenderer _meshRenderer;

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
            if (_meshRenderer == null && !TryGetComponent(out _meshRenderer))
                return;

            if (_materialInstance == null)
                _materialInstance = _meshRenderer.sharedMaterial;

            _materialInstance.color = Color;
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Color = Properties.TryGetValue("Color", out object color) ? YamlHelpers.ParseColor(color) : default;
            PrimitiveType = Properties.TryGetValue("PrimitiveType", out object primitiveType) ? YamlHelpers.ParseEnum<PrimitiveType>(primitiveType) : default;
            PrimitiveFlags = Properties.TryGetValue("PrimitiveFlags", out object primitiveFlags) ? YamlHelpers.ParseEnum<PrimitiveFlags>(primitiveFlags) : default;

            Debug.Log($"Parsed Color: {Color}");
            ApplyColor();
        }
    }
}