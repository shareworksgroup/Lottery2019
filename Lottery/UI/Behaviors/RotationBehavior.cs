using FarseerPhysics.Dynamics;
using FlysEngine.Sprites;
using SharpDX;

namespace Lottery2019.UI.Behaviors
{
    public class RotationBehavior : Behavior
    {
        public float Speed { get; set; } = 1.0f;

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);
            if (sprite.Body.BodyType == BodyType.Static)
            {
                sprite.Body.BodyType = BodyType.Kinematic;
            }
        }

        public override void Update(float dt)
        {
            Sprite.Body.AngularVelocity = Speed * MathUtil.TwoPi;
        }
    }
}
