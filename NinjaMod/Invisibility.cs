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
                    var color = Color.clear;
                    if (PlayerControl.LocalPlayer.Data.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        color.a = 0.1f;
                    }
                    PlayerControl.LocalPlayer.GetComponent<SpriteRenderer>().color = color;

                    PlayerControl.LocalPlayer.HatRenderer.SetHat(0, 0);
                    PlayerControl.LocalPlayer.nameText.Text = "";
                    if (PlayerControl.LocalPlayer.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                        .AllSkins.ToArray()[0].ProdId)
                    {
                        PlayerControl.LocalPlayer.MyPhysics.SetSkin(0);
                    }
                    if (PlayerControl.LocalPlayer.CurrentPet != null)
                    {
                        UnityEngine.Object.Destroy(PlayerControl.LocalPlayer.CurrentPet.gameObject);
                    }
                    PlayerControl.LocalPlayer.CurrentPet =
                        UnityEngine.Object.Instantiate(
                            DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
                    PlayerControl.LocalPlayer.CurrentPet.transform.position = PlayerControl.LocalPlayer.transform.position;
                    PlayerControl.LocalPlayer.CurrentPet.Source = PlayerControl.LocalPlayer;
                    PlayerControl.LocalPlayer.CurrentPet.Visible = PlayerControl.LocalPlayer.Visible;
                },
<<<<<<< Updated upstream
                CustomGameOptions.NinjaInvisCD, // The cooldown for this button is How many seconds set in the game lobby settings
=======
                PlayerControlPatch.NinjaKillTimer() * 1000f, // The cooldown for this button is five seconds
>>>>>>> Stashed changes
                Properties.Resources.demo_invis_button, // change yournamehere to the name you set in step 2
                new Vector2(0.125f, 0.125f), // The position of the button, 1 unit is 100 pixels
                () => 
                {
                    // Who has access to the button? This allows alive crewmates to use the new button while the game is started
                    return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControlPatch.isNinja(PlayerControl.LocalPlayer) && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
                },
                __instance,
                10f,
                () =>
                {
                    PlayerControl.LocalPlayer.GetComponent<SpriteRenderer>().color = Color.white;

                    var colorId = PlayerControl.LocalPlayer.Data.ColorId;
                    PlayerControl.LocalPlayer.nameText.Text = PlayerControl.LocalPlayer.Data.PlayerName;
                    PlayerControl.SetPlayerMaterialColors(colorId, PlayerControl.LocalPlayer.myRend);
                    PlayerControl.LocalPlayer.HatRenderer.SetHat(PlayerControl.LocalPlayer.Data.HatId, colorId);
                    

                    if (PlayerControl.LocalPlayer.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                        .AllSkins.ToArray()[(int) PlayerControl.LocalPlayer.Data.SkinId].ProdId)
                    {
                        //SetSkin(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data.SkinId);
                        PlayerControl.LocalPlayer.MyPhysics.SetSkin(PlayerControl.LocalPlayer.Data.SkinId);
                    }


                    if (PlayerControl.LocalPlayer.CurrentPet != null)
                    {
                        Object.Destroy(PlayerControl.LocalPlayer.CurrentPet.gameObject);
                    }

                    PlayerControl.LocalPlayer.CurrentPet =
                        Object.Instantiate(
                            DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int) PlayerControl.LocalPlayer.Data.PetId]);
                    PlayerControl.LocalPlayer.CurrentPet.transform.position = PlayerControl.LocalPlayer.transform.position;
                    PlayerControl.LocalPlayer.CurrentPet.Source = PlayerControl.LocalPlayer;
                    PlayerControl.LocalPlayer.CurrentPet.Visible = PlayerControl.LocalPlayer.Visible;


                    PlayerControl.SetPlayerMaterialColors(colorId, PlayerControl.LocalPlayer.CurrentPet.rend);
                }
            );
        }
    }
}