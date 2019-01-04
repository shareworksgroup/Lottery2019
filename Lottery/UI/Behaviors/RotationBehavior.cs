using FarseerPhysics.Dynamics;
using Lottery2019.UI.Sprites;
using SharpDX;

namespace Lottery2019.UI.Behaviors
{
    public class RotationBehavior : Behavior
    {
        public float Speed { get; set; } = 1.0f;

        public RotationBehavior(Sprite sprite):
            base(sprite)
        {
            if (sprite.Body.BodyType == BodyType.Static)
            {
                sprite.Body.BodyType = BodyType.Kinematic;
            }
        }

        public override void UpdateLogic(float dt)
        {
            Sprite.Body.AngularVelocity = Speed * MathUtil.TwoPi;
        }
    }
}
