using UnityEngine;

namespace Assets.Scripts.Components
{
    public class WorkstationObject : ObjectBase
    {
        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new();
        }
    }
}