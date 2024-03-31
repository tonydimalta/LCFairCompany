using HarmonyLib;
using LCFairCompany.Configs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(SandSpiderAI))]
    internal static class SandSpiderAIPatch
    {
        private static int Health => ConfigManager.Instance.Spider.Health.Value;
        private static int Damage => ConfigManager.Instance.Spider.Damage.Value;
        private static int ScaleDPS => ConfigManager.Instance.Spider.ScaleDPS.Value;

        [HarmonyPatch(nameof(SandSpiderAI.Start))]
        [HarmonyPostfix]
        private static void StartPostfix(SandSpiderAI __instance)
        {
            __instance.SetEnemyHP(Health); // from 5
        }

        [HarmonyPatch(nameof(SandSpiderAI.OnCollideWithPlayer))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnCollideWithPlayerTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = instructions.ToList();
            const int baseDamage = 90;
            bool bFoundDamage = false;
            const float baseCooldown = 1f;
            float currentDPS = Damage / baseCooldown;
            float scaledDPS = currentDPS * ScaleDPS / 100f;
            float patchedCooldown = Damage / scaledDPS;
            FieldInfo timeSinceHittingPlayer = AccessTools.Field(typeof(SandSpiderAI), nameof(SandSpiderAI.timeSinceHittingPlayer));
            bool bFoundCooldown = false;

            Plugin.Logger?.LogDebug($"Enumerating code instructions in \"{nameof(SandSpiderAI.OnCollideWithPlayer)}\" until we find the hardcoded damage ({baseDamage}) and cooldown ({baseCooldown})...");
            for (int i = 0; i < instructionsList.Count; ++i)
            {
                // Commented out to cleanup logs (I wish there was a Verbose log).
                // Plugin.Logger?.LogDebug($"Found code instruction: \"{instructionsList[i]}\"");

                // We can't rely on the method using the hardcoded damageNumber to find it as
                // the instructions don't occur in a row, so let's assume it's the right one...
                if (instructionsList[i].LoadsConstant(baseDamage))
                {
                    Plugin.Logger?.LogInfo($"Changing \"{EntitiesName.BunkerSpider}\" Damage ({baseDamage} => {Damage})");
                    instructionsList[i].operand = Damage;
                    bFoundDamage = true;
                }

                if (instructionsList[i].LoadsField(timeSinceHittingPlayer) &&
                    i + 1 < instructionsList.Count &&
                    instructionsList[i + 1].LoadsConstant(baseCooldown))
                {
                    Plugin.Logger?.LogInfo($"Changing \"{EntitiesName.BunkerSpider}\" HitCooldown ({baseCooldown} => {patchedCooldown})");
                    instructionsList[i + 1].operand = patchedCooldown;
                    bFoundCooldown = true;
                }

                if (bFoundDamage && bFoundCooldown)
                {
                    // No need to keep going through any remaining instruction.
                    break;
                }
            }

            if (!bFoundDamage)
            {
                Plugin.Logger?.LogError($"[{nameof(SandSpiderAI.OnCollideWithPlayer)}] Cannot find hardcoded damage ({baseDamage})");
            }

            if (!bFoundCooldown)
            {
                Plugin.Logger?.LogError($"[{nameof(SandSpiderAI.OnCollideWithPlayer)}] Cannot find \"{timeSinceHittingPlayer.Name}\" or hardcoded cooldown ({baseCooldown})");
            }

            return instructionsList;
        }
    }
}
