using System.Linq;
using Lottery2019.UI.Sprites;
using Lottery2019.UI.Shapes;
using SharpDX;
using SharpDX.Direct2D1;

namespace Lottery2019.UI.Behaviors
{
    public class AutoBorderBehavior : Behavior
    {
        private bool _mouseOverSprite;
        public Color Color = Color.Orange;

        public AutoBorderBehavior(Sprite sprite) : base(sprite)
        {
        }

        public override void Draw(DeviceContext renderTarget)
        {
            if (!_mouseOverSprite) return;

            DrawInternal(renderTarget);
        }

        public void DrawInternal(DeviceContext renderTarget)
        {
            foreach (var shape in Sprite.Shapes.Concat(Sprite.Edges))
            {
                shape.Draw(renderTarget, Sprite.XResource.Brushes.Get(Color));
            }
        }

        public override void UpdateLogic(float dt)
        {
            var res = Sprite.XResource;
            _mouseOverSprite = Shape.TestPoint(
                Sprite.Shapes.Concat(Sprite.Edges),
                res.InvertTransformPoint(Sprite.Transform * res.RenderTarget.Transform, res.MouseClientPosition));
        }
    }
}
