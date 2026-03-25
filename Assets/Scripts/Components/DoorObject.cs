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
    }
}