using GameNetcodeStuff;
using HarmonyLib;
using LCFairCompany.Configs;
using UnityEngine;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(CentipedeAI))]
    internal class CentipedeAIPatch : MonoBehaviour
    {
        private CentipedeAI _centipedeAI = null;

        private int _damageSinceClingingToPlayer = 0;
        private int _timesPlayedInSameSpot = 0;
        private float? _delayUntilNextAudioCue = null;

        private bool LimitDamagePerClinging => ConfigManager.Instance.Centipede.LimitDamagePerClinging.Value;
        private int MaxDamagePerClinging => ConfigManager.Instance.Centipede.MaxDamagePerClinging.Value;
        private bool PlayAudioShriekOnCeiling => ConfigManager.Default.Centipede.PlayAudioShriekOnCeiling.Value;
        private int AudioShriekMinFrequency => ConfigManager.Default.Centipede.AudioShriekMinFrequency.Value;
        private int AudioShriekMaxFrequency => ConfigManager.Default.Centipede.AudioShriekMaxFrequency.Value;

        protected void Start()
        {
            Plugin.Logger?.LogDebug($"\"{EntitiesName.SnareFlea}\" applies \"{nameof(CentipedeAIPatch)}\"");
            _centipedeAI = gameObject.GetComponent<CentipedeAI>();
        }

        protected void LateUpdate()
        {
            if (PlayAudioShriekOnCeiling)
            {
                UpdateDelayUntilNextAudioCue(Time.deltaTime);
            }
            else
            {
                _delayUntilNextAudioCue = null;
            }
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
            else if (LimitDamagePerClinging &&
                _damageSinceClingingToPlayer >= MaxDamagePerClinging)
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

        private void PlayRandomShriekSFX()
        {
            if (_centipedeAI == null)
            {
                return;
            }

            Plugin.Logger?.LogDebug($"Playing \"{EntitiesName.SnareFlea}\" RandomShriekSFX ({nameof(_timesPlayedInSameSpot)}={_timesPlayedInSameSpot})");
            AudioHelpers.PlayRandomOneShot(
                _centipedeAI.creatureSFX,
                _centipedeAI.shriekClips,
                volume: .1f,
                minDistance: 10f,
                maxDistance: 35f,
                bTransmitToWalkieTalkie: false,
                _timesPlayedInSameSpot
            );
        }

        private void UpdateDelayUntilNextAudioCue(float deltaTime)
        {
            if (_centipedeAI == null ||
                !_centipedeAI.IsClient ||
                !_centipedeAI.clingingToCeiling ||
                _centipedeAI.ceilingAnimationCoroutine != null)
            {
                _delayUntilNextAudioCue = null;
                _timesPlayedInSameSpot = 0;
                return;
            }

            if (!_delayUntilNextAudioCue.HasValue)
            {
                float minDelay = 60f / Mathf.Max(AudioShriekMaxFrequency, 1);
                float maxDelay = 60f / Mathf.Max(AudioShriekMinFrequency, 1);
                if (minDelay <= maxDelay)
                {
                    _delayUntilNextAudioCue = Random.Range(minDelay, maxDelay);
                }
                else
                {
                    _delayUntilNextAudioCue = Random.Range(maxDelay, minDelay);
                }
            }

            _delayUntilNextAudioCue -= deltaTime;
            if (_delayUntilNextAudioCue <= 0f)
            {
                PlayRandomShriekSFX();
                _delayUntilNextAudioCue = null;
                ++_timesPlayedInSameSpot;
            }
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
            __state = __instance.clingingToPlayer?.health ?? 0;
        }

        [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
        [HarmonyPostfix]
        private static void DamagePlayerOnIntervalsPostfix(CentipedeAI __instance, int __state)
        {
            int damage = __state - __instance.clingingToPlayer?.health ?? 0;
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
