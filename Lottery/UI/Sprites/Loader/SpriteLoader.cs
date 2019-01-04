using Lottery2019.UI.Behaviors;
using Lottery2019.UI.Sprites.Loader.Json;
using Lottery2019.UI.Shapes;
using Newtonsoft.Json;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlysEngine.Desktop;
using Lottery2019.UI.Forms;

namespace Lottery2019.UI.Sprites.Loader
{
    public class SpriteLoader
    {
        private readonly Dictionary<string, JsonSpriteStaticDefinition> _staticDef;

        public SpriteLoader(string staticSpriteFile)
        {
            _staticDef = LoadJsonSprites<Dictionary<string, JsonSpriteStaticDefinition>>(
                staticSpriteFile);
        }

        public IEnumerable<Sprite> CreateSprites(string stageFile, SpriteForm renderWindow)
        {
            var onStages = LoadJsonSprites<List<JsonSprite>>(stageFile);
            return onStages.Select(x =>
                MakeSprite(x, _staticDef, renderWindow));
        }

        private Sprite MakeSprite(
            JsonSprite def,
            Dictionary<string, JsonSpriteStaticDefinition> defs,
            SpriteForm xResource)
        {
            var sprite = new Sprite(xResource);

            var staticDef = defs[def.Type];
            sprite.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;
            sprite.SpriteType = def.Type;
            sprite.Frames = staticDef.Frames;
            sprite.Position = new Vector2(def.Position[0], def.Position[1]);
            sprite.Center = new Vector2(staticDef.Center[0], staticDef.Center[1]);
            sprite.Rotation = MathUtil.DegreesToRadians(def.Angle);
            sprite.Alpha = def.Alpha;

            // Shapes
            sprite.Shapes = staticDef.Shapes
                .Select(x => x.Create(sprite.Center))
                .ToList();
            sprite.Edges = staticDef.Edges
                .Select(x => x.Create(sprite.Center))
                .ToList();

            sprite.Children.AddRange(
                staticDef.Children.Select(child =>
                    MakeSprite(child, defs, xResource)));
            sprite.Children.AddRange(
                def.Children.Select(child =>
                    MakeSprite(child, defs, xResource)));

            // Behaviors
            //sprite.Behaviors["default"] = new AutoFrameBehavior(sprite);
#if DEBUG
            sprite.Behaviors["AutoBorderBehavior"] = new AutoBorderBehavior(sprite);
#endif
            foreach (var behavior in staticDef.Behaviors)
            {
                sprite.Behaviors[behavior.Key] = Behavior.Create(behavior.Key, sprite, behavior.Value);
            }
            foreach (var behavior in def.Behaviors)
            {
                sprite.Behaviors[behavior.Key] = Behavior.Create(behavior.Key, sprite, behavior.Value);
            }

            return sprite;
        }

        private static T LoadJsonSprites<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    namespace Json
    {
        public class JsonSpriteStaticDefinition
        {
            public string[] Frames { get; set; }

            public float[] Center { get; set; } = new float[2];

            public List<JsonSprite> Children { get; set; } = new List<JsonSprite>();

            public List<JsonShape> Shapes { get; set; } = new List<JsonShape>();

            public List<JsonShape> Edges { get; set; } = new List<JsonShape>();

            public Dictionary<string, Dictionary<string, object>> Behaviors { get; set; }
                = new Dictionary<string, Dictionary<string, object>>();
        }

        public class JsonSprite
        {
            public string Type { get; set; }

            public float[] Position { get; set; } = new float[2];

            public List<JsonSprite> Children { get; set; } = new List<JsonSprite>();

            public float Angle { get; set; }

            public float Alpha { get; set; } = 1.0f;

            public Dictionary<string, Dictionary<string, object>> Behaviors { get; set; }
                = new Dictionary<string, Dictionary<string, object>>();
        }
    }
}
