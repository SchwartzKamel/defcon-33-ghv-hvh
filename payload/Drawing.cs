using BNB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace payload
{
    internal class Drawing
    {
        public static void DrawText(Vector3 WorldPos)
        {
            Vector3Int position = VoxelMap.WorldToVoxel(WorldPos);
            using (Gizmo.Scope(null))
            {
                Gizmo.Draw.Color = Color.White;
                Gizmo.GizmoDraw draw = Gizmo.Draw;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
                defaultInterpolatedStringHandler.AppendLiteral("POS: ");
                defaultInterpolatedStringHandler.AppendFormatted<Vector3Int>(position);
                draw.ScreenText(defaultInterpolatedStringHandler.ToStringAndClear(), new Vector2(20f, (float)(Screen.Height / 2f).FloorToInt()), "Consolas", 24f, TextFlag.LeftTop);
            }
        }
    }
}
