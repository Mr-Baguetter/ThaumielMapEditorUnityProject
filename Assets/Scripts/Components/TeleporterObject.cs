using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class TeleporterObject : ObjectBase
    {
        public Guid Id { get; set; }

        [field: SerializeField]
        public TeleporterObject Target { get; set; }

        [field: SerializeField]
        public float CoolDown { get; set; }

        [field: SerializeField]
        public List<RoleTypeId> AllowedRoles { get; set; } = new();

        [field: SerializeField]
        public bool PerPlayerCooldown { get; set; }

        [field: SerializeField]
        public TeleporterFlags Flags { get; set; }

        public override ObjectType ObjectType => ObjectType.Teleporter;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Id"] = Id,
                ["Target"] = Target.Id,
                ["CoolDown"] = CoolDown,
                ["AllowedRoles"] = AllowedRoles,
                ["PerPlayerCooldown"] = PerPlayerCooldown,
                ["Flags"] = Flags
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            Id = Properties.TryGetValue("Id", out object id) ? (Guid)id : Guid.NewGuid();
            CoolDown = Properties.TryGetValue("CoolDown", out object cooldown) ? (float)cooldown : default;
            AllowedRoles = Properties.TryGetValue("AllowedRoles", out object allowed) ? (List<RoleTypeId>)allowed : default;
            PerPlayerCooldown = Properties.TryGetValue("PerPlayerCooldown", out object perplayer) && (bool)perplayer;
            Flags = Properties.TryGetValue("Flags", out object teleporterFlags) ? YamlHelpers.ParseEnum<TeleporterFlags>(teleporterFlags) : default;
        }

        private void Start()
        {
            Id = Guid.NewGuid();
        }
    }
}