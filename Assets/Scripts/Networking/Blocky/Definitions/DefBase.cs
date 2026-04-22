using System;
using System.Linq;

namespace Assets.Scripts.Networking.Blocky.Definitions
{
    public abstract class DefBase
    {
        public abstract void Register();

        public static (string label, string value)[] EnumOptions<T>() where T : struct, Enum
            => Enum.GetNames(typeof(T)).Select(n => (n, n)).ToArray();
    }
}
