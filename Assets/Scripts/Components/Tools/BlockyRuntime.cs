using Assets.Scripts.Enums;
using Assets.Scripts.Networking.Blocky;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Components.Tools
{
    public class BlockyRuntime : ToolBase
    {
        public CodeExportPayload Blocky;

        public override ToolType ToolType => ToolType.BlockyRuntime;

        public override void Compile()
        {
            Properties = new()
            {
                ["Blocky"] = Blocky
            };
        }

        public override void OnBlocklyExportReceived(CodeExportPayload payload, string targetEvent)
        {
            if (targetEvent == nameof(Blocky))
            {
                Blocky = payload;
                Debug.Log($"[BlockyRuntime] Successfully added export to {gameObject.name}'s blocky property.");
            }

            EditorUtility.SetDirty(this);
        }
    }
}