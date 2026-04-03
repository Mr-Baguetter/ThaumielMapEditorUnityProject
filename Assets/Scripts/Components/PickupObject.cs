using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
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

        public override ObjectType ObjectType => ObjectType.Pickup;

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

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            ItemToSpawn = Properties.TryGetValue("ItemToSpawn", out object itemToSpawn) ? YamlHelpers.ParseEnum<ItemType>(itemToSpawn) : default;
            SpawnPercentage = Properties.TryGetValue("SpawnPercentage", out object spawnPercentage) ? Convert.ToSingle(spawnPercentage) : default;
            MaxAmount = Properties.TryGetValue("MaxAmount", out object maxAmount) ? Convert.ToUInt32(maxAmount) : default;
            IsInfinite = Properties.TryGetValue("IsInfinite", out object isInfinite) && Convert.ToBoolean(isInfinite);
        }
    }
}

