using Reactor.Extensions;
using Reactor.Unstrip;
using Reactor.Button;
using UnityEngine;
using HarmonyLib;
namespace YourCoolMod
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
                Properties.Resources.yournamehere, // change yournamehere to the name you set in step 2
                new Vector2(0.125f, 0.125f), // The position of the button, 1 unit is 100 pixels
                () => 
                {
                    // Who has access to the button? This allows alive crewmates to use the new button while the game is started
                    return !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsImpostor && (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
                },
                __instance
            );
        }
    }
}