using GameNetcodeStuff;
using HarmonyLib;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal static class EnemyAIPatch
    {
        [HarmonyPatch(nameof(EnemyAI.PlayerIsTargetable))]
        [HarmonyPostfix]
        private static void PlayerIsTargetablePostfix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript)
        {
            if (!__result || !(__instance is CentipedeAI))
            {
                return;
            }

            if (playerScript.TryGetComponent(out PlayerControllerBPatch patch))
            {
                // There's already a centipede on this player's head, lets not stack them...
                if (patch.IsClingingToPlayer())
                {
                    // Commented out to avoid spamming logs (I wish there was a way to collapse similar messages).
                    // Plugin.Logger?.LogDebug($"Preventing another \"{EntitiesName.SnareFlea}\" from targeting player \"{playerScript?.playerUsername}\"");
                    __result = false;
                }
            }
        }
    }
}
