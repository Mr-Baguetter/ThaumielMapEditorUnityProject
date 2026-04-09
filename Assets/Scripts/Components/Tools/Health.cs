using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;

namespace Assets.Scripts.Components.Tools
{
    public class Health : ToolBase
    {
        public enum DestroyState
        {
            Animate,
            ApplyPhysics,
            Destroy
        }

        public override ToolType ToolType => ToolType.Health;

        public DestroyState State;

        public float MaxHealth;

        public float DespawnTime;

        public List<DamageType> AllowedDamage;

        public string AnimationName;

        public override void Compile()
        {
            base.Compile();
            base.Properties = new()
            {
                ["State"] = State,
                ["MaxHealth"] = MaxHealth,
                ["AllowedDamage"] = AllowedDamage,
                ["Despawn"] = DespawnTime,
                ["StateName"] = AnimationName,
            };
        }

        public override void Decompile()
        {
            State = Properties.TryGetValue("State", out object stateobj) ? YamlHelpers.ParseEnum<DestroyState>(stateobj) : default;
            MaxHealth = Properties.TryGetValue("MaxHealth", out object maxobj) ? Convert.ToSingle(maxobj) : 100f;
            AllowedDamage = Properties.TryGetValue("AllowedDamage", out object allowedobj) ? YamlHelpers.ParseEnumList<DamageType>(allowedobj) : default;
            DespawnTime = Properties.TryGetValue("Despawn", out object despawnobj) ? Convert.ToSingle(despawnobj) : 5f;
            AnimationName = Properties.TryGetValue("StateName", out object animobj) ? Convert.ToString(animobj) : string.Empty;
        }
    }
}