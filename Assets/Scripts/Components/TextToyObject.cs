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
        public string TextFormat { get; set; }

        [field: SerializeField]
        public List<string> Arguments { get; set; } = new();

        public override ObjectType ObjectType => ObjectType.TextToy;

        private TextMeshPro _textMesh;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();
            _rectTransform = _textMesh.GetComponent<RectTransform>();
        }

        private void OnValidate()
        {
            if (_textMesh == null)
                _textMesh = GetComponent<TextMeshPro>();

            if (_rectTransform == null && _textMesh != null)
                _rectTransform = _textMesh.GetComponent<RectTransform>();

            ApplyDisplaySize();
            RefreshText();
        }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["DisplaySize"] = DisplaySize,
                ["TextFormat"] = TextFormat,
                ["Arguments"] = Arguments,
            };
        }

        private void ApplyDisplaySize()
        {
            if (_rectTransform == null)
                return;

            _rectTransform.sizeDelta = DisplaySize;
        }

        private void RefreshText()
        {
            if (_textMesh == null || string.IsNullOrEmpty(TextFormat))
                return;

            if (Arguments != null && Arguments.Count > 0)
            {
                try
                {
                    _textMesh.SetText(string.Format(TextFormat, Arguments.ToArray()));
                }
                catch (System.FormatException ex)
                {
                    _textMesh.SetText(TextFormat);
                    Debug.LogWarning($"[TextToyObject] Failed to format text on '{gameObject.name}': {ex.Message}");
                }
            }
            else
                _textMesh.SetText(TextFormat);
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            DisplaySize = Properties.TryGetValue("DisplaySize", out object displaySize) ? (Vector2)displaySize : default;
            TextFormat = Properties.TryGetValue("TextFormat", out object textFormat) ? textFormat.ToString() : string.Empty;
            Arguments = Properties.TryGetValue("Arguments", out object arguments) && arguments is List<string> argsList ? argsList : new List<string>();

            ApplyDisplaySize();
            RefreshText();
        }
    }
}