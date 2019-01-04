using Lottery2019.UI.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public abstract class StagesBehavior : Behavior
    {
        public virtual float[] Stages { get; set; }

        public abstract float Length { get; }

        public int StageId { get; set; } = 0;

        protected float AccumulateDt;
        float WayAccumulate;

        public StagesBehavior(Sprite sprite) : base(sprite)
        {
        }

        public override void UpdateLogic(float dt)
        {
            AccumulateDt += dt;
            if (AccumulateDt >= Stages[StageId])
            {
                AccumulateDt = Stages[StageId];
                CalcDx();
                AccumulateDt = 0;

                StageId = (StageId + 1) % Stages.Length;
                WayAccumulate = 0;
            }

            CalcDx();
        }

        private void CalcDx()
        {
            var should = AccumulateDt / Stages[StageId] * Length;
            var dx = should - WayAccumulate;
            WayAccumulate += dx;

            ByDx(dx);
        }

        protected abstract void ByDx(float dx);
    }
}
