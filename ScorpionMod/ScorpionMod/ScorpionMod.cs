using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace ScorpionMod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class ScorpionMod : BasePlugin
    {
        public const string Id = "scorpion.mod";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static BepInEx.Logging.ManualLogSource log;

        public ScorpionMod()
        {
            log = Log;
        }

        public override void Load()
        {
            log.LogMessage("Scorpion Mod loaded");

            Harmony.PatchAll();
        }

    }
}
