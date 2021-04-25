using HarmonyLib;
using GameOptionsData = IGDMNKLDEPI;
namespace NinjaMod
{
    [HarmonyPatch(typeof(GameOptionsData))]
    public class GameOptionsDataPatch
    {


        //Update lobby options text
        [HarmonyPostfix]
        [HarmonyPatch("PGDIEOCAHEK")]
        public static void Postfix1(GameOptionsData __instance, ref string __result, int CCKLGJKEEDP)
        {
            if (CustomGameOptions.showNinja)
               __result +=  "Show Ninja: On" + "\n";
            else
               __result+="Show Ninja: Off" + "\n";
           __result+= "Ninja Kill Cooldown: " + CustomGameOptions.NinjaKillCD.ToString() + "s";

        }

       
    }
    
    
}