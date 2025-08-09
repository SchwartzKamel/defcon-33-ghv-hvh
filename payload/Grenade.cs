using BNB;
using SmallFishUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    public static class Grenade
    {
        public static List<BNB.Player> GrenadeTargets = [];
        public static TimeSince LastGrenadeSpawn = 0;

        public static void OnUpdate()
        {
            if (LastGrenadeSpawn < 5 || GrenadeTargets.Count == 0)
                return;

            var local = Utils.GetLocalPlayer();

            if (local == null || !local.IsAlive || !local.IsValid())
                return;

            foreach(var p in GrenadeTargets)
            {
                if (p == null || p.Unit == null || !p.IsAlive || !p.IsValid())
                    continue;

                var grenade_pos = p.Unit.GetHitboxPosition(HitboxLimb.Head);

                //grenade_pos += (Vector3.Up * 10f);

                //local.ReleaseGrenade(grenade_pos);

                Vector3 dir = (Vector3.Up + Vector3.Random.WithZ(0f)).Normal;
                Vector3 vel = dir * BNB.Random.Float((float)700f * 0.2f, (float)700f);
                SpawnNade(grenade_pos + dir * 32f, vel);
            }

            LastGrenadeSpawn = 0;
        }

        public static void SpawnNade(Vector3 pos, Vector3 velocity)
        {
            GameObject grenadePrefab = Prefab.Get("prefabs/projectiles/grenade.prefab");

            GameObject obj = ((grenadePrefab != null) ? grenadePrefab.Clone() : null);
            if (!obj.IsValid())
            {
                return;
            }
            PhysicsExplosive nade = obj.Components.Get<PhysicsExplosive>(true);
            if (!nade.IsValid())
            {
                obj.Destroy();
                return;
            }
            nade.ExplodeCooldown = BNB.Random.Float(1f, 2f);
            nade.PlayerDamage = 200f;
            nade.Velocity = velocity;
            nade.BounceVelocityLoss = 0f;
            obj.WorldPosition = pos;
            obj.Transform.ClearInterpolation();
            obj.Enabled = true;
            obj.SetupNetworking(Connection.Local, OwnerTransfer.Fixed, NetworkOrphaned.Host);
        }
    }
}


// ReleaseGrenade(new Vector3?(base.WorldPosition + Vector3.Up * 30f));