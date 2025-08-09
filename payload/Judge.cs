using BNB;
using BNB.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    public static class Judge
    {
        public static List<BNB.Player> JudgeTargets = [];
        //public static TimeSince LastGrenadeSpawn = 0;

        public static void OnUpdate()
        {
            var local = Utils.GetLocalPlayer();

            if (local == null || !local.IsAlive || !local.IsValid())
                return;

            foreach (var p in JudgeTargets)
            {
                if (p == null || p.Unit == null || !p.IsAlive || !p.IsValid())
                    continue;

                ChatCommands.KillPlayer(p);
            }
        }
    }
}


// ReleaseGrenade(new Vector3?(base.WorldPosition + Vector3.Up * 30f));