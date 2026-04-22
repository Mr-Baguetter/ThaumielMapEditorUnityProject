using System.Collections.Generic;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Networking.Blocky.Definitions
{
    public class PlayerDefs : DefBase
    {
        private enum Properties
        {
            Rotation,
            LookRotation,
            Scale,
            Position,
            Role,
            RoleBase,
            PlayerId,
            Name,
            DisplayName,
            LogName,
            UserID,
            IsAlive,
            GameObject,
            ReferenceHub,
            ReadyList,
            IsNpc,
            IsHost,
            IsPlayer,
            IsDummy,
            NetworkId,
            IsDestroyed,
            IsReady,
            LifeId,
            CustomInfo,
            InfoArea,
            Health,
            MaxHealth,
            ArtificialHealth,
            MaxArtificialHealth,
            HumeShield,
            MaxHumeShield,
            HumeShieldRegenRate,
            HumeShieldRegenCooldown,
            Gravity,
            RemoteAdminAccess,
            DoNotTrack,
            IsOverwatchEnabled,
            CurrentlySpectating,
            CurrentSpectators,
            IsSpectatable,
            CurrentItem,
            ActiveEffects,
            Room,
            Zone,
            Items,
            Ammo,
            GroupColor,
            GroupName,
            UserGroup,
            PermissionsGroupName,
            UnitId,
            HasReservedSlot,
            Velocity,
            IsInventoryFull,
            IsWithoutItems,
            IsOutOfAmmo,
            IsDisarmed,
            IsMuted,
            IsIntercomMuted,
            IsUsingRadio,
            IsSpeaking,
            IsGlobalModerator,
            IsNorthwoodStaff,
            IsExiledContributer,
            IsTMEContributer,
            IsBypassEnabled,
            IsGodModeEnabled,
            IsNoclipEnabled,
            DisarmedBy,
            Team,
            Faction,
            IsSCP,
            IsHuman,
            IsNTF,
            IsChaos,
            IsTutorial,
            StaminaRemaining,
            Emotion,
        }
        
        public override void Register()
        {
            BlocklyServer.RegisterCategory("Player", "#df6717", "");

            RegisterGetProperty();
            RegisterGravity();
            RegisterGetPlayer();
            RegisterSetRole();
            RegisterItems();
            RegisterHealth();
            RegisterGroup();
            RegisterMessages();
        }

        private void RegisterGetProperty()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "get_player_property",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gets a property of the player.",
                Message = "Get %2 of Player: %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.Dropdown("Property", EnumOptions<Properties>())
                }
            });
        }

        private void RegisterGravity()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_gravity",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players gravity. Default: (0, -19.86, 0)",
                Message = "Set Gravity of Player: %1 → x: %2  y: %3  z: %4",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("x", 0),
                    BlockArg.NumberField("y", -19.86),
                    BlockArg.NumberField("z", 0)
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_scale",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players scale.",
                Message = "Set Scale of Player: %1 → x: %2  y: %3  z: %4",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("x", 1),
                    BlockArg.NumberField("y", 1),
                    BlockArg.NumberField("z", 1)
                }
            });
        }

        private void RegisterGetPlayer()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "get_player_by_id",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gets a player by their id.",
                Message = "Get Player by ID: %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.NumberField("Player Id")
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "get_player_by_userid",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gets a player by their user id.",
                Message = "Get Player by User ID: %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.TextField("Player User Id")
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "get_player_by_collider",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gets a player by their collider.",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output },
                Message = "Get Player by Collider",
                Args = null
            });
        }

        private void RegisterSetRole()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_role",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets a players role.",
                Message = "Set Role of Player: %1 → %2  Keep Position: %3",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.Dropdown("New Role", EnumOptions<RoleTypeId>()),
                    BlockArg.Checkbox("Keep Position", true)
                }
            });
        }

        private void RegisterItems()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "give_player_item",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gives the player a item.",
                Message = "Give Player: %1  Item: %2  Drop if Full: %3",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.Dropdown("New Item", EnumOptions<ItemType>()),
                    BlockArg.Checkbox("Drop if full", true)
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "give_player_item",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Gives the player multiple items.",
                Message = "Give Player: %1  Item: %2  ×%3  Drop if Full: %4",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.Dropdown("New Item", EnumOptions<ItemType>()),
                    BlockArg.NumberField("Amount", 1),
                    BlockArg.Checkbox("Drop if full", true)
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "remove_player_item",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Removes the item from a player.",
                Message = "Remove Item: %2 from Player: %1",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.Dropdown("Removed Item", EnumOptions<ItemType>()),
                }
            });
        }

        private void RegisterHealth()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_health",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players health.",
                Message = "Set Health of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Health"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_max_health",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players max health.",
                Message = "Set Max Health of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Max Health"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_artificial_health",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players artificial health.",
                Message = "Set Artificial Health of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Artificial Health"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_max_artificial_health",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players max artificial health.",
                Message = "Set Max Artificial Health of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Max Artificial Health"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_hume_shield",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players hume shield.",
                Message = "Set Hume Shield of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Hume shield"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_max_hume_shield",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players max hume shield.",
                Message = "Set Max Hume Shield of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Max Hume Shield"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_hume_shield_regen_rate",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players hume shield regen rate.",
                Message = "Set Hume Shield Regen Rate of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Hume Shield Regen Rate"),
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_hume_shield_regen_cooldown",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players hume shield regen cooldown.",
                Message = "Set Hume Shield Regen Cooldown of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.NumberField("Hume Shield Regen Cooldown"),
                }
            });
        }

        private void RegisterGroup()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_group_name",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players group name.",
                Message = "Set Group Name of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.TextField("Group Name")
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "set_player_group_color",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sets the players group color.",
                Message = "Set Group Color of Player: %1 → %2",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.TextField("Group Color")
                }
            });
        }

        private void RegisterMessages()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "send_player_broadcast",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sends a broadcast message to the player.",
                Message = "Send Broadcast to Player: %1 → Broadcast Message: %2 Duration: %3",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.TextField("Broadcast Message"),
                    BlockArg.NumberField("Duration", 5)
                }
            });

            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "send_player_hint",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "Sends a hint message to the player.",
                Message = "Send Hint to Player: %1 → Hint Message: %2 Duration: %3",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Previous, BlockConnectionType.Next },
                Args = new List<Dictionary<string, object>>
                {
                    BlockArg.Value("Player"),
                    BlockArg.TextField("Hint Message"),
                    BlockArg.NumberField("Duration", 5)
                }
            });
        }

        private void RegisterLists()
        {
            BlocklyServer.RegisterBlock(new BlockDefinition
            {
                Id = "player_list",
                Category = "Player",
                Color = "#df6717",
                Tooltip = "List of all players currently on the server.",
                Message = "Player List",
                Connections = new List<BlockConnectionType> { BlockConnectionType.Output, BlockConnectionType.Input },
                Args = null
            });
        }
    }
}