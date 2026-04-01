using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Lockers;
using Assets.Scripts.Yaml;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class LockerObject : ObjectBase
    {
        [field: SerializeField]
        [HideInInspector]
        public LockerType LockerType { get; set; }

        [field: SerializeField]
        public List<LockerChamber> Chambers { get; set; }

        public override ObjectType ObjectType => ObjectType.Locker;

        private void OnValidate()
        {
            if (Chambers == null)
                return;

            for (int i = 0; i < Chambers.Count; i++)
            {
                if (Chambers[i] == null)
                    continue;

                Chambers[i].Index = (uint)i;
            }
        }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["LockerType"] = LockerType,
                ["Chambers"] = Chambers
            };
        }

        public override void Decompile(Transform root)
        {
            base.Decompile(root);

            LockerType = Properties.TryGetValue("LockerType", out object lockerType) ? (LockerType)Enum.Parse(typeof(LockerType), lockerType.ToString()) : default;
            Chambers = Properties.TryGetValue("Chambers", out object chambers) && chambers is List<object> chamberList ? chamberList.Select(c => YamlHelpers.ParseChamber(c)).ToList() : new List<LockerChamber>();
        }
    }
}