using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Converter.PMERConverter;

namespace Assets.Scripts.Converter
{
    [Serializable]
    public class PMERBlock
    {
        /// <summary>
        /// Gets or sets the name of the PMER block
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the object id of the PMER block
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the parent id of the PMER block
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the position of the PMER block
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the PMER block
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale of the PMER block
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the blocktype of the PMER block
        /// </summary>
        public PMERBlockType BlockType { get; set; }

        /// <summary>
        /// Gets or sets the properties of the PMER block
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}