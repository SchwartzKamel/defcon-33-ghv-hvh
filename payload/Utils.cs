using BNB;
using Sandbox.Internal;
using SmallFishUtils;
using System;
using System.Reflection;
using static Sandbox.PhysicsContact;


namespace payload
{
    internal class Utils
    {

        public static void EnableDevMode()
        {
            Type? targetType = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "package.fish.blocks_and_bullets")
                ?.GetType("BNB.Utils");

            if (targetType == null)
            {
                Log("Target type is null");
                return;
            }


            FieldInfo? field = targetType?.GetField("_fishList", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
            {
                Log("Failed to get _fishList");
                return;
            }

            var fish_list = field.GetValue(null) as List<long>;

            if (fish_list != null && !fish_list.Contains(Connection.Local.SteamId.Value))
            {
                fish_list?.Add(Connection.Local.SteamId.Value);
                Log("ID added to fish list - enjoy your dev access!");
            }
        }

        public static void Log(string message, string level = "INFO")
        {
            Sandbox.Internal.GlobalGameNamespace.Log.Info($"[defcon33] [{level.ToUpper()}] {message}");
        }

        public static bool UnitInFOV(UnitComponent source, UnitComponent target, float detection_angle = 90f)
        {
            var targetPos = target.WorldPosition;
            var sourcePos = source.WorldPosition;
            var sourceForward = source.ViewRay.Forward;

            Vector3 toTarget = (targetPos - sourcePos).Normal;
            float dot = Vector3.Dot(sourceForward.Normal, toTarget);

            // Convert angle to cosine
            float cosThreshold = MathF.Cos(detection_angle * MathF.PI / 180f);
            return dot >= cosThreshold;
        }

        public static BNB.Player? GetPlayerByName(string name)
        {
            foreach(var c in GameManager.ValidClientsAndBots)
            {
                if (!c.IsValid())
                    continue;

                if (c.IsSpectator)
                    continue;

                if(c.DisplayName != null)
                    Utils.Log(c.DisplayName);

                if ((c.DisplayName != null && c.DisplayName.ToLower().Contains(name)))
                {
                    return c?.GetPawn<Player>();
                }
            }

            return null;
        }

        public static BNB.Player? GetLocalPlayer()
        {
            if (Client.Local == null)
                return null;

            return Client.Local?.GetPawn<Player>();
        }
    }
}



//public static void DebugTarget()
//{
//    if (!Utils.IsFish())
//    {
//        return;
//    }
//    GameObject man = PrefabLibrary.FindByComponent<TargetPracticeMan>().FirstOrDefault<ValueTuple<GameObject, PrefabFile>>().Item1;
//    Player player;
//    if (man != null && Client.Local.TryGetPawn<Player>(out player))
//    {
//        SceneTrace trace = Game.ActiveScene.Trace;
//        Ray ray = new Ray(player.Camera.WorldPosition, player.Camera.WorldRotation.Forward);
//        float num = 10000f;
//        SceneTraceResult playerRay = trace.Ray(in ray, in num).IgnoreGameObjectHierarchy(player.GameObject).IgnoreDynamic()
//            .Run();
//        if (!playerRay.Hit)
//        {
//            GlobalGameNamespace.Log.Info("Nowhere to spawn the NPC");
//            return;
//        }
//        GameObject gameObject = man.Clone(playerRay.HitPosition, Rotation.LookAt(playerRay.Direction.WithZ(0f)));
//        TargetPracticeMan component = gameObject.GetComponent<TargetPracticeMan>(false);
//        component.StartingPosition = playerRay.HitPosition;
//        component.StartingRotation = Rotation.LookAt(playerRay.Direction.WithZ(0f));
//        gameObject.NetworkSpawn();
//    }
//}


//public void SpawnNade(Vector3 pos, Vector3 vel)
//{
//    GameObject grenadePrefab = this.GrenadePrefab;
//    GameObject obj = ((grenadePrefab != null) ? grenadePrefab.Clone() : null);
//    if (!obj.IsValid())
//    {
//        return;
//    }
//    PhysicsExplosive nade = obj.Components.Get<PhysicsExplosive>(true);
//    if (!nade.IsValid())
//    {
//        obj.Destroy();
//        return;
//    }
//    nade.ExplodeCooldown = Random.Float(1f, 2f);
//    nade.PlayerDamage = 200f;
//    nade.Velocity = vel;
//    nade.BounceVelocityLoss = 0f;
//    obj.WorldPosition = pos;
//    obj.Transform.ClearInterpolation();
//    obj.Enabled = true;
//    obj.SetupNetworking(Connection.Local, OwnerTransfer.Fixed, NetworkOrphaned.Host);
