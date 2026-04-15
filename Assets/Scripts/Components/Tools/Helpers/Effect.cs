using System;

namespace Assets.Scripts.Components.Tools.Helpers
{
    [Serializable]
    public class GiveEffect
    {
        public int Intensity;
        public float Duration;
        public string EffectName;
    }

    [Serializable]
    public class RemoveEffect
    {
        public int Intensity;
        public string EffectName;
    }
}