using BepInEx.Configuration;
using System;

namespace LCFairCompany.Configs
{
    internal class SandSpiderConfig
    {
        public ConfigEntry<int> Health { get; private set; } = null;
        public ConfigEntry<int> Damage { get; private set; } = null;
        public ConfigEntry<int> ScaleDPS { get; private set; } = null;

        public SandSpiderConfig(ConfigFile config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Health = config.Bind("Bunker Spider Settings", "Health", 4, new ConfigDescription("Changes how much health the spider has. Game default: 5", new AcceptableValueRange<int>(minValue: 3, maxValue: 5)));
            Damage = config.Bind("Bunker Spider Settings", "Damage", 60, new ConfigDescription("Changes how much damage the spider does per hit. Game default: 90", new AcceptableValueRange<int>(minValue: 45, maxValue: 90)));
            // Aiming for 45 DPS (half the base value) which gives 75% DPS (~1.33 sec of cooldown for 60 damage)
            ScaleDPS = config.Bind("Bunker Spider Settings", "Scale DPS (%)", 75, new ConfigDescription("Allows to reduce the damage per second (i.e. how fast the spider can chain hits). Game default: 100%", new AcceptableValueRange<int>(minValue: 50, maxValue: 100)));
        }

        ~SandSpiderConfig()
        {
            if (Health != null)
            {
                Health.ConfigFile.Remove(Health.Definition);
                Health = null;
            }

            if (Damage != null)
            {
                Damage.ConfigFile.Remove(Damage.Definition);
                Damage = null;
            }

            if (ScaleDPS != null)
            {
                ScaleDPS.ConfigFile.Remove(ScaleDPS.Definition);
                ScaleDPS = null;
            }
        }
    }
}