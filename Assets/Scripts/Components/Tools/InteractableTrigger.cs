using System;
using Assets.Scripts.Components.Tools.Helpers;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Components.Tools
{
    public class InteractableTrigger : ToolBase
    {
        public Vector3 Bounds;

        public ColliderShape Shape;

        public float InteractionTime;

        public Permission Permissions;

        public InteractableClasses OnInteracted;

        public InteractableClasses OnInteractionDenied;

        public override ToolType ToolType => ToolType.InteractableTrigger;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.orange;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            switch (Shape)
            {
                case ColliderShape.Box:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                    Gizmos.DrawWireCube(Vector3.zero, Bounds);
                    break;

                case ColliderShape.Sphere:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    float radius = Bounds.x * transform.lossyScale.x * 0.5f;
                    Gizmos.DrawWireSphere(Vector3.zero, radius);
                    break;

                case ColliderShape.Capsule:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    float cRadius = Mathf.Min(Bounds.x * transform.lossyScale.x, Bounds.z * transform.lossyScale.z) * 0.5f;
                    float cHeight = Bounds.y * transform.lossyScale.y * 0.5f - cRadius;

                    Gizmos.DrawWireSphere(new Vector3(0,  cHeight, 0), cRadius);
                    Gizmos.DrawWireSphere(new Vector3(0, -cHeight, 0), cRadius);

                    Gizmos.DrawLine(new Vector3( cRadius,  cHeight, 0), new Vector3( cRadius, -cHeight, 0));
                    Gizmos.DrawLine(new Vector3(-cRadius,  cHeight, 0), new Vector3(-cRadius, -cHeight, 0));
                    Gizmos.DrawLine(new Vector3(0,  cHeight,  cRadius), new Vector3(0, -cHeight,  cRadius));
                    Gizmos.DrawLine(new Vector3(0,  cHeight, -cRadius), new Vector3(0, -cHeight, -cRadius));
                    break;
            }
        }

        public override void Compile()
        {
            Properties = new()
            {
                ["Bounds"] = Bounds,
                ["Shape"] = Shape,
                ["Permission"] = Permissions,
                ["InteractionTime"] = InteractionTime,
                ["OnInteracted"] = OnInteracted,
                ["OnInteractionDenied"] = OnInteractionDenied
            };
        }

        public override void Decompile()
        {
            if (Properties.TryGetValue("OnInteracted", out var interacted))
                OnInteracted = YamlHelpers.ParseObject<InteractableClasses>(interacted);

            if (Properties.TryGetValue("OnInteractionDenied", out var denied))
                OnInteractionDenied = YamlHelpers.ParseObject<InteractableClasses>(denied);      

            if (Properties.TryGetValue("InteractionTime", out var time))
                InteractionTime = Convert.ToSingle(time);

            if (Properties.TryGetValue("Shape", out var shape))
                Shape = YamlHelpers.ParseEnum<ColliderShape>(shape);

            if (Properties.TryGetValue("Bounds", out var bounds))
                Bounds = YamlHelpers.ParseVector3(bounds);

            if (Properties.TryGetValue("Permission", out var permission))
                Permissions = YamlHelpers.ParseObject<Permission>(permission);
        }
    }
}