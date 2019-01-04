using System.Linq;
using SharpDX;
using SharpDX.Direct2D1;
using FlysEngine.Sprites;
using FlysEngine.Managers;
using FlysEngine.Sprites.Shapes;

namespace Lottery2019.UI.Behaviors
{
    public class AutoBorderBehavior : Behavior
    {
        private bool _mouseOverSprite;
        public Color Color = Color.Orange;

        public override void Draw(DeviceContext renderTarget)
        {
            if (!_mouseOverSprite) return;

            DrawInternal(renderTarget);
        }

        public void DrawInternal(DeviceContext renderTarget)
        {
            foreach (var shape in Sprite.Shapes.Concat(Sprite.Edges))
            {
                shape.Draw(renderTarget, Sprite.XResource.GetColor(Color));
            }
        }

        public override void Update(float dt)
        {
            var res = Sprite.XResource;
            _mouseOverSprite = Shape.TestPoint(
                Sprite.Shapes.Concat(Sprite.Edges),
                res.InvertTransformPoint(Sprite.Transform * res.RenderTarget.Transform, Sprite.Window.MouseClientPosition));
        }
    }
}
