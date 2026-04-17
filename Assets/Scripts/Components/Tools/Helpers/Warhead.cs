using System;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Components.Tools.Helpers
{
    [Serializable]
    public class Warhead
    {
        public WarheadAction Action { get; set; }
        public bool SuppressSubtitles { get; set; }
    }
}