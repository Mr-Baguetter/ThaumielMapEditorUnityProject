using System.Collections.Generic;

namespace Assets.Scripts.Networking.Blocky.Definitions
{
    public class TextToyDefs : DefBase
    {
        private enum Properties
        {
            Rotation,
            Scale,
            Position,
            Base,
            Text,
            DisplaySize,
        }

        public override void Register()
        {
            BlocklyServer.RegisterCategory("TextToy", "#2E7D32", "");

            RegisterCreateTextToy();
            RegisterSetText();
            RegisterSetDisplaySize();
            RegisterGetProperty();
        }

        private void RegisterGetProperty()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "get_texttoy_property",
                Category = "TextToy",
                Color = "#2E7D32",
                Tooltip = "Gets a property of the text toy.",
                Message = "Get %1 of TextToy",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Dropdown("Property", EnumOptions<Properties>())
                }
            });
        }

        private void RegisterCreateTextToy()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "texttoy_create",
                Category = "TextToy",
                Color = "#2E7D32",
                Tooltip = "Create a new TextToyObject with a given name.",
                Message = "Create TextToy  Name: %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.TextField("name", "MyTextToy")
                }
            });
        }

        private void RegisterSetText()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "texttoy_set_text",
                Category = "TextToy",
                Color = "#2E7D32",
                Tooltip = "Set the text format string displayed by the TextToy.",
                Message = "Set Text → %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.TextField("text", "Hello World")
                }
            });
        }

        private void RegisterSetDisplaySize()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "texttoy_set_display_size",
                Category = "TextToy",
                Color = "#2E7D32",
                Tooltip = "Set the display size (width and height) of the TextToy.",
                Message = "Set Display Size →  w: %1  h: %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.NumberField("x", 1.0),
                    BlockArg.NumberField("y", 1.0)
                }
            });
        }
    }
}