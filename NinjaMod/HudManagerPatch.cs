using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;


namespace NinjaMod
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
                if (CustomGameOptions.ShowNinja)
                    __instance.GameSettings.Text = GameSettingsText + "Show Ninja: On" + "\n";
                else
                    __instance.GameSettings.Text = GameSettingsText + "Show Ninja: Off" + "\n";
                
                __instance.GameSettings.Text += "Ninja Invisibility Cooldown: " + CustomGameOptions.NinjaInvisCD.ToString() + "s";
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
                if (PlayerControlPatch.Ninja != null && playerVoteArea.NameText.Text == PlayerControlPatch.Ninja.name)
                {
                    if (CustomGameOptions.ShowNinja | PlayerControlPatch.isNinja(PlayerControl.LocalPlayer))
                    {
                        playerVoteArea.NameText.Color = new Color((float)(0.5), (float)(0.5), (float)(0.5), 1);
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
      
            if (PlayerControl.AllPlayerControls.Count > 1 & PlayerControlPatch.Ninja != null)
            {
                if (PlayerControlPatch.isNinja(PlayerControl.LocalPlayer))
                {

                    PlayerControl.LocalPlayer.nameText.Color = new Color((float)(0.5), (float)(0.5), (float)(0.5), 1);
                    
                    
                    
                }
            }

            if (counter < 30)
            {
                counter++;
                return;
            }
            counter = 0;

            if (GameOptionsMenuPatch.ShowNinjaOption != null && GameOptionsMenuPatch.NinjaInvisCooldown!=null)
            {
                var isOptionsMenuActive = GameObject.FindObjectsOfType<GameOptionsMenu>().Count != 0;
                GameOptionsMenuPatch.ShowNinjaOption.gameObject.SetActive(isOptionsMenuActive);
                GameOptionsMenuPatch.NinjaInvisCooldown.gameObject.SetActive(isOptionsMenuActive);
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