using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    internal class Loop
    {
        private static bool first_tick = false;

        public static void Main()
        {
            if(!first_tick)
            {
                first_tick = true;

                Log.Info("[Harmony] OnUpdate Postfix tick");
            }


            ESP.OnUpdate();
            Aimbot.OnUpdate();
            //Ghost.OnUpdate();
            Grenade.OnUpdate();
            Judge.OnUpdate();
        }
    }
}
