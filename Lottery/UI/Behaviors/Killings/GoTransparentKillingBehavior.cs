using Lottery2019.UI.Sprites;
using Animation = SharpDX.Animation;

namespace Lottery2019.UI.Behaviors.Killings
{
    public class GoTransparentKillingBehavior : KillingBehavior
    {
        const float KillingTime = 3.0f;
        private Animation.Variable _var;

        public override void Update(float dt)
        {
            base.Update(dt);

            if (PersonSprite.IsAlive) return;

            Sprite.Alpha = (float)_var.Value;
            if (_var.Value == _var.FinalValue)
            {
                _var.Dispose();
                _var = null;                

                ((PersonSprite)Sprite).SetCanBeDelete();
            }
        }

        public override void Kill()
        {
            PersonSprite.IsAlive = false;
            _var = Sprite.XResource.CreateAnimation(1.0f, 0.0f, KillingTime);
        }

        public override void Dispose()
        {
            if (_var != null)
            {
                _var.Dispose();
                _var = null;
            }
        }
    }
}
