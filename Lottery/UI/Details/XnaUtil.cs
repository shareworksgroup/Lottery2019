using FarseerPhysics;

namespace Lottery2019.UI.Details
{
    public static class XnaUtil
    {
        public static SharpDX.Vector2 ToVector2(this Microsoft.Xna.Framework.Vector2 xnaVector2)
        {
            return new SharpDX.Vector2(
                ConvertUnits.ToDisplayUnits(xnaVector2.X), 
                ConvertUnits.ToDisplayUnits(xnaVector2.Y));
        }

        public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this SharpDX.Vector2 vector2)
        {
            return new Microsoft.Xna.Framework.Vector2(
                ConvertUnits.ToSimUnits(vector2.X), 
                ConvertUnits.ToSimUnits(vector2.Y));
        }

        public static SharpDX.Vector2 ToVector2(this System.Drawing.Point vector2)
        {
            return new SharpDX.Vector2(
                vector2.X,
                vector2.Y);
        }
    }
}
