using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : MonoBehaviour
    {
        private PlayerControllerB _playerController = null;
        private CentipedeAI _clingingToPlayer = null;

        protected void Start()
        {
            _playerController = gameObject.GetComponent<PlayerControllerB>();
            Plugin.Logger?.LogDebug($"\"{_playerController.playerUsername}\" applies \"{nameof(PlayerControllerBPatch)}\"");
        }

        public void OnClingToPlayer(CentipedeAI centipedeAI)
        {
            Plugin.Logger?.LogDebug($"\"{EntitiesName.SnareFlea}\" has latched on player \"{_playerController?.playerUsername}\"");
            _clingingToPlayer = centipedeAI;
        }

        public void OnStopClingingToPlayer()
        {
            Plugin.Logger?.LogDebug($"\"{EntitiesName.SnareFlea}\" has dropped from player \"{_playerController?.playerUsername}\"");
            _clingingToPlayer = null;
        }

        public bool IsClingingToPlayer()
        {
            return _clingingToPlayer != null && !_clingingToPlayer.isEnemyDead;
        }

        [HarmonyPatch(nameof(PlayerControllerB.Awake))]
        [HarmonyPostfix]
        private static void PlayerControllerBAwakePostfix(PlayerControllerB __instance)
        {
            __instance.gameObject.AddComponent<PlayerControllerBPatch>();
        }
    }
}