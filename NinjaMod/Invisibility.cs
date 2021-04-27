using Reactor.Extensions;
using Reactor.Unstrip;
using Reactor.Button;
using UnityEngine;
using HarmonyLib;
namespace NinjaMod
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
                    // Do cool stuff when the button is pressed
                },
                5f, // The cooldown for this button is five seconds
                Properties.Resources.demo_invis_button, // change yournamehere to the name you set in step 2
                new Vector2(0.125f, 0.125f), // The position of the button, 1 unit is 100 pixels
                () => 
                {
                    // Who has access to the button? This allows alive crewmates to use the new button while the game is started
                    return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControlPatch.isNinja(PlayerControl.LocalPlayer) && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
                },
                __instance
            );
        }
    }
}