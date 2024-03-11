using HarmonyLib;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal static class StartOfRoundPatch
    {
        public static bool LastSurvivorSecondChanceGiven = false;

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
                    enemyType.SetEnemyPowerLevel(2); // from 3
                    enemyType.SetEnemyMaxCount(2); // from 1
                }
                else if (enemyType.IsMatchingName(EntitiesName.Coilhead))
                {
                    enemyType.SetEnemyPowerLevel(2); // from 1
                    enemyType.SetEnemyMaxCount(4); // from 5
                }
                else if (enemyType.IsMatchingName(EntitiesName.GhostGirl))
                {
                    enemyType.SetEnemyPowerLevel(3); // from 2
                }
                else if (enemyType.IsMatchingName(EntitiesName.Jester))
                {
                    enemyType.SetEnemyPowerLevel(2); // from 3
                }
            }
        }
    }
}