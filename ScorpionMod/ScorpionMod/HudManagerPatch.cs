using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;


namespace ScorpionMod
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerPatch
    {
        // Methods
        
        
        static int counter = 0;

        public static KillButtonManager KillButton = null;
        static System.Random random = new System.Random();
        static string GameSettingsText = null;

        public static bool isMeetingHudActive;


      
        public static void UpdateGameSettingsText(HudManager __instance)
        {
            if (__instance.GameSettings.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Count() == 19)
            {
                GameSettingsText = __instance.GameSettings.Text;
            }
            
            if (GameSettingsText != null)
            {
                if (CustomGameOptions.ShowScorpion)
                    __instance.GameSettings.Text = GameSettingsText + "Show Scorpion: On" + "\n";
                else
                    __instance.GameSettings.Text = GameSettingsText + "Show Scorpion: Off" + "\n";
                
                __instance.GameSettings.Text += "Scorpion Kill Cooldown: " + CustomGameOptions.ScorpionInvisCD.ToString() + "s";
            }

        }
        public static void updateGameOptions(GameOptionsData options)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> allplayer = PlayerControl.AllPlayerControls;

            foreach (PlayerControl player in allplayer)
            {
                player.RpcSyncSettings(options);

            }

        }

        public static void updateMeetingHud(MeetingHud __instance)
        {
            foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
            {
                if (PlayerControlPatch.Scorpion != null && playerVoteArea.NameText.Text == PlayerControlPatch.Scorpion.name)
                {
                    if (CustomGameOptions.ShowScorpion | PlayerControlPatch.isScorpion(PlayerControl.LocalPlayer))
                    {
                        playerVoteArea.NameText.Color = new Color((float)1, (float)0.92, (float)0.016, 1);
                    }
                }
            }
        }

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            isMeetingHudActive = MeetingHud.Instance != null;

            if (isMeetingHudActive)
            {
                updateMeetingHud(MeetingHud.Instance);
            }

            UpdateGameSettingsText(__instance);
      
            if (PlayerControl.AllPlayerControls.Count > 1 & PlayerControlPatch.Scorpion != null)
            {
                if (PlayerControlPatch.isScorpion(PlayerControl.LocalPlayer))
                {

                    PlayerControl.LocalPlayer.nameText.Color = new Color((float)1, (float)0.92, (float)0.016, 1);
                }
            }
            PlayerControlPatch.closestPlayer = PlayerControlPatch.getClosestPlayer(PlayerControl.LocalPlayer);

            if (counter < 30)
            {
                counter++;
                return;
            }
            counter = 0;

            if (GameOptionsMenuPatch.ShowScorpionOption != null && GameOptionsMenuPatch.ScorpionInvisCooldown!=null)
            {
                var isOptionsMenuActive = GameObject.FindObjectsOfType<GameOptionsMenu>().Count != 0;
                GameOptionsMenuPatch.ShowScorpionOption.gameObject.SetActive(isOptionsMenuActive);
                GameOptionsMenuPatch.ScorpionInvisCooldown.gameObject.SetActive(isOptionsMenuActive);
            }
        }

    }

    [HarmonyPatch(typeof(MeetingHud))]
    public static class MeetingHudPatchClose
    {

        [HarmonyPatch(nameof(MeetingHud.Close))]
        public static void Postfix(MeetingHud __instance)
        {
            PlayerControlPatch.lastKilled = DateTime.UtcNow;
            PlayerControlPatch.lastKilled = PlayerControlPatch.lastKilled.AddSeconds(8);
        }
    }
}