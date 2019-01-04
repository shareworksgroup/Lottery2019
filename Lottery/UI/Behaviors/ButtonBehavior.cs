using System;
using Lottery2019.UI.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class ButtonBehavior : Behavior
    {
        const float MaxHoldDuration = 0.2f;
        float _accumulateDt = 0.0f;

        public ButtonBehavior(Sprite sprite) : base(sprite)
        {
        }

        public override void UpdateLogic(float dt)
        {
            var res = Sprite.XResource;

            if (res.MouseState.Buttons[0])
            {
                if (Sprite.IsMouseOver())
                {
                    Sprite.FrameId = 1;
                    _accumulateDt += dt;
                }
            }
            else
            {
                if (_accumulateDt > 0 && _accumulateDt < MaxHoldDuration)
                {
                    Click?.Invoke(this, EventArgs.Empty);
                }

                Sprite.FrameId = 0;
                _accumulateDt = 0;
            }
        }

        public override void Dispose()
        {
            foreach (EventHandler d in Click.GetInvocationList())
            {
                Click -= d;
            }
        }

        public event EventHandler Click;
    }
}
