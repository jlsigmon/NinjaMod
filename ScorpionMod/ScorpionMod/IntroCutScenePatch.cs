using HarmonyLib;
using UnityEngine;

namespace ScorpionMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__11))]
    public static class IntroCutScenePatch
    {
        [HarmonyPatch(nameof(IntroCutscene._CoBegin_d__11.MoveNext))]
        public static void Postfix(IntroCutscene._CoBegin_d__11 __instance)
        {
            if (PlayerControlPatch.isScorpion(PlayerControl.LocalPlayer))
            {
                __instance.__this.Title.Text = "Scorpion";
                __instance.__this.Title.Color = new Color((float)1, (float)0.92, (float)0.016, 1);
                __instance.__this.ImpostorText.Text = "Get Over Here";
                __instance.__this.BackgroundBar.material.color = new Color((float)1, (float)0.92, (float)0.016, 1);

            }
        }
    }
}