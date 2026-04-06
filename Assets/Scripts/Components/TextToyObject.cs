using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshPro))]
    public class TextToyObject : ObjectBase
    {
        public Vector2 DisplaySize { get; set; }

        [field: SerializeField]
        public string Text { get; set; }

        public override ObjectType ObjectType => ObjectType.TextToy;

        private TMP_Text _textMesh;

        private void Awake()
        {
            TryGetComponent(out _textMesh);
        }

        private void OnValidate()
        {
            RefreshText();

            if (_textMesh != null)
            {
                DisplaySize = _textMesh.rectTransform.sizeDelta;
                _textMesh.margin = Vector4.zero;
            }
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

            DisplaySize = Properties.TryGetValue("DisplaySize", out object displaySize) ? YamlHelpers.ParseVector2(displaySize) : default;
            Text = Properties.TryGetValue("Text", out object text) ? Convert.ToString(text) : string.Empty;

            RefreshText();
        }
    }
}