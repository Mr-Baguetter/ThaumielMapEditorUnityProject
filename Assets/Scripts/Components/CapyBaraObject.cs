using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CapyBaraObject : ObjectBase
    {
        [field: SerializeField]
        public bool Collisions { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Collisions"] = Collisions,
            };
        }
    }
}