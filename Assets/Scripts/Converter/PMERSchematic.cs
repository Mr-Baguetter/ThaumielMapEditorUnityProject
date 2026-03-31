using System;
using System.Collections.Generic;

namespace Assets.Scripts.Converter
{
    [Serializable]
    public class PMERSchematic
    {
        /// <summary>
        /// Gets or sets the root id of the PMER schematic
        /// </summary>
        public int RootObjectId { get; set; }
        
        /// <summary>
        /// Gets or sets the blocks of the PMER schematic
        /// </summary>
        public List<PMERBlock> Blocks { get; set; } = new();
    }
}