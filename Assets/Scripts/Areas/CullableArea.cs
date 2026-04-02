using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Areas
{
    public class CullableArea : AreaBase
    {
        [field: SerializeField]
        public Vector3 Bounds { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Bounds"] = transform.localScale,
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Bounds = Properties.TryGetValue("Bounds", out object bounds) ? YamlHelpers.ParseVector3(bounds) : default;
        }
    }
}