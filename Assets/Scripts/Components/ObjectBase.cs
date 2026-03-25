using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ObjectBase : MonoBehaviour
    {
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

            Position = root.InverseTransformPoint(t.position);
            Rotation = Quaternion.Inverse(root.rotation) * t.rotation.eulerAngles;
            Scale = gameObject.transform.localScale;
            Static = gameObject.isStatic;
        }
    }
}