using UnityEngine;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Components
{
    public class TargetObject : ObjectBase
    {
        [HideInInspector]
        [field: SerializeField]
        public TargetType TargetType { get; set; }

        public override ObjectType ObjectType => ObjectType.Target;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["TargetType"] = TargetType,
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);
            TargetType = (TargetType)Properties["TargetType"];
        }
    }
}
