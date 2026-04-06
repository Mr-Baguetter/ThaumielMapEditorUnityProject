using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;
using UnityEngine.Rendering;

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

        public override ObjectType ObjectType => ObjectType.Primitive;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MaterialPropertyBlock _propertyBlock;

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

            _propertyBlock ??= new MaterialPropertyBlock();

            _meshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void Start()
        {
            TryGetComponent(out _meshFilter);
            TryGetComponent(out _meshRenderer);
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


        private void OnDrawGizmos()
        {
            if (PrimitiveFlags.HasFlag(PrimitiveFlags.Visible))
                return;

            if (_meshFilter == null)
                TryGetComponent(out _meshFilter);

            if (_meshFilter == null || _meshFilter.sharedMesh == null)
                return;

            Gizmos.color = new Color(Color.r, Color.g, Color.b, 1f);
            Gizmos.DrawWireMesh(_meshFilter.sharedMesh, 0, transform.position, transform.rotation, transform.localScale);
        }
    }
}