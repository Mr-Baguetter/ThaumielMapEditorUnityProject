using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PickupObject : ObjectBase
    {
        [Header("Pickup Settings")]
        [Tooltip("The item that will be spawned.")]
        public ItemType ItemToSpawn;

        [Tooltip("The chance for this pickup to spawn, from 0 to 100.")]
        public float SpawnPercentage;

        [Tooltip("The maximum number of items that can spawn.")]
        public uint MaxAmount;

        [Tooltip("If enabled, this pickup will spawn infinitely.")]
        public bool IsInfinite;

        public override ObjectType ObjectType => ObjectType.Pickup;

        public override void Compile(Transform root)
        {
            base.Compile(root);

            Properties = new()
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