using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Plugin.Logger?.LogDebug($"Updating \"{EntitiesName.SnareFlea}\" DamageSinceClingingToPlayer ({_damageSinceClingingToPlayer} => {_damageSinceClingingToPlayer + damage})");
            _damageSinceClingingToPlayer += damage;
            if (_damageSinceClingingToPlayer >= MaxDamagePerClinging &&
                !_centipedeAI.inDroppingOffPlayerAnim)
            {
                Plugin.Logger?.LogInfo($"\"{EntitiesName.SnareFlea}\" reached MaxDamagePerClinging ({_damageSinceClingingToPlayer}/{MaxDamagePerClinging}), triggering StopClinging on player \"{_centipedeAI.clingingToPlayer?.playerUsername}\"");
                _centipedeAI.inDroppingOffPlayerAnim = true;
                if (_centipedeAI.IsServer)
                {
                    _centipedeAI.StopClingingServerRpc(playerDead: false);
                }
                else
                {
                    _centipedeAI.StopClingingClientRpc(playerDead: false);
                }
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

        [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DamagePlayerOnIntervalsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = instructions.ToList();
            const int maxConnectedPlayersAmount = 0;
            bool bFoundPlayersAmount = false;
            FieldInfo connectedPlayersAmount = AccessTools.Field(typeof(StartOfRound), nameof(StartOfRound.connectedPlayersAmount));
            FieldInfo livingPlayers = AccessTools.Field(typeof(StartOfRound), nameof(StartOfRound.livingPlayers));

            Plugin.Logger?.LogDebug($"Enumerating code instructions in \"{nameof(CentipedeAI.DamagePlayerOnIntervals)}\" until we find the conditions to trigger the second chance...");
            for (int i = 0; i < instructionsList.Count; ++i)
            {
                // Commented out to cleanup logs (I wish there was a Verbose log).
                // Plugin.Logger?.LogDebug($"Found code instruction: \"{instructionsList[i]}\"");

                if (instructionsList[i].LoadsField(connectedPlayersAmount) &&
                    i + 1 < instructionsList.Count &&
                    instructionsList[i + 1].LoadsConstant(maxConnectedPlayersAmount))
                {
                    Plugin.Logger?.LogInfo($"Changing \"{EntitiesName.SnareFlea}\" StopClinging from single player to last survivor (multiplayer included)");
                    instructionsList[i].operand = livingPlayers;
                    instructionsList[i + 1].operand = 1;
                    bFoundPlayersAmount = true;
                }

                if (bFoundPlayersAmount)
                {
                    // No need to keep going through any remaining instruction.
                    break;
                }
            }

            if (!bFoundPlayersAmount)
            {
                Plugin.Logger?.LogError($"[{nameof(CentipedeAI.DamagePlayerOnIntervals)}] Cannot find \"{nameof(connectedPlayersAmount)}\"");
            }

            return instructionsList;
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
