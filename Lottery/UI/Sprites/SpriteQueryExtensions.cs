using FlysEngine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery2019.UI.Sprites
{
    public static class SpriteQueryExtensions
    {
        public static IEnumerable<Sprite> FindAll(this Dictionary<Guid, Sprite> sprites, string spriteType)
        {
            return sprites.Values.FindAll(spriteType);
        }

        public static IEnumerable<Sprite> FindAll(this IEnumerable<Sprite> sprites, string spriteType)
        {
            foreach (var sprite in sprites)
            {
                if (sprite.Name == spriteType)
                {
                    yield return sprite;
                }

                foreach (var childSprite in FindAll(sprite.Children, spriteType))
                {
                    yield return childSprite;
                }
            }
        }

        public static Sprite FindSingle(this Dictionary<Guid, Sprite> sprites, string spriteType)
        {
            return FindAll(sprites, spriteType).Single();
        }

        public static Sprite FindSingle(this IEnumerable<Sprite> sprites, string spriteType)
        {
            return FindAll(sprites, spriteType).Single();
        }

        public static T QueryBehavior<T>(this Dictionary<Guid, Sprite> sprites, string spriteType)
            where T : Behavior
        {
            var sprite = sprites.FindSingle(spriteType);
            return sprite.QueryBehavior<T>();
        }

        public static IEnumerable<T> QueryBehaviorAll<T>(this Dictionary<Guid, Sprite> sprites) where T : Behavior
        {
            return sprites.Values
                .EnumerateAll()
                .Select(x => x.QueryBehavior<T>())
                .Where(x => x != null);
        }

        public static IEnumerable<Sprite> EnumerateAll(this IEnumerable<Sprite> sprites)
        {
            foreach (Sprite sprite in sprites)
            {
                yield return sprite;
                foreach (Sprite childSprite in sprite.Children) yield return childSprite;
            }
        }

        public static T QueryBehavior<T>(this Dictionary<string, Behavior> behaviors)
            where T : Behavior
        {
            var behavior = behaviors[typeof(T).Name];
            return (T)behavior;
        }

        public static void QueryBehaviorAll<T>(this IEnumerable<Sprite> sprites, string spriteType, Action<T> action)
            where T : Behavior
        {
            var all = sprites.FindAll(spriteType);
            foreach (var sprite in all)
            {
                action(sprite.QueryBehavior<T>());
            }
        }

        public static bool RemoveChild(this List<Sprite> sprites, Sprite val)
        {
            if (sprites.Remove(val))
                return true;

            foreach (var sprite in sprites)
            {
                if (sprite.Children.RemoveChild(val))
                    return true;
            }

            return false;
        }

        public static bool RemoveChild(this Dictionary<Guid, Sprite> sprites, Sprite val)
        {
            if (sprites.Remove(val.Id))
                return true;

            foreach (var sprite in sprites.Values)
            {
                if (sprite.Children.RemoveChild(val))
                    return true;
            }

            return false;
        }
    }
}
