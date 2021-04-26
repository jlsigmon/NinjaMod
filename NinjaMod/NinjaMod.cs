using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace NinjaMod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class NinjaMod : BasePlugin
    {
        public const string Id = "me.change.please";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static BepInEx.Logging.ManualLogSource log;

        public NinjaMod()
        {
            log = Log;
        }

        public override void Load()
        {
            log.LogMessage("Ninja Mod loaded");

            

            Harmony.PatchAll();
        }

    }
}
