using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(CentipedeAI))]
    internal class CentipedeAIPatch : MonoBehaviour
    {
        private CentipedeAI _centipedeAI;

        private int _damageSinceClingingToPlayer = 0;

        public const int MaxDamagePerClinging = 60;

        protected void Start()
        {
            Plugin.Logger?.LogDebug($"\"{EntitiesName.SnareFlea}\" applies \"{nameof(CentipedeAIPatch)}\"");
            _centipedeAI = gameObject.GetComponent<CentipedeAI>();
        }

        public void OnPlayerDamaged(int damage)
        {
            if (_centipedeAI.inDroppingOffPlayerAnim)
            {
                return;
            }

            Plugin.Logger?.LogDebug($"Updating \"{EntitiesName.SnareFlea}\" DamageSinceClingingToPlayer ({_damageSinceClingingToPlayer} => {_damageSinceClingingToPlayer + damage})");
            _damageSinceClingingToPlayer += damage;

            bool bShouldStopClinging = false;
            if (StartOfRound.Instance.connectedPlayersAmount > 0 && StartOfRound.Instance.livingPlayers == 1 && _centipedeAI.clingingToPlayer.health <= 15 && !StartOfRoundPatch.LastSurvivorSecondChanceGiven)
            {
                Plugin.Logger?.LogInfo($"\"{EntitiesName.SnareFlea}\" is attempting to kill the last survivor");
                StartOfRoundPatch.LastSurvivorSecondChanceGiven = true;
                bShouldStopClinging = true;
            }
            else if (_damageSinceClingingToPlayer >= MaxDamagePerClinging)
            {
                Plugin.Logger?.LogInfo($"\"{EntitiesName.SnareFlea}\" reached MaxDamagePerClinging ({_damageSinceClingingToPlayer}/{MaxDamagePerClinging})");
                bShouldStopClinging = true;
            }

            if (bShouldStopClinging)
            {
                _centipedeAI.inDroppingOffPlayerAnim = true;
                _centipedeAI.StopClingingServerRpc(playerDead: false);
            }
        }

        public void OnStopClingingToPlayer()
        {
            Plugin.Logger?.LogDebug($"Resetting \"{EntitiesName.SnareFlea}\" DamageSinceClingingToPlayer ({_damageSinceClingingToPlayer} => 0)");
            _damageSinceClingingToPlayer = 0;
        }

        [HarmonyPatch(nameof(CentipedeAI.Start))]
        [HarmonyPostfix]
        private static void StartPostfix(CentipedeAI __instance)
        {
            __instance.gameObject.AddComponent<CentipedeAIPatch>();
        }

        [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
        [HarmonyPrefix]
        private static void DamagePlayerOnIntervalsPrefix(CentipedeAI __instance, out int __state)
        {
            __state = __instance.clingingToPlayer.health;
        }

        [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
        [HarmonyPostfix]
        private static void DamagePlayerOnIntervalsPostfix(CentipedeAI __instance, int __state)
        {
            int damage = __state - __instance.clingingToPlayer.health;
            if (damage > 0 &&
                __instance.gameObject.TryGetComponent(out CentipedeAIPatch patch))
            {
                patch.OnPlayerDamaged(damage);
            }
        }

        [HarmonyPatch(nameof(CentipedeAI.ClingToPlayer))]
        [HarmonyPostfix]
        private static void ClingToPlayerPostfix(CentipedeAI __instance, PlayerControllerB playerScript)
        {
            if (playerScript.TryGetComponent(out PlayerControllerBPatch patch))
            {
                patch.OnClingToPlayer(__instance);
            }
        }

        [HarmonyPatch(nameof(CentipedeAI.StopClingingToPlayer))]
        [HarmonyPrefix]
        private static void StopClingingToPlayerPrefix(CentipedeAI __instance)
        {
            if (__instance.clingingToPlayer.TryGetComponent(out PlayerControllerBPatch patch))
            {
                patch.OnStopClingingToPlayer();
            }
        }

        [HarmonyPatch(nameof(CentipedeAI.StopClingingToPlayer))]
        [HarmonyPostfix]
        private static void StopClingingToPlayerPostfix(CentipedeAI __instance)
        {
            if (__instance.gameObject.TryGetComponent(out CentipedeAIPatch patch))
            {
                patch.OnStopClingingToPlayer();
            }
        }
    }
}
