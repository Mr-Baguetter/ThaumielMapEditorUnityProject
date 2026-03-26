using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ObjectBase : MonoBehaviour
    {
        public int ObjectId { get; set; }
        public int ParentId { get; set; }

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }

        [field: SerializeField]
        public bool Static { get; set; }

        [field: SerializeField]
        public float MovementSmoothing { get; set; }

        public ObjectType Type { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public virtual void Compile(Transform root)
        {
            Transform t = transform;
            Name = t.name;

            ObjectId = transform.gameObject.GetInstanceID();
            ParentId = root.gameObject.GetInstanceID();
            Position = root.InverseTransformPoint(t.position);
            Rotation = Quaternion.Inverse(root.rotation) * t.rotation.eulerAngles;
            Scale = gameObject.transform.localScale;
            Static = gameObject.isStatic;
        }

        public virtual void Decompile(Transform root)
        {
            Transform t = transform;
            t.name = Name;

            t.SetPositionAndRotation(root.TransformPoint(Position), root.rotation * Quaternion.Euler(Rotation));
            t.localScale = Scale;
            gameObject.isStatic = Static;
        }
    }
}