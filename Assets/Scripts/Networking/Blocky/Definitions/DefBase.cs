using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Networking.Blocky.Definitions
{
    public abstract class DefBase
    {
        public abstract void Register();

        public static (string label, string value)[] EnumOptions<T>() where T : struct, Enum
        {
            if (typeof(T).GetCustomAttribute<FlagsAttribute>() != null)
            {
                List<T> individualFlags = Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Where(v =>
                    {
                        ulong numeric = Convert.ToUInt64(v);
                        return numeric != 0 && (numeric & (numeric - 1)) == 0;
                    })
                    .ToList();

                List<(string label, string value)> combinations = new();

                int totalCombinations = 1 << individualFlags.Count;
                for (int i = 1; i < totalCombinations; i++)
                {
                    List<string> selected = individualFlags.Where((_, idx) => (i & (1 << idx)) != 0).Select(v => v.ToString()).ToList();
                    string label = string.Join(" | ", selected);
                    combinations.Add((label, label));
                }

                return combinations.ToArray();
            }

            return Enum.GetNames(typeof(T)).Select(n => (n, n)).ToArray();
        }
    }
}
