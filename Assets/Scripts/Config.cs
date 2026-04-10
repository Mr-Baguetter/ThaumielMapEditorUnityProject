using System;
using System.IO;

namespace Assets.Scripts
{
    public class Config
    {
        public bool OpenExportAfterCompiling { get; set; }

        public string ExportPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ThaumielMapEditor");
        
        public bool CompressExport { get; set; }
    }
}