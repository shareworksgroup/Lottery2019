using Lottery2019.UI.Sprites;
using Animation = SharpDX.Animation;

namespace Lottery2019.UI.Behaviors.Killings
{
    public class GoTransparentKillingBehavior : KillingBehavior
    {
        const float KillingTime = 3.0f;
        private Animation.Variable _var;
        bool killing = false;

        public GoTransparentKillingBehavior(Sprite sprite) : base(sprite)
        {
        }

        public override void UpdateLogic(float dt)
        {
            base.UpdateLogic(dt);

            if (!killing)
                return;

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
            killing = true;
            _var = Sprite.XResource.CreateAnimation(1.0f, 0.0f, KillingTime);
            Sprite.Body.Enabled = false;
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
