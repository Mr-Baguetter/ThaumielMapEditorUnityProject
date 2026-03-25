using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Lockers
{
    [Serializable]
    public class LockerChamber
    {
        [HideInInspector]
        public uint Index;
        public DoorPermissionFlags Permissions;
        public List<ChamberData> Data = new();
    }

    [Serializable]
    public class ChamberData
    {
        public ItemType ItemType;

        [Range(0f, 100f)]
        public float SpawnPercent;


        public int AmountToSpawn;
    }
}