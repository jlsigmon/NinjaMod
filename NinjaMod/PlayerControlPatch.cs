using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace NinjaMod
{
    enum CustomRPC
    {

        SetNinja = 40,
        SyncCustomSettings = 41,
        NinjaInvis = 42,
        NinjaUninvis = 43

    }
    enum RPC
    {

        PlayAnimation = 0,
        CompleteTask = 1,
        SyncSettings = 2,
        SetInfected = 3,
        Exiled = 4,
        CheckName = 5,
        SetName = 6,
        CheckColor = 7,
        SetColor = 8,
        SetHat = 9,
        SetSkin = 10,
        ReportDeadBody = 11,
        MurderPlayer = 12,
        SendChat = 13,
        StartMeeting = 14,
        SetScanner = 15,
        SendChatNote = 16,
        SetPet = 17,
        SetStartCounter = 18,
        EnterVent = 19,
        ExitVent = 20,
        SnapTo = 21,
        Close = 22,
        VotingComplete = 23,
        CastVote = 24,
        ClearVote = 25,
        AddVote = 26,
        CloseDoorsOfType = 27,
        RepairSystem = 28,
        SetTasks = 29,
        UpdateGameData = 30,


    }

    [HarmonyPatch(typeof(PlayerControl))]
    public static class PlayerControlPatch
    {
        public static PlayerControl closestPlayer = null;
        public static PlayerControl Ninja;
        public static bool NinjaInTask;
        public static bool NinjaInAdmin;
        public static DateTime lastKilled;
        
        [HarmonyPatch(nameof(PlayerControl.HandleRpc))]
        public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            try
            {
                switch (callId)
                {
                    case (byte)CustomRPC.SetNinja:
                        {
                            byte NinjaId = reader.ReadByte();
                            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                            {
                                if (player.PlayerId == NinjaId)
                                {
                                    Ninja = player;
                                    if (CustomGameOptions.ShowNinja)
                                    {
                                        player.nameText.Color = new Color((float)0.5, (float)0.5, (float)0.5, 1);
                                    }
                                }
                            }
                            
                            break;
                        }
                    
                    case (byte)CustomRPC.SyncCustomSettings:
                        {
                            CustomGameOptions.ShowNinja = reader.ReadBoolean();
                            CustomGameOptions.NinjaInvisCD = BitConverter.ToSingle(reader.ReadBytes(4).ToArray(), 0);
                            break;
                        }
                    
                    case (byte)CustomRPC.NinjaInvis:
                        {
                            byte ninja = reader.ReadByte();
                            
                            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                            {
                                if (player.PlayerId == ninja)
                                {
                                    Ninja = player;
                                    goInvis(player);
                                }
                            }
                            break;
                        }

                    case (byte)CustomRPC.NinjaUninvis:
                        {
                            byte ninja = reader.ReadByte();
                            
                            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                            {
                                if (player.PlayerId == ninja)
                                {
                                    Ninja = player;
                                    goUninvis(player);
                                }
                            }
                            break;
                        }
                }
            }
            catch {
                NinjaMod.log.LogInfo("RPC error... possible reasons: Not all players in the lobby have installed the mod or Ninja mod versions do not match");
            }
        }

        public static bool isNinja(PlayerControl player)
        {
            if (Ninja == null)
                return false;
            
            return player.PlayerId == Ninja.PlayerId;
        }

        public static void goInvis(PlayerControl player)
        {
            var color = Color.clear;
            if (PlayerControlPatch.isNinja(PlayerControl.LocalPlayer) || PlayerControl.LocalPlayer.Data.IsDead)
            {
                color.a = 0.1f;
            }
            player.GetComponent<SpriteRenderer>().color = color;

            player.HatRenderer.SetHat(0, 0);
            player.nameText.Text = "";
            PlayerControl.AllPlayerControls.Remove(player);
            if (player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[0].ProdId)
            {
                player.MyPhysics.SetSkin(0);
            }
            if (player.CurrentPet != null)
            {
                UnityEngine.Object.Destroy(player.CurrentPet.gameObject);
            }
            player.CurrentPet =
                UnityEngine.Object.Instantiate(
                    DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
            player.CurrentPet.transform.position = player.transform.position;
            player.CurrentPet.Source = player;
            player.CurrentPet.Visible = player.Visible;
        }

        public static void goUninvis(PlayerControl player)
        {
            player.GetComponent<SpriteRenderer>().color = Color.white;

            var colorId = player.Data.ColorId;
            player.nameText.Text = player.Data.PlayerName;
            PlayerControl.SetPlayerMaterialColors(colorId, player.myRend);
            player.HatRenderer.SetHat(player.Data.HatId, colorId);
                    

            if (player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[(int) player.Data.SkinId].ProdId)
            {
                //SetSkin(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data.SkinId);
                player.MyPhysics.SetSkin(player.Data.SkinId);
            }


            if (player.CurrentPet != null)
            {
                UnityEngine.Object.Destroy(player.CurrentPet.gameObject);
            }
                    
            player.CurrentPet =
                UnityEngine.Object.Instantiate(
                    DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int) player.Data.PetId]);
            player.CurrentPet.transform.position = player.transform.position;
            player.CurrentPet.Source = player;
            player.CurrentPet.Visible = player.Visible;


            PlayerControl.SetPlayerMaterialColors(colorId, player.CurrentPet.rend);
        }

        public static void changeTaskState(bool active)
        {
            if (isNinja(PlayerControl.LocalPlayer) && NinjaInTask != active)
            {
                NinjaInTask = active;
            }
        }

        public static void changeAdminState(bool active)
        {
            if (isNinja(PlayerControl.LocalPlayer) && NinjaInAdmin != active)
            {
                NinjaInAdmin = active;
            }
        }

        public static PlayerControl getPlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }
            
            return null;
        }
        public static float NinjaKillTimer()
        {
            if (lastKilled == null)
                return 0;
            
            DateTime now = DateTime.UtcNow;
            TimeSpan diff = now - lastKilled;
            var KillCoolDown = CustomGameOptions.NinjaInvisCD * 1000.0f;
            
            if (KillCoolDown - (float)diff.TotalMilliseconds < 0)
                return 0;
            
            return (KillCoolDown - (float)diff.TotalMilliseconds) / 1000.0f;
        }
        public static List<PlayerControl> getCrewMates(Il2CppReferenceArray<GameData.PlayerInfo> infection)
        {
            List<PlayerControl> CrewmateIds = new List<PlayerControl>();
            
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {

                bool isInfected = false;
                foreach (GameData.PlayerInfo infected in infection)
                {

                    if (player.PlayerId == infected.Object.PlayerId)
                    {
                        isInfected = true;

                        break;
                    }

                }
                if (!isInfected)
                {
                    CrewmateIds.Add(player);
                }
            }
            
            return CrewmateIds;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            double mindist = double.MaxValue;
            PlayerControl closestplayer = null;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead || player.inVent)
                    continue;

                if (player.PlayerId != refplayer.PlayerId)
                {

                    double dist = getDistBetweenPlayers(player, refplayer);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closestplayer = player;
                    }

                }

            }
            return closestplayer;

        }

        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var refpos = refplayer.GetTruePosition();
            var playerpos = player.GetTruePosition();

            return Math.Sqrt((refpos[0] - playerpos[0]) * (refpos[0] - playerpos[0]) + (refpos[1] - playerpos[1]) * (refpos[1] - playerpos[1]));
        }

        [HarmonyPatch(nameof(PlayerControl.RpcSetInfected))]
        public static void Postfix([HarmonyArgument(0)] Il2CppReferenceArray<GameData.PlayerInfo> PlayerInfos)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetNinja, Hazel.SendOption.None, -1);
            List<PlayerControl> crewmates = getCrewMates(PlayerInfos);
            
            System.Random r = new System.Random();
            Ninja = crewmates[r.Next(0, crewmates.Count)];
            
            if (CustomGameOptions.ShowNinja)
            {
                Ninja.nameText.Color = new Color(1, (float)(204.0 / 255.0), 0, 1);
            }
            
            byte NinjaId = Ninja.PlayerId;

            writer.Write(NinjaId);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        [HarmonyPatch(nameof(PlayerControl.MurderPlayer))]
        public static bool Prefix(PlayerControl __instance)
        {
            if (Ninja != null)
            {
                if (__instance.PlayerId == Ninja.PlayerId)
                {
                    __instance.Data.IsImpostor = true;

                }
            }
            return true;
        }

        [HarmonyPatch(nameof(PlayerControl.MurderPlayer))]
        public static void Postfix(PlayerControl __instance)
        {
            if (Ninja != null)
            {
                if (__instance.PlayerId == Ninja.PlayerId)
                {
                    __instance.Data.IsImpostor = false;
                }
            }
        }

        [HarmonyPatch(nameof(PlayerControl.RpcSyncSettings))]
        public static void Postfix([HarmonyArgument(0)] GameOptionsData gameOptionsData)
        {
            if (PlayerControl.AllPlayerControls.Count > 1)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncCustomSettings, Hazel.SendOption.None, -1);

                writer.Write(CustomGameOptions.ShowNinja);
                writer.Write(CustomGameOptions.NinjaInvisCD);
                
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

        }
    }
}