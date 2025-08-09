using BNB;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    public static class Aimbot
    {
        public static bool enabled = false;
        public static float AimFOV = 45f;
        public static bool RageMode = false;

        public static void OnUpdate()
        {
            var local = Utils.GetLocalPlayer();


            if(local == null || local.Unit == null || !local.IsAlive || !GameManager.CurrentState.Playing || !enabled) // || Settings.PayloadSettings?.Aimbot == null || !Settings.PayloadSettings.Aimbot)
            {
                return;
            }

            if (!local.Noclip)
                local.Noclip = true;

            var target_component = AcquireTarget(local);

            if(target_component == null)
            {
                
                return;
            }

            var target_head_pos = target_component.GetHitboxPosition(HitboxLimb.Head);

            Rotation perfectRotation = Rotation.LookAt(Vector3.Direction(local.Unit.EyePosition, in target_head_pos), Vector3.Up);

            //local.WorldRotation = perfectRotation;
            //Utils.Log("Have a target");

            //local.Controller.WorldRotation = perfectRotation;
            //local.WorldRotation = perfectRotation;

            Weapon? weapon = local.ActiveGear as Weapon;

            if (weapon == null)
                return;

            if (!weapon.CanFire)
            {
                return;
            }

            //Utils.Log(weapon._lastFired.ToString());
            //if (weapon._lastFired < 0.25f)
            //    return;

            //var oldAngles = local.EyeAngles;
            local.EyeAngles = perfectRotation.Angles();

            if(TargetIsUnderAim(local, target_component))
            {

                if(!RageMode)
                {
                    if (weapon.TryAttack())
                    {
                        //weapon.FireBullets(local.Unit);
                    }
                }
                else
                {
                    if (weapon._lastFired > 0.1)
                    {
                        weapon.FireBullets(local.Unit);
                        weapon._lastFired = 0;
                    }
                        
                }
                

                //local.EyeAngles = oldAngles;
            }
            
        }

        public static bool LineOfSight(Player local, Vector3 start, Vector3 end)
        {
            SceneTraceResult fromToTrace = local.Scene.Trace.Ray(in start, in end).WithoutTags(new string[] { "unit", "breakpiece", "gear", "noaudioblock", "trigger" }).IgnoreDynamic()
                .Run();
            SceneTraceResult toFromTrace = local.Scene.Trace.Ray(in end, in start).WithoutTags(new string[] { "unit", "breakpiece", "gear", "noaudioblock", "trigger" }).IgnoreDynamic()
                .Run();
            return !fromToTrace.Hit && !toFromTrace.Hit;
        }

        public static bool TargetInLineOfSight(Player local, UnitComponent target)
        {
            if (!target.IsValid() || !local.Unit.IsValid())
            {
                return false;
            }

            bool flag = LineOfSight(local, local.Unit.EyePosition, target.GetHitboxPosition(HitboxLimb.Head));
            bool torsoTrace = LineOfSight(local, local.Unit.EyePosition, target.GetHitboxPosition(HitboxLimb.Torso));

            //Utils.Log($"flag: {flag}, torso {torsoTrace}");
            return flag || torsoTrace;
        }

        public static UnitComponent? AcquireTarget(Player local, float vision_dist = 11000f)
        {
            foreach (UnitComponent found in (from x in local.Scene.FindInPhysics(new Sphere(local.WorldPosition, vision_dist))
                                             select x.Components.Get<UnitComponent>(FindMode.EverythingInSelfAndParent) into x
                                             where x.IsValid() && x.IsAlive
                                             select x).OrderBy(delegate (UnitComponent x)
                                             {
                                                 Vector3 worldPosition = x.WorldPosition;
                                                 Vector3 worldPosition2 = local.WorldPosition;
                                                 return worldPosition.Distance(in worldPosition2);
                                             }))
            {
                if (found != local.Unit)
                {
                    if (found.Team != Team.Unaligned)
                    {
                        Team team = found.Team;
                        UnitComponent unit = local.Unit;
                        if (team == ((unit != null) ? unit.Team : Team.Unaligned))
                        {
                            continue;
                        }
                    }

                    //Utils.Log("Have a target 1");
                    if (Utils.UnitInFOV(local.Unit, found, AimFOV))
                    {
                        //Utils.Log("Unit in FOV");
                        if(TargetInLineOfSight(local, found))
                        {
                            return found;
                        }
                        
                    }
                }
            }
            return null;
        }

        public static bool TargetIsUnderAim(Player local, UnitComponent target)
        {
            if (!local.Unit.IsValid())
            {
                return false;
            }

            //BBox targetHitbox = (this.AimingEnabled ? this.Target.Bounds : this.Target.GetHitboxBounds(this.AimingTarget));
            //if (this.AimingEnabled)
            //{
            //    float hitboxSize = MathF.Min(MathF.Min(targetHitbox.Size.x, targetHitbox.Size.y), targetHitbox.Size.z) / 2f;
            //    float sizeVariation = MathF.Sqrt(this.AimingAccuracy).Remap(1f, 3.16f, 5f, 0f) * hitboxSize;
            //    targetHitbox = targetHitbox.Grow(in sizeVariation);
            //}
            BBox targetHitbox = target.GetHitboxBounds(HitboxLimb.Head);
            Ray viewRay = local.Unit.ViewRay;
            float hitDistance;
            return targetHitbox.Trace(in viewRay, 100000f, out hitDistance);
        }
    }
}
