using System;
using FlysEngine.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class ButtonBehavior : Behavior
    {
        const float MaxHoldDuration = 0.2f;
        float _accumulateDt = 0.0f;

        public override void Update(float dt)
        {
            var res = Sprite.XResource;
            var win = Sprite.Window;

            if (win.MouseState.Buttons[0])
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

        public event EventHandler Click;
    }
}
