using FlysEngine.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class ThunderBehavior : Behavior
    {
        public float[] Stages { get; set; } = new[] { 0.5f, 0.5f, 1.0f };
        public int StageId { get; set; } = 0;
        public float CurrentStage => Stages[StageId];

        float _dtAll = 0;

        public override void Update(float dt)
        {
            _dtAll += dt;
            if (_dtAll > CurrentStage)
            {
                _dtAll -= CurrentStage;

                StageId = (StageId + 1) % Stages.Length;
                OnStage(StageId);
            }
        }

        private void OnStage(int stageId)
        {
            switch (stageId)
            {
                case 0: // thunder, unsafe
                    Sprite.Body.Enabled = true;
                    Sprite.Alpha = 1;
                    foreach (var item in Sprite.Children) item.Alpha = 1;
                    break;
                case 1: // air, safe
                    Sprite.Body.Enabled = false;
                    Sprite.Alpha = 1;
                    foreach (var item in Sprite.Children) item.Alpha = 0;
                    break;
                case 2: // nothing, safe
                    Sprite.Body.Enabled = false;
                    Sprite.Alpha = 0;
                    foreach (var item in Sprite.Children) item.Alpha = 0;
                    break;
            }
        }

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);
            sprite.Body.IsSensor = true;
            foreach (Sprite child in sprite.Children) child.Body.IsSensor = true;
            OnStage(0);
        }
    }
}
