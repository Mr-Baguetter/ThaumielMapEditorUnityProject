using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Components.Tools.Helpers
{
    [Serializable]
    public class SendCassieMessage
    {
        public string Message { get; set; } = string.Empty;
        public string CustomSubtitles { get; set; } = string.Empty;
        public bool PlayBackground { get; set; }
        public float Priority { get; set; }
        public float GlitchScale { get; set; }
    }
}
