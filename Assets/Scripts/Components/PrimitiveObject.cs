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

        public override ObjectType ObjectType => ObjectType.Primitive;

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

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Color = Properties.TryGetValue("Color", out object color) ? YamlHelpers.ParseColor(color) : default;
            PrimitiveType = Properties.TryGetValue("PrimitiveType", out object primitiveType) ? YamlHelpers.ParseEnum<PrimitiveType>(primitiveType) : default;
            PrimitiveFlags = Properties.TryGetValue("PrimitiveFlags", out object primitiveFlags) ? YamlHelpers.ParseEnum<PrimitiveFlags>(primitiveFlags) : default;
        }

        private void Start()
        {
            TryGetComponent(out _filter);
            TryGetComponent(out _renderer);
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");

            if (litShader == null)
            {
                Debug.LogError("Failed to find shader 'Universal Render Pipeline/Lit'.");
                return;
            }

            _sharedRegular = new Material(litShader);
        }

        private void Update()
        {
            _filter.hideFlags = HideFlags.HideInInspector;
            _renderer.hideFlags = HideFlags.HideInInspector;

            _renderer.sharedMaterial = _sharedRegular;
            _renderer.sharedMaterial.color = Color;

            _renderer.enabled = PrimitiveFlags.HasFlag(PrimitiveFlags.Visible);
        }

        private void OnDrawGizmos()
        {
            if (PrimitiveFlags.HasFlag(PrimitiveFlags.Visible) || _filter == null || _filter.sharedMesh == null)
                return;

            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireMesh(_filter.sharedMesh);
        }

        internal MeshFilter _filter;
        private MeshRenderer _renderer;
        private Material _sharedRegular;
    }
}