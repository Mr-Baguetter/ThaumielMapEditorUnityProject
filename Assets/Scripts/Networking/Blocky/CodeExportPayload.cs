using System;
using System.Collections.Generic;

namespace Assets.Scripts.Networking.Blocky
{
    [Serializable]
    public class CodeExportPayload
    {
        public string Type;

        public string Language;

        public string Code;

        public string Xml;

        public List<object> Blocks;

        public string Timestamp;
    }
}