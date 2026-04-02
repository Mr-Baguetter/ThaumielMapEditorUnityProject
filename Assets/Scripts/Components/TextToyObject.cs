using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(TextMeshPro))]
    public class TextToyObject : ObjectBase
    {
        [field: SerializeField]
        public Vector2 DisplaySize { get; set; }

        [field: SerializeField]
        public string Text { get; set; }

        public override ObjectType ObjectType => ObjectType.TextToy;

        private TMP_Text _textMesh;
        private MeshRenderer _renderer;

        private void Awake()
        {
            TryGetComponent(out _textMesh);
            TryGetComponent(out _renderer);
        }

        private void OnValidate()
        {
            _textMesh.margin = Vector4.zero;
            _renderer.hideFlags = HideFlags.HideInInspector;

            RefreshText();
        }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["DisplaySize"] = DisplaySize,
                ["Text"] = Text
            };
        }

        private void RefreshText()
        {
            if (_textMesh == null || string.IsNullOrEmpty(Text))
                return;

            _textMesh.SetText(Text);
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            DisplaySize = Properties.TryGetValue("DisplaySize", out object displaySize) ? (Vector2)displaySize : default;
            Text = Properties.TryGetValue("Text", out object text) ? Convert.ToString(text) : string.Empty;

            RefreshText();
        }
    }
}