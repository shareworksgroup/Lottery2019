using FarseerPhysics;
using SharpDX;
using SharpDX.Direct2D1;
using EngineShapes = FarseerPhysics.Collision.Shapes;

namespace Lottery2019.UI.Shapes
{
    public class CircleShape : Shape
    {
        public CircleShape(float r)
        {
            R = r;
        }

        public CircleShape(JsonShape jsonShape, Vector2 center) : base(jsonShape, center)
        {
            R = jsonShape.R;
        }

        public float R { get; private set; }

        public CircleShape Clone()
        {
            return new CircleShape(R)
            {
                Offset = Offset, 
                Center = Center, 
            };
        }

        public override void Draw(DeviceContext renderTarget, Brush brush)
        {
            renderTarget.DrawEllipse(
                        new Ellipse(Center + Offset, R, R),
                        brush,
                        2.0f);
        }

        public override bool TestPoint(Vector2 point)
        {
            return Vector2.DistanceSquared(Center + Offset, point) 
                < R * R;
        }

        public override EngineShapes.Shape ToEngineShape()
        {
            return new EngineShapes.CircleShape(
                    ConvertUnits.ToSimUnits(R), Density);
        }
    }
}
