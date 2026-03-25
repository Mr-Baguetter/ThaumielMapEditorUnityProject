using Assets.Scripts.Enums;
using UnityEngine;
namespace Assets.Scripts.Components
{
    public class PickupObject : ObjectBase
    {
        [field: SerializeField]
        public ItemType ItemToSpawn { get; set; }

        [field: SerializeField]
        public float SpawnPercentage { get; set; }

        [field: SerializeField]
        public uint MaxAmount { get; set; }

        [field: SerializeField]
        public bool IsInfinite { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["ItemToSpawn"] = ItemToSpawn,
                ["SpawnPercentage"] = SpawnPercentage,
                ["MaxAmount"] = MaxAmount,
                ["IsInfinite"] = IsInfinite
            };
        }
    }
}

