using UnityEngine;

namespace Assets.Scripts.Components
{
    public class WaypointObject : ObjectBase
    {
        [field: SerializeField]
        public float Priority { get; set; }

        [field: SerializeField]
        public bool VisualizeBounds { get; set; }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["Priority"] = Priority,
                ["VisualizeBounds"] = VisualizeBounds,
                ["BoundsSize"] = transform.localScale
            };
        }
    }
}