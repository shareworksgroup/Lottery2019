using FlysEngine.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class BoxOpenBehavior : Behavior
    {
        private float _dtAll;
        private bool _started = false;

        public float Duration { get; set; }

        public override void Update(float dt)
        {
            if (!_started) return;

            if (Sprite.FrameId == Sprite.Frames.Length - 1)
            {
                _started = false;
                Sprite.SetShapes();
                return;
            }

            _dtAll += dt;
            float stageDt = Duration / Sprite.Frames.Length;
            if (_dtAll >= stageDt)
            {
                Sprite.FrameId++;
                _dtAll = _dtAll - stageDt;
            }
        }

        public void Open() => _started = true;

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);
            foreach (var fixture in sprite.Body.FixtureList)
            {
                fixture.Restitution = 2.5f;
                fixture.Friction = 0.0f;
            }
        }
    }
}
