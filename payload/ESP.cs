using BNB;
using BNB.UI;
using Sandbox;
using Sandbox.Internal;
using System.Runtime.CompilerServices;

namespace payload
{
    public class ESP
    {
        // Client player
        // player.GetPawn<Player>()

        public static List<Player> Enemies = new List<Player>();
        public static List<Player> Friendlies = new List<Player>();

        public static void OnUpdate()
        {


            //using (Gizmo.Scope(null))
            //{
            //    Gizmo.Draw.LineThickness = 2f;
            //    Gizmo.Draw.Color = Gizmo.Colors.Selected.WithAlpha(0.33f);
            //    Gizmo.Draw.LineBBox(in bounds);
            //}

            var local = Utils.GetLocalPlayer();

            

            if (local == null || !local.IsAlive)
            {
                //Log.Info("LP null");
                return;
            }

            Enemies.Clear();
            Friendlies.Clear();

            var state = GameManager.CurrentState;

            foreach (var p in BNB.Player.GetAllLiving())
            {
                if (local == p || !p.IsValid())
                    continue;

                if (state.Name.ToLower() == "free for all" || state.Name.ToLower().Contains("gib") || state.Name.ToLower().Contains("gun game"))
                {
                    Enemies.Add(p);
                }

                else
                {
                    if (local.Team != BNB.Team.Unaligned && p.Team != local.Team)
                    {
                        // enemies
                        // Log.Info(p.ToString());

                        //Log.Info(Utils.UnitInFOV(local.Unit, p.Unit))

                        Enemies.Add(p);


                        //Vector3Int position = VoxelMap.WorldToVoxel(p.WorldPosition);
                        //using (Gizmo.Scope(null))
                        //{
                        //    Gizmo.Draw.Color = Color.White;
                        //    Gizmo.GizmoDraw draw = Gizmo.Draw;
                        //    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
                        //    defaultInterpolatedStringHandler.AppendLiteral("POS: ");
                        //    defaultInterpolatedStringHandler.AppendFormatted<Vector3Int>(position);
                        //    draw.ScreenText(defaultInterpolatedStringHandler.ToStringAndClear(), new Vector2(20f, (float)(Screen.Height / 2f).FloorToInt()), "Consolas", 24f, TextFlag.LeftTop);
                        //}
                    }
                    else if (local.Team != BNB.Team.Unaligned && p.Team == local.Team)
                    {
                        // friendlies
                        Friendlies.Add(p);
                    }
                }

                if (Utils.UnitInFOV(local.Unit, p.Unit, 45f))
                {
                    //Log.Info("I see it!");
                    using (Gizmo.Scope(null))
                    {
                        Gizmo.Draw.IgnoreDepth = true;
                        Gizmo.Draw.LineThickness = 1f;

                        Gizmo.Draw.Color = p.TeamColor;
                        Gizmo.GizmoDraw draw = Gizmo.Draw;
                        BBox boundingBox = p.Unit.GetHitboxBounds(HitboxLimb.Torso);

                        Vector3 center = boundingBox.Center;
                        Vector3 extents = boundingBox.Size * 0.5f;
                        Vector3 scaledExtents = extents * 1.5f;

                        BBox expanded = new BBox(center - scaledExtents, center + scaledExtents);
                        //boundingBox.
                        draw.LineBBox(in expanded);
                        //Gizmo.Draw.LineBBox(in unit_bbox1);
                    }
                }
            }


            //Utils.Log($"{Enemies.Count}");

           
        }
    }
}

//internal void UpdateWorldRotation()
//{
//    Rotation targetRotation = Rotation.FromYaw(Rotation.LookAt(this.EyeDirection, Vector3.Up).Angles().yaw);
//    Vector3 vector;
//    for (; ; )
//    {
//        vector = targetRotation.Forward;
//        Vector3 forward = this.Renderer.WorldRotation.Forward;
//        if (Vector3.Dot(in vector, in forward) > 0.5f)
//        {
//            break;
//        }
//        Rotation newRotation = Rotation.Slerp(this.Renderer.WorldRotation, targetRotation, 0.01f, true);
//        this.Renderer.WorldRotation = newRotation;
//    }
//    vector = this.Controller.Velocity;
//    if (vector.Length > 10f)
//    {
//        Rotation newRotation2 = Rotation.Slerp(this.Renderer.WorldRotation, targetRotation, Time.Delta * 10f, true);
//        this.Renderer.WorldRotation = newRotation2;
//    }
//}
