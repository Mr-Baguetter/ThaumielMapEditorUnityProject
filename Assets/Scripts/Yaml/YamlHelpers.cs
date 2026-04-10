using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Lockers;
using UnityEngine;

namespace Assets.Scripts.Yaml
{
    public class YamlHelpers
    {
        public static Vector2 ParseVector2(object value)
        {
            if (value is Vector2 v)
                return v;

            if (value is Dictionary<string, object> dict)
            {
                float x = dict.TryGetValue("x", out object xVal) ? Convert.ToSingle(xVal) : 0f;
                float y = dict.TryGetValue("y", out object yVal) ? Convert.ToSingle(yVal) : 0f;
                return new Vector2(x, y);
            }

            return default;
        }

        public static Vector3 ParseVector3(object value)
        {
            if (value is Vector3 v)
                return v;

            if (value is Dictionary<string, object> dict)
            {
                float x = dict.TryGetValue("x", out object xVal) ? Convert.ToSingle(xVal) : 0f;
                float y = dict.TryGetValue("y", out object yVal) ? Convert.ToSingle(yVal) : 0f;
                float z = dict.TryGetValue("z", out object zVal) ? Convert.ToSingle(zVal) : 0f;
                return new Vector3(x, y, z);
            }

            return default;
        }

        public static Color ParseColor(object value)
        {
            if (value is Color c)
                return c;

            if (value is Dictionary<object, object> dict)
            {
                float r = dict.TryGetValue("r", out object rVal) ? Convert.ToSingle(rVal) : 0f;
                float g = dict.TryGetValue("g", out object gVal) ? Convert.ToSingle(gVal) : 0f;
                float b = dict.TryGetValue("b", out object bVal) ? Convert.ToSingle(bVal) : 1f;
                float a = dict.TryGetValue("a", out object aVal) ? Convert.ToSingle(aVal) : 1f;

                return new Color(r, g, b, a);
            }

            return Color.white;
        }

        public static LockerChamber ParseChamber(object value)
        {
            if (value is not Dictionary<string, object> dict)
                return new LockerChamber();

            LockerChamber chamber = new()
            {
                Index = (uint)(dict.TryGetValue("Index", out object index) ? Convert.ToInt32(index) : default),
                Permissions = dict.TryGetValue("Permissions", out object permissions) ? (DoorPermissionFlags)Enum.Parse(typeof(DoorPermissionFlags), permissions.ToString()) : default,
                Data = dict.TryGetValue("Data", out object data) && data is List<object> dataList ? dataList.Select(d => ParseLockerData(d)).ToList() : new List<ChamberData>()
            };

            return chamber;
        }

        public static ChamberData ParseLockerData(object value)
        {
            if (value is not Dictionary<string, object> dict)
                return new ChamberData();

            return new ChamberData()
            {
                ItemType = dict.TryGetValue("ItemType", out object itemType) ? (ItemType)Enum.Parse(typeof(ItemType), itemType.ToString()) : default,
                SpawnPercent = dict.TryGetValue("SpawnPercent", out object spawnPercent) ? Convert.ToSingle(spawnPercent) : default,
                AmountToSpawn = dict.TryGetValue("AmountToSpawn", out object amountToSpawn) ? Convert.ToInt32(amountToSpawn) : default
            };
        }

        public static List<T> ParseList<T>(object value, Func<object, T> parser)
        {
            if (value is not List<object> list)
                return new List<T>();

            return list.Select(parser).ToList();
        }

        public static List<T> ParseList<T>(object value)
        {
            if (value is not List<object> list)
                return new List<T>();

            return list.OfType<T>().ToList();
        }


        public static T ParseEnum<T>(object value) where T : struct, Enum
        {
            if (value is T direct)
                return direct;

            if (Enum.TryParse(value.ToString(), ignoreCase: true, out T result))
                return result;

            return default;
        }

        public static List<T> ParseEnumList<T>(object value) where T : struct, Enum
            => ParseList(value, ParseEnum<T>);
    }
}