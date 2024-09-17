using HarmonyLib;
using LCFairCompany.Configs;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal static class StartOfRoundPatch
    {
        public static bool LastSurvivorSecondChanceGiven = false;

        private static bool BunkerSpiderChangeMaxCount => ConfigManager.Instance.Enemy.BunkerSpiderChangeMaxCount.Value;
        private static bool CoilheadChangePowerLevel => ConfigManager.Instance.Enemy.CoilheadChangePowerLevel.Value;
        private static bool CoilheadChangeMaxCount => ConfigManager.Instance.Enemy.CoilheadChangeMaxCount.Value;
        private static bool GhostGirlChangePowerLevel => ConfigManager.Instance.Enemy.GhostGirlChangePowerLevel.Value;
        private static bool JesterChangePowerLevel => ConfigManager.Instance.Enemy.JesterChangePowerLevel.Value;

        [HarmonyPatch(nameof(StartOfRound.StartGame))]
        [HarmonyPostfix]
        private static void StartGamePostfix()
        {
            LastSurvivorSecondChanceGiven = false;

            if (StartOfRound.Instance is null || !StartOfRound.Instance.IsServer)
            {
                return;
            }

            if (StartOfRound.Instance.currentLevel is null)
            {
                Plugin.Logger?.LogError($"{nameof(StartOfRound.Instance.currentLevel)} is null!");
                return;
            }

            Plugin.Logger?.LogDebug("Enumerating all spawnable enemies...");
            foreach (SpawnableEnemyWithRarity spawnableEnemy in StartOfRound.Instance.currentLevel.Enemies)
            {
                if (spawnableEnemy is null || spawnableEnemy.enemyType is null)
                {
                    continue;
                }

                EnemyType enemyType = spawnableEnemy.enemyType;
                Plugin.Logger?.LogDebug($"Found spawnable enemy: \"{enemyType.enemyName}\"");
                if (enemyType.IsMatchingName(EntitiesName.BunkerSpider))
                {
                    if (BunkerSpiderChangeMaxCount)
                    {
                        enemyType.SetEnemyMaxCount(2); // from 1
                    }
                }
                else if (enemyType.IsMatchingName(EntitiesName.Coilhead))
                {
                    if (CoilheadChangePowerLevel)
                    {
                        enemyType.SetEnemyPowerLevel(1.5f); // from 1
                    }

                    if (CoilheadChangeMaxCount)
                    {
                        enemyType.SetEnemyMaxCount(4); // from 5
                    }
                }
                else if (enemyType.IsMatchingName(EntitiesName.GhostGirl))
                {
                    if (GhostGirlChangePowerLevel)
                    {
                        enemyType.SetEnemyPowerLevel(3f); // from 2
                    }
                }
                else if (enemyType.IsMatchingName(EntitiesName.Jester))
                {
                    if (JesterChangePowerLevel)
                    {
                        enemyType.SetEnemyPowerLevel(2f); // from 3
                    }
                }
            }
        }
    }
}