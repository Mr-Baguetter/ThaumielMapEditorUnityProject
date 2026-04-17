using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PlayerSpawnPointObject : ObjectBase
    {
        [Tooltip("The roles that are allowed to spawn here.")]
        public List<RoleTypeId> AllowedRoles;

        [Tooltip("The chance for this to be selected.")]
        [Range(0, 100)]
        public float Chance;

        [Tooltip("Flags that disable this spawn point.")]
        public DisableFlags Disable { get; set; }

        public override ObjectType ObjectType => ObjectType.PlayerSpawnPoint;

        public override void Compile(Transform root)
        {
            base.Compile(root);
            Properties = new()
            {
                ["AllowedRoles"] = AllowedRoles,
                ["Chance"] = Chance,
                ["DisableFlags"] = Disable
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            AllowedRoles = Properties.TryGetValue("AllowedRoles", out var roles) ? YamlHelpers.ParseEnumList<RoleTypeId>(roles) : new();
            Chance = Properties.TryGetValue("Chance", out var chance) ? Convert.ToSingle(chance) : 50f;
            Disable = Properties.TryGetValue("DisableFlags", out var disable) ? YamlHelpers.ParseEnum<DisableFlags>(disable) : default;
        }
    }
}