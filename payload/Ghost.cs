using BNB;
using SmallFishUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    public static class Ghost
    {
        public static bool enabled = false;
        public static bool IsGhost = false;
        public static TimeSince _ghosted = new TimeSince();

        public static void OnUpdate()
        {
            if (!enabled)
                return;

            var local = Utils.GetLocalPlayer();


            if (local == null || local.Unit == null || !local.IsAlive || !GameManager.CurrentState.Playing || _ghosted < 5f)
            {
                return;
            }

            bool shouldGhost = false;

            foreach (var p in ESP.Enemies)
            {
                if (!p.IsValid() || !p.IsAlive)
                    continue;

                if(Utils.UnitInFOV(p.Unit, local.Unit, 30f))
                {
                    shouldGhost = true;
                    break;
                }
            }

            if(shouldGhost && !IsGhost)
            {
                IsGhost = true;
                Utils.Log("In an FOV");
                GhostMode(local, true);
                _ghosted = 0f;
            }
            else if(IsGhost && !shouldGhost)
            {
                IsGhost = false;
                Utils.Log("Not in an FOV");
                GhostMode(local, false);
            }
        }

        public static void GhostMode(Player local, bool enabled)
        {
            try
            {
                // Load the assembly
                Assembly gameAssembly = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "package.fish.blocks_and_bullets");

                if (gameAssembly == null)
                {
                    Log.Warning("Assembly not found: package.fish.blocks_and_bullets");
                    return;
                }

                // Get the target type
                Type gameManagerType = gameAssembly.GetType("BNB.GameManager");
                if (gameManagerType == null)
                {
                    Log.Warning("Type not found: BNB.GameManager");
                    return;
                }

                // Get the method via reflection
                MethodInfo method = gameManagerType.GetMethod("DebugInvisible", BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                {
                    Log.Warning("Method not found: DebugInvisible");
                    return;
                }

                // Call the method
                method.Invoke(null, new object[] { enabled });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to call DebugInvisible: {ex}");
            }
        }
    }
}
