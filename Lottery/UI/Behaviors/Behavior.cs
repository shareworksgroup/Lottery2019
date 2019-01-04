using Lottery2019.UI.Sprites;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery2019.UI.Behaviors
{
    public abstract class Behavior : IDisposable
    {
        protected readonly Sprite Sprite;

        public Behavior(Sprite sprite)
        {
            Sprite = sprite;
        }

        public virtual void UpdateLogic(float dt)
        {
        }

        public virtual void Draw(SharpDX.Direct2D1.DeviceContext renderTarget)
        {
            // default do nothing
        }

        private static Dictionary<string, Func<Sprite, Behavior>> BehaviorMap
            = new Dictionary<string, Func<Sprite, Behavior>>
            {
                [nameof(RotationBehavior)] = r => new RotationBehavior(r),
                [nameof(AutoFrameBehavior)] = r => new AutoFrameBehavior(r),
                [nameof(ChomperBehavior)] = r => new ChomperBehavior(r),
                [nameof(GoAngleBehavior)] = r => new GoAngleBehavior(r), 
                [nameof(CircleCreatorBehavior)] = r => new CircleCreatorBehavior(r), 
                [nameof(ButtonBehavior)] = r => new ButtonBehavior(r), 
                [nameof(AutoBorderBehavior)] = r => new AutoBorderBehavior(r), 
                [nameof(OpenCloseBehavior)] = r => new OpenCloseBehavior(r)
            };

        public static Behavior Create(string behaviorName, Sprite sprite, Dictionary<string, object> options)
        {
            if (BehaviorMap.ContainsKey(behaviorName))
            {
                var behavior = BehaviorMap[behaviorName](sprite);
                var type = behavior.GetType();
                foreach (var option in options)
                {
                    var prop = type.GetProperty(option.Key);
                    prop.SetValue(behavior, SwitchType(option.Value, prop.PropertyType));
                }
                return behavior;
            }
            throw new KeyNotFoundException(nameof(behaviorName));
        }

        private static object SwitchType(object v, Type targetType)
        {
            if (v is double d)
            {
                return (float)d;
            }
            else if (v is long di)
            {
                if (targetType == typeof(int))
                {
                    return (int)di;
                }
                else if (targetType == typeof(float))
                {
                    return (float)di;
                }
            }
            else if (v is JArray ds)
            {
                return ds.Select(x => (float)x).ToArray();
            }

            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            // do nothing by default
        }
    }
}
