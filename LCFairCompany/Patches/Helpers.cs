using UnityEngine;
using StringComparison = System.StringComparison;

namespace LCFairCompany
{
    struct EntitiesName
    {
        // Indoor
        public const string SnareFlea = "Centipede";
        public const string BunkerSpider = "Bunker Spider";
        public const string HoardingBug = "Hoarding Bug";
        public const string Bracken = "Flowerman";
        public const string Thumper = "Crawler";
        public const string Hydrogene = "Blob";
        public const string GhostGirl = "Girl";
        public const string SporeLizard = "Puffer";
        public const string Nutcracker = "Nutcracker";
        public const string Coilhead = "Spring";
        public const string Jester = "Jester";
        public const string Masked = "Masked";

        // Nightttime (Outdoor)
        public const string EyelessDog = "MouthDog";
        public const string ForestKeeper = "ForestGiant";
        public const string EarthLeviathan = "Earth Leviathan";
        public const string BaboonHawk = "Baboon hawk";

        // Daytime (Outdoor)
        public const string CircuitBees = "Red Locust Bees";
        public const string Manticoil = "Manticoil";
        public const string RoamingLocusts = "Docile Locust Bees";

        // Unused
        public const string LassoMan = "Lasso";
    }

    internal static class EnemyHelpers
    {
        public static bool IsMatchingName(this EnemyType enemyType, string enemyName)
        {
            return enemyType.enemyName.Equals(enemyName, StringComparison.OrdinalIgnoreCase);
        }

        public static void SetEnemyPowerLevel(this EnemyType enemyType, int powerLevel)
        {
            if (enemyType.PowerLevel == powerLevel)
            {
                Plugin.Logger?.LogDebug($"\"{enemyType.enemyName}\" PowerLevel is already {powerLevel}, nothing to change");
                return;
            }

            Plugin.Logger?.LogInfo($"Changing \"{enemyType.enemyName}\" PowerLevel ({enemyType.PowerLevel} => {powerLevel})");
            enemyType.PowerLevel = powerLevel;
        }

        public static void SetEnemyMaxCount(this EnemyType enemyType, int maxCount)
        {
            if (enemyType.MaxCount == maxCount)
            {
                Plugin.Logger?.LogDebug($"\"{enemyType.enemyName}\" MaxCount is already {maxCount}, nothing to change");
                return;
            }

            Plugin.Logger?.LogInfo($"Changing \"{enemyType.enemyName}\" MaxCount ({enemyType.MaxCount} => {maxCount})");
            enemyType.MaxCount = maxCount;
        }

        public static void SetEnemyHP(this EnemyAI enemyAI, int enemyHP)
        {
            if (enemyAI.enemyHP == enemyHP)
            {
                Plugin.Logger?.LogDebug($"\"{enemyAI.enemyType.enemyName}\" EnemyHP is already {enemyHP}, nothing to change");
                return;
            }

            Plugin.Logger?.LogInfo($"Changing \"{enemyAI.enemyType.enemyName}\" EnemyHP ({enemyAI.enemyHP} => {enemyHP})");
            enemyAI.enemyHP = enemyHP;
        }
    }

    internal static class AudioHelpers
    {
        public static int PlayRandomOneShot(this AudioSource audioSource, AudioClip[] clipsArray, float volume = 1f, float? minDistance = null, float? maxDistance = null, bool bTransmitToWalkieTalkie = true, int timesPlayedInSameSpot = 0)
        {
            if (audioSource == null || clipsArray == null || clipsArray.Length <= 0)
            {
                return -1;
            }

            // Cache previous values to set them back before exiting.
            float prevPitch = audioSource.pitch;
            float prevMinDist = audioSource.minDistance;
            float prevMaxDist = audioSource.maxDistance;

            audioSource.pitch = Random.Range(0.94f, 1.06f);

            if (minDistance.HasValue)
            {
                audioSource.minDistance = minDistance.Value;
            }

            if (maxDistance.HasValue)
            {
                audioSource.maxDistance = maxDistance.Value;
            }

            int num = Random.Range(0, clipsArray.Length);
            audioSource.PlayOneShot(clipsArray[num], volume);

            if (bTransmitToWalkieTalkie)
            {
                WalkieTalkie.TransmitOneShotAudio(audioSource, clipsArray[num], 0.85f * volume);
            }

            if (audioSource.spatialBlend > 0f)
            {
                RoundManager.Instance.PlayAudibleNoise(audioSource.transform.position, 4f * volume, volume / 2f, timesPlayedInSameSpot, noiseIsInsideClosedShip: false, noiseID: 0);
            }

            // Re-apply previous values before exiting.
            audioSource.pitch = prevPitch;
            audioSource.minDistance = prevMinDist;
            audioSource.maxDistance = prevMaxDist;

            return num;
        }
    }
}