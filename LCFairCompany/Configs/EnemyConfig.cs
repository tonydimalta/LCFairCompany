using BepInEx.Configuration;
using System;

namespace LCFairCompany.Configs
{
    internal class EnemyConfig
    {
        public ConfigEntry<bool> BunkerSpiderChangePowerLevel { get; private set; } = null;
        public ConfigEntry<bool> BunkerSpiderChangeMaxCount { get; private set; } = null;
        public ConfigEntry<bool> CoilheadChangePowerLevel { get; private set; } = null;
        public ConfigEntry<bool> CoilheadChangeMaxCount { get; private set; } = null;
        public ConfigEntry<bool> GhostGirlChangePowerLevel { get; private set; } = null;
        public ConfigEntry<bool> JesterChangePowerLevel { get; private set; } = null;

        public EnemyConfig(ConfigFile config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            BunkerSpiderChangePowerLevel = config.Bind("Bunker Spider Settings", "Change Power Level", true, new ConfigDescription("If enabled, makes it more likely to spawn (e.g. more than the Bracken but less than Hoarding Bugs)."));
            BunkerSpiderChangeMaxCount = config.Bind("Bunker Spider Settings", "Change Max Count", true, new ConfigDescription("If enabled, there can be at most 2 spiders spawned on the map (instead of 1). Do you prefer potentially having 2 spiders or other monsters? That's up to you."));
            CoilheadChangePowerLevel = config.Bind("Coilhead Settings", "Change Power Level", true, new ConfigDescription("If enabled, makes it less likely to spawn (e.g. more than the Bracken but less than Hoarding Bugs)."));
            CoilheadChangeMaxCount = config.Bind("Coilhead Settings", "Change Max Count", true, new ConfigDescription("If enabled, there can be at most 4 coilheads spawned on the map (instead of 5)."));
            GhostGirlChangePowerLevel = config.Bind("Ghost Girl Settings", "Change Power Level", true, new ConfigDescription("If enabled, makes it less likely to spawn (e.g. same as the Bracken)."));
            JesterChangePowerLevel = config.Bind("Jester Settings", "Change Power Level", true, new ConfigDescription("If enabled, makes it more likely to spawn (e.g. more than the Bracken but less than Hoarding Bugs)."));
        }

        ~EnemyConfig()
        {
            if (BunkerSpiderChangePowerLevel != null)
            {
                BunkerSpiderChangePowerLevel.ConfigFile.Remove(BunkerSpiderChangePowerLevel.Definition);
                BunkerSpiderChangePowerLevel = null;
            }

            if (BunkerSpiderChangeMaxCount != null)
            {
                BunkerSpiderChangeMaxCount.ConfigFile.Remove(BunkerSpiderChangeMaxCount.Definition);
                BunkerSpiderChangeMaxCount = null;
            }

            if (CoilheadChangePowerLevel != null)
            {
                CoilheadChangePowerLevel.ConfigFile.Remove(CoilheadChangePowerLevel.Definition);
                CoilheadChangePowerLevel = null;
            }

            if (CoilheadChangeMaxCount != null)
            {
                CoilheadChangeMaxCount.ConfigFile.Remove(CoilheadChangeMaxCount.Definition);
                CoilheadChangeMaxCount = null;
            }

            if (GhostGirlChangePowerLevel != null)
            {
                GhostGirlChangePowerLevel.ConfigFile.Remove(GhostGirlChangePowerLevel.Definition);
                GhostGirlChangePowerLevel = null;
            }

            if (JesterChangePowerLevel != null)
            {
                JesterChangePowerLevel.ConfigFile.Remove(JesterChangePowerLevel.Definition);
                JesterChangePowerLevel = null;
            }
        }
    }
}