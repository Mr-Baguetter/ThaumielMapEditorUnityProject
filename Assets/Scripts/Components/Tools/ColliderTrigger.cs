using Assets.Scripts.Components.Tools.Helpers;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Components.Tools
{
    public class ColliderTrigger : ToolBase
    {
        public Vector3 Bounds;

        public ColliderClasses OnEntered;

        public ColliderClasses OnExited;

        public Permission Permissions;

        public override ToolType ToolType => ToolType.ColliderTrigger;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, Bounds);
        }

        public override void Compile()
        {
            Properties = new()
            {
                ["Bounds"] = Bounds,
                ["OnEntered"] = OnEntered,
                ["OnExited"] = OnExited,
                ["Permission"] = Permissions
            };
        }

        public override void Decompile()
        {
            if (Properties.TryGetValue("OnEntered", out var entered))
                OnEntered = YamlHelpers.ParseObject<ColliderClasses>(entered);

            if (Properties.TryGetValue("OnExited", out var exited))
                OnExited = YamlHelpers.ParseObject<ColliderClasses>(exited);

            if (Properties.TryGetValue("Bounds", out var bounds))
                Bounds = YamlHelpers.ParseVector3(bounds);

            if (Properties.TryGetValue("Permission", out var perms))
                Permissions = YamlHelpers.ParseObject<Permission>(perms);
        }
    }
}