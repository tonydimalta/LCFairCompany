using BepInEx.Configuration;
using System;

namespace LCFairCompany.Configs
{
    internal class CentipedeConfig
    {
        public ConfigEntry<bool> LimitDamagePerClinging { get; private set; } = null;
        public ConfigEntry<int> MaxDamagePerClinging { get; private set; } = null;
        public ConfigEntry<bool> PlayAudioShriekOnCeiling { get; private set; } = null;
        public ConfigEntry<int> AudioShriekMinFrequency { get; private set; } = null;
        public ConfigEntry<int> AudioShriekMaxFrequency { get; private set; } = null;

        public CentipedeConfig(ConfigFile config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            LimitDamagePerClinging = config.Bind("Snare Flea Settings", "Limit Damage Per Clinging", true, new ConfigDescription("Limit how much damage Snare Fleas can inflict per clinging to any player (i.e. prevents getting oneshot if a player doesn't find a shovel or the exit in time)."));
            MaxDamagePerClinging = config.Bind("Snare Flea Settings", "Max Damage Per Clinging", 60, new ConfigDescription("If limit is applied, Snare Fleas stop clinging from any player after inflicting that much damage.", new AcceptableValueRange<int>(minValue: 50, maxValue: 100)));
            PlayAudioShriekOnCeiling = config.Bind("Snare Flea Settings (client only)", "Play Audio Shriek On Ceiling", true, new ConfigDescription("Add a quiet audio shriek (triggering 3 to 4 times per minute by default) when a Snare Flea is on a ceiling."));
            AudioShriekMinFrequency = config.Bind("Snare Flea Settings (client only)", "Audio Shriek Min Frequency", 3, new ConfigDescription("Snare Fleas will play the audio shriek at least these many times per minute.", new AcceptableValueRange<int>(minValue: 1, maxValue: 5)));
            AudioShriekMaxFrequency = config.Bind("Snare Flea Settings (client only)", "Audio Shriek Max Frequency", 4, new ConfigDescription("Snare Fleas will play the audio shriek at most these many times per minute.", new AcceptableValueRange<int>(minValue: 1, maxValue: 5)));
        }

        ~CentipedeConfig()
        {
            if (LimitDamagePerClinging != null)
            {
                LimitDamagePerClinging.ConfigFile.Remove(LimitDamagePerClinging.Definition);
                LimitDamagePerClinging = null;
            }

            if (MaxDamagePerClinging != null)
            {
                MaxDamagePerClinging.ConfigFile.Remove(MaxDamagePerClinging.Definition);
                MaxDamagePerClinging = null;
            }

            if (PlayAudioShriekOnCeiling != null)
            {
                PlayAudioShriekOnCeiling.ConfigFile.Remove(PlayAudioShriekOnCeiling.Definition);
                PlayAudioShriekOnCeiling = null;
            }

            if (AudioShriekMinFrequency != null)
            {
                AudioShriekMinFrequency.ConfigFile.Remove(AudioShriekMinFrequency.Definition);
                AudioShriekMinFrequency = null;
            }

            if (AudioShriekMaxFrequency != null)
            {
                AudioShriekMaxFrequency.ConfigFile.Remove(AudioShriekMaxFrequency.Definition);
                AudioShriekMaxFrequency = null;
            }
        }
    }
}