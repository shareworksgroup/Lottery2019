using System;
using Lottery2019.Dtos;
using Lottery2019.Images;
using Lottery2019.UI.Sprites;
using SharpDX;
using SharpDX.Direct2D1;
using Animation = SharpDX.Animation;

namespace Lottery2019.UI.Behaviors
{
    public class QuoteBehavior : Behavior
    {
        public const float QuoteDuration = 2.0f;
        public const float QuoteHidingDuration = 1.0f;
        private static readonly float FontSize = 16.0f;
        public static float FontScale = 1.5f;
        public QuoteStatus Status { get; private set; } = QuoteStatus.Peath;

        static QuoteBehavior()
        {
            FontSize = FontScale * 16.0f;
        }

        private BarrageDto _barrage;
        private Animation.Variable _alpha;
        private Color GetCurrentColor(float alpha = 1.0f)
        {
            try
            {
                var color = System.Drawing.ColorTranslator.FromHtml(_barrage.Color);
                return new Color(color.R, color.G, color.B, alpha);
            }
            catch (Exception e) when (e.InnerException is FormatException)
            {
                return Color.Yellow;
            }
        }

        public QuoteBehavior(Sprite sprite) : base(sprite)
        {
        }

        public void CreateQuote(BarrageDto barrage)
        {
            if (Status != QuoteStatus.Peath)
            {
                Clear();
            }

            _barrage = barrage;
            Status = QuoteStatus.Quoting;
            _alpha = Sprite.XResource.CreateAnimation(3.0, 0.0, QuoteDuration);
        }

        private void Clear()
        {
            if (_alpha != null)
            {
                _alpha.Dispose();
                _alpha = null;
            }
            if (_barrage != null)
            {
                Sprite.XResource.TextLayouts.Remove(
                    _barrage.Text, FontSize);
                _barrage = null;
            }

            Status = QuoteStatus.Peath;
        }

        public override void UpdateLogic(float dt)
        {
            base.UpdateLogic(dt);
            if (_alpha != null && _alpha.Value == _alpha.FinalValue)
            {
                switch (Status)
                {
                    case QuoteStatus.Hiding:
                        Clear();
                        break;
                    case QuoteStatus.Quoting:
                        Status = QuoteStatus.Hiding;
                        _alpha.Dispose();
                        _alpha = Sprite.XResource.CreateAnimation(1.0, 0.0, QuoteHidingDuration);
                        break;
                }
            }
        }

        public override void Draw(DeviceContext renderTarget)
        {
            if (Status == QuoteStatus.Peath)
                return;

            var alpha = Status == QuoteStatus.Hiding ?
                (float)_alpha.Value : 1.0f;

            var bmp = Sprite.XResource.TextLayouts[_barrage.Text, FontSize];
            renderTarget.DrawTextLayout(new Vector2(ImageDefines.RealSize, 0.0f),
                bmp,
                Sprite.XResource.GetColor(GetCurrentColor(alpha)));
        }

        public override void Dispose()
        {
            base.Dispose();
            Clear();
        }

        public enum QuoteStatus
        {
            Peath,
            Quoting,
            Hiding,
        }
    }
}
