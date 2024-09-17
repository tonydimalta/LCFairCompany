using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Netcode;

namespace LCFairCompany.Configs
{
    [Serializable]
    internal class ConfigManager : SyncedInstance<ConfigManager>
    {
        public EnemyConfig Enemy { get; private set; } = null;
        public SandSpiderConfig Spider { get; private set; } = null;
        public CentipedeConfig Centipede { get; private set; } = null;

        private ConfigFile _config = null;
        public ConfigFile Config
        {
            get
            {
                return _config;
            }
            set
            {
                if (_config != null ||
                    value == null)
                {
                    Enemy = null;
                    Spider = null;
                    Centipede = null;
                }

                _config = value;

                if (_config != null)
                {
                    Plugin.Logger?.LogDebug("Binding config...");

                    // We want to disable saving our config file every time we bind a
                    // setting as it's inefficient and slow
                    _config.SaveOnConfigSet = false;

                    Enemy = new EnemyConfig(_config);
                    Spider = new SandSpiderConfig(_config);
                    Centipede = new CentipedeConfig(_config);

                    // Get rid of old settings from the config file that are not used anymore
                    ClearOrphanedEntries(_config);
                    // We need to manually save since we disabled `SaveOnConfigSet` earlier
                    _config.Save();
                    // And finally, we re-enable `SaveOnConfigSet` so changes to our config
                    // entries are written to the config file automatically from now on
                    _config.SaveOnConfigSet = true;
                }
            }
        }

        public ConfigManager(ConfigFile config)
        {
            InitInstance(this);
            Config = config;
        }

        public static void RequestSync()
        {
            if (!IsClient)
            {
                return;
            }

            using var stream = new FastBufferWriter(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage($"{PluginInfo.PLUGIN_NAME}_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost)
            {
                return;
            }

            Plugin.Logger?.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using var stream = new FastBufferWriter(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage($"{PluginInfo.PLUGIN_NAME}_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                Plugin.Logger?.LogError($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                Plugin.Logger?.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                Plugin.Logger?.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            Plugin.Logger?.LogInfo("Successfully synced config with host.");
        }

        private static void ClearOrphanedEntries(ConfigFile config)
        {
            // Find the private property `OrphanedEntries` from the type `ConfigFile`
            PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            // And get the value of that property from our ConfigFile instance
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(config);
            // And finally, clear the `OrphanedEntries` dictionary
            orphanedEntries.Clear();
        }
    }
}