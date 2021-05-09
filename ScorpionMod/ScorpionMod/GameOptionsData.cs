using HarmonyLib;
using System;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace ScorpionMod
{
    [HarmonyPatch(typeof(GameOptionsMenu))]
    public static class GameOptionsMenuPatch
    {
        public static ToggleOption ShowScorpionOption;
        public static NumberOption ScorpionInvisCooldown;
        public static float LowestY;

        static float GetLowestConfigY(GameOptionsMenu __instance)
        {
            return __instance.GetComponentsInChildren<OptionBehaviour>()
                .Min(option => option.transform.localPosition.y);
        }

        public static void PositionElement(OptionBehaviour element)
        {
            LowestY -= 0.5f;
            element.transform.localPosition = new Vector3(element.transform.localPosition.x, LowestY,
                element.transform.localPosition.z);
        }

        public static ToggleOption PrepareToggle(GameOptionsMenu instance, string title, bool enabled)
        {
            ToggleOption toggle = UnityEngine.Object.Instantiate(instance.GetComponentsInChildren<ToggleOption>().Last(),
                instance.transform);
            PositionElement(toggle);
            toggle.TitleText.Text = title;
            toggle.CheckMark.enabled = enabled;

            return toggle;
        }

        public static NumberOption PrepareNumberOption(GameOptionsMenu instance, string title, float value)
        {
            NumberOption option = UnityEngine.Object.Instantiate(instance.GetComponentsInChildren<NumberOption>().Last(),
                instance.transform);

            PositionElement(option);
            option.gameObject.name = title;
            option.TitleText.Text = title;
            option.Value = value;
            option.ValueText.Text = value.ToString();

            return option;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameOptionsMenu.Start))]
        public static void Postfix1(GameOptionsMenu __instance)
        {
            if (GameObject.FindObjectsOfType<ToggleOption>().Count == 4)
            {

                LowestY = GetLowestConfigY(__instance);
                
                System.Collections.Generic.List<OptionBehaviour> NewOptions = __instance.Children.ToList();

                ShowScorpionOption = PrepareToggle(__instance, "Show Scorpion", CustomGameOptions.ShowScorpion);

                ScorpionInvisCooldown =
                    PrepareNumberOption(__instance, "Scorpion Kill Cooldown", CustomGameOptions.ScorpionInvisCD);

                
                NewOptions.Add(ShowScorpionOption);
                NewOptions.Add(ScorpionInvisCooldown);
                
                __instance.GetComponentInParent<Scroller>().YBounds.max +=
                    0.5f * (NewOptions.Count - __instance.Children.Count);

                __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(NewOptions.ToArray());
            }
        }
    }
    
    [HarmonyPatch(typeof(ToggleOption))]
    public static class ToggleButtonPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ToggleOption.Toggle))]
        public static bool Prefix(ToggleOption __instance)
        {
            if (__instance.TitleText.Text == GameOptionsMenuPatch.ShowScorpionOption.TitleText.Text)
            {
                CustomGameOptions.ShowScorpion = !CustomGameOptions.ShowScorpion;
                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

                __instance.oldValue = CustomGameOptions.ShowScorpion;
                __instance.CheckMark.enabled = CustomGameOptions.ShowScorpion;
                
                return false;
            }
            
            return true;
        }

    }
    
    [HarmonyPatch(typeof(NumberOption))]
    public static class NumberOptionPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NumberOption.Increase))]
        public static bool Prefix1(NumberOption __instance)
        {
            if (__instance.TitleText.Text == GameOptionsMenuPatch.ScorpionInvisCooldown.TitleText.Text)
            {
                CustomGameOptions.ScorpionInvisCD = Math.Min(CustomGameOptions.ScorpionInvisCD + 2.5f, 45);
                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
                __instance.oldValue = CustomGameOptions.ScorpionInvisCD;
                __instance.Value = CustomGameOptions.ScorpionInvisCD;
                __instance.ValueText.Text = CustomGameOptions.ScorpionInvisCD.ToString();
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(NumberOption.Decrease))]
        public static bool Prefix2(NumberOption __instance)
        {
            if (__instance.TitleText.Text == GameOptionsMenuPatch.ScorpionInvisCooldown.TitleText.Text)
            {
                CustomGameOptions.ScorpionInvisCD = Math.Max(CustomGameOptions.ScorpionInvisCD - 2.5f, 10);

                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
                __instance.oldValue = CustomGameOptions.ScorpionInvisCD;
                __instance.Value = CustomGameOptions.ScorpionInvisCD;
                __instance.ValueText.Text = CustomGameOptions.ScorpionInvisCD.ToString();
                
                return false;
            }

            return true;
        }
    }
}
