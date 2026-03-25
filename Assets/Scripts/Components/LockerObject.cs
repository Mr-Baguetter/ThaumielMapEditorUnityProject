using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Lockers;
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

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["LockerType"] = LockerType,
                ["Chambers"] = Chambers
            };
        }
    }
}