using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class ToolBase : MonoBehaviour
    {
        public virtual string ToolName { get; }

        public virtual string ToolDescription { get; }
    }
}