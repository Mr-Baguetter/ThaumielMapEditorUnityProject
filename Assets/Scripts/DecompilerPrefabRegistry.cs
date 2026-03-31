using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "BuilderPrefabRegistry", menuName = "Thaumiel/Prefab Registry")]
    public class BuilderPrefabRegistry : ScriptableObject
    {
        [Header("Primitives")]
        public GameObject SpherePrefab;
        public GameObject CubePrefab;
        public GameObject CylinderPrefab;
        public GameObject CapsulePrefab;
        public GameObject PlanePrefab;
        public GameObject QuadPrefab;

        [Header("Doors")]
        public GameObject LczDoorPrefab;
        public GameObject HczDoorPrefab;
        public GameObject EzDoorPrefab;
        public GameObject GateDoorPrefab;
        public GameObject BulkHeadDoorPrefab;

        [Header("Cameras")]
        public GameObject LczCameraPrefab;
        public GameObject HczCameraPrefab;
        public GameObject EzCameraPrefab;
        public GameObject EzArmCameraPrefab;
        public GameObject SzCameraPrefab;

        [Header("Clutter")]
        public GameObject SimpleBoxesPrefab;
        public GameObject PipesShortPrefab;
        public GameObject BoxesLadderPrefab;
        public GameObject TankSupportedShelfPrefab;
        public GameObject AngledFencesPrefab;
        public GameObject HugeOrangePipesPrefab;
        public GameObject PipesLongOpenPrefab;
        public GameObject BrokenElectricalBoxPrefab;

        [Header("Lockers")]
        public GameObject PedestalPrefab;
        public GameObject LargeGunPrefab;
        public GameObject RifleRackPrefab;
        public GameObject MiscLockerPrefab;
        public GameObject MedkitPrefab;
        public GameObject AdrenalinePrefab;
        public GameObject ExperimentalWeaponPrefab;

        [Header("Targets")]
        public GameObject BinaryTargetPrefab;
        public GameObject ClassDTargetPrefab;
        public GameObject SportTargetPrefab;

        [Header("Other")]
        public GameObject TextToyPrefab;
        public GameObject CapybaraPrefab;
        public GameObject LightPrefab;
        public GameObject WorkstationPrefab;
        public GameObject InteractablePrefab;
        public GameObject WaypointPrefab;
        public GameObject PickupPrefab;
        public GameObject TeleporterPrefab;
    }
}