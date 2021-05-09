using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Reflection;
using System;
namespace ScorpionMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class ExampleButton
    {
        private static CooldownButton btn;
        

        public static void Postfix(HudManager __instance)
        {
            btn = new CooldownButton(
                () =>
                {
                    var dist = PlayerControlPatch.getDistBetweenPlayers(PlayerControl.LocalPlayer, PlayerControlPatch.closestPlayer);
                     if (dist < 2f)
                    {
                        // Do cool stuff when the button is pressed
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ScorpionKill, Hazel.SendOption.None, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(PlayerControlPatch.closestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        PlayerControlPatch.closestPlayer.transform.position = PlayerControl.LocalPlayer.transform.position;
                        PlayerControl.LocalPlayer.MurderPlayer(PlayerControlPatch.closestPlayer);
                    }
                    PlayerControlPatch.lastKilled = DateTime.UtcNow;
                },

                30f, // The cooldown for this button is five seconds

                Properties.Resources.killbutton, // change yournamehere to the name you set in step 2
                new Vector2(0.125f, 0.125f), // The position of the button, 1 unit is 100 pixels
                () => 
                {
                    // Who has access to the button? This allows alive crewmates to use the new button while the game is started
                    return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControlPatch.isScorpion(PlayerControl.LocalPlayer) && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
                },
                __instance
            );
        }
    }
}