using System;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DoorObject : ObjectBase
    {
        [field: SerializeField]
        [HideInInspector]
        public DoorType DoorType { get; set; }

        [field: SerializeField]
        public DoorPermissionFlags Permissions { get; set; }
        
        [field: SerializeField]
        public bool RequireAllPermissions { get; set; }

        [field: SerializeField]
        public bool Bypass2176 { get; set; }

        [field: SerializeField]
        public float MaxHealth { get; set; }

        [field: SerializeField]
        public float Health { get; set; }

        [field: SerializeField]
        public bool IsOpen { get; set; }

        [field: SerializeField]
        public bool IsLocked { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["DoorType"] = DoorType,
                ["Permissions"] = Permissions,
                ["RequireAllPermissions"] = RequireAllPermissions,
                ["Bypass2176"] = Bypass2176,
                ["MaxHealth"] = MaxHealth,
                ["Health"] = Health,
                ["IsOpen"] = IsOpen,
                ["IsLocked"] = IsLocked,
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            DoorType = Properties.TryGetValue("DoorType", out object doorType) ? (DoorType)Enum.Parse(typeof(DoorType), doorType.ToString()) : default;
            Permissions = Properties.TryGetValue("Permissions", out object permissions) ? (DoorPermissionFlags)Enum.Parse(typeof(DoorPermissionFlags), permissions.ToString()) : default;
            RequireAllPermissions = Properties.TryGetValue("RequireAllPermissions", out object requireAllPermissions) && (bool)requireAllPermissions;
            Bypass2176 = Properties.TryGetValue("Bypass2176", out object bypass2176) && (bool)bypass2176;
            MaxHealth = Properties.TryGetValue("MaxHealth", out object maxHealth) ? (float)maxHealth : default;
            Health = Properties.TryGetValue("Health", out object health) ? (float)health : default;
            IsOpen = Properties.TryGetValue("IsOpen", out object isOpen) && (bool)isOpen;
            IsLocked = Properties.TryGetValue("IsLocked", out object isLocked) && (bool)isLocked;
        }
    }
}