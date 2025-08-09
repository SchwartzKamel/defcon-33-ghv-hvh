
using HarmonyLib;

namespace payload;

static public class Addon
{

    private static Harmony harmony = new Harmony("com.defcon33.gamepatch");

    //[ModuleInitializer]
    //public static void Init()
    //{

    //}

    public static void OnLoad()
    {
        Utils.Log("Addon::OnLoad() Executed");

        Utils.EnableDevMode();

        harmony.PatchAll();

        Utils.Log("Patches applied, enjoy!");
        
    }

    public static void OnReload()
    {
        Utils.Log("Addon::OnReload() Executed");

        Utils.EnableDevMode();

        harmony.PatchAll();

        Utils.Log("Patches applied, enjoy!");
    }

    public static void OnDestroy()
    {
        // com.defcon33.gamepatch
        // com.defcon33.sandboxpatch
        harmony.UnpatchAll("com.defcon33.gamepatch");
        Log.Info("[defcon33] Unpatched all hooks, unloading...");
    }
}

