using System;
using System.Diagnostics;
using FlysEngine.Sprites;
using Lottery2019.UI.Sprites;
using SharpDX;
using Animation = SharpDX.Animation;

namespace Lottery2019.UI.Behaviors
{
    public class OpenCloseBehavior : Behavior
    {
        private Sprite _left;
        private Sprite _right;

        public float OpenDuration { get; set; } = 0.5f;

        public float CloseDuration { get; set; } = 0.5f;

        public float Angle { get; set; } = MathUtil.DegreesToRadians(120.0f);

        // 0 = static
        // 1 = opening
        // 2 = closing
        public OpenCloseStatus Status { get; set; } = OpenCloseStatus.Closed;

        private Animation.Variable _animationL, _animationR;

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);
            _left = sprite.Children.FindSingle("BaffleSprite");
            _right = sprite.Children.FindSingle("Baffle2Sprite");
        }

        public void Open()
        {
            if (Status == OpenCloseStatus.Closed)
            {
                Status = OpenCloseStatus.Opening;
                Debug.Assert(_animationL == null);
                
                _animationL = _left.XResource.CreateAnimation(
                    _left.Rotation,
                    _left.Rotation + Angle,
                    OpenDuration);
                _animationR = _right.XResource.CreateAnimation(
                    _right.Rotation,
                    _right.Rotation - Angle,
                    OpenDuration);
            }
        }

        public void Close()
        {
            if (Status == OpenCloseStatus.Opened)
            {
                Status = OpenCloseStatus.Closing;
                Debug.Assert(_animationL == null);

                var angle = MathUtil.DegreesToRadians(Angle);
                _animationL = _left.XResource.CreateAnimation(
                    _left.Rotation,
                    _left.Rotation - Angle,
                    CloseDuration);
                _animationR = _right.XResource.CreateAnimation(
                    _right.Rotation,
                    _right.Rotation + Angle,
                    CloseDuration);
            }
        }

        public event EventHandler Opened;
        public event EventHandler Closed;

        public override void Update(float dt)
        {
            if (_animationL == null) return;

            _left.Rotation = (float)_animationL.Value;
            _right.Rotation = (float)_animationR.Value;

            if (_animationL.Value == _animationL.FinalValue)
            {
                if (Status == OpenCloseStatus.Opening)
                {
                    // Open done.
                    Status = OpenCloseStatus.Opened;
                    Opened?.Invoke(this, EventArgs.Empty);
                }
                else if (Status == OpenCloseStatus.Closing)
                {
                    Status = OpenCloseStatus.Closed;
                    Closed?.Invoke(this, EventArgs.Empty);
                }
                _animationL.Dispose();
                _animationL = null;
            }
        }

        public override void Dispose()
        {
            if (_animationL != null)
            {
                _animationL.Dispose();
                _animationR.Dispose();
            }
        }

        public enum OpenCloseStatus
        {
            Closed, 
            Opening, 
            Opened, 
            Closing, 
        }
    }
}
