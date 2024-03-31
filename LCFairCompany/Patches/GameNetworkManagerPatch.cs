using HarmonyLib;
using LCFairCompany.Configs;

namespace LCFairCompany.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch(nameof(GameNetworkManager.SteamMatchmaking_OnLobbyMemberJoined))]
        [HarmonyPostfix]
        private static void SteamMatchmaking_OnLobbyMemberJoinedPostfix()
        {
            if (ConfigManager.IsHost)
            {
                ConfigManager.MessageManager.RegisterNamedMessageHandler($"{PluginInfo.PLUGIN_NAME}_OnRequestConfigSync", ConfigManager.OnRequestSync);
                ConfigManager.Synced = true;

                return;
            }

            ConfigManager.Synced = false;
            ConfigManager.MessageManager.RegisterNamedMessageHandler($"{PluginInfo.PLUGIN_NAME}_OnReceiveConfigSync", ConfigManager.OnReceiveSync);
            ConfigManager.RequestSync();
        }

        [HarmonyPatch(nameof(GameNetworkManager.StartDisconnect))]
        [HarmonyPostfix]
        public static void StartDisconnectPostfix()
        {
            ConfigManager.RevertSync();
        }
    }
}