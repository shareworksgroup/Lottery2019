using FarseerPhysics.Dynamics;
using Lottery2019.UI.Behaviors;
using Lottery2019.UI.Details;
using Lottery2019.UI.Shapes;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Direct2D1 = SharpDX.Direct2D1;

namespace Lottery2019.UI.Sprites
{
    public class Sprite : IDisposable
    {
        public readonly XResource XResource;
        public event EventHandler<Sprite> Hit;        

        public Dictionary<string, Behavior> Behaviors = new Dictionary<string, Behavior>();

        public string Id = Guid.NewGuid().ToString();
        public string SpriteType;
        public string[] Frames;
        public int FrameId = 0;
        public List<Sprite> Children = new List<Sprite>();
        public bool IsDisposed { get; set; }
        public object UserData { get; set; }

        public readonly Body Body;

        private List<Shape> _shapes;
        public List<Shape> Shapes
        {
            get => _shapes;
            set
            {
                _shapes = value;
                foreach (var fixture in Body.FixtureList.ToList())
                    Body.DestroyFixture(fixture);
                Shape.CreateFixtures(value, Body);
            }
        }

        private List<Shape> _edges = new List<Shape>();
        public List<Shape> Edges
        {
            get => _edges;
            set
            {
                _edges = value;
                Shape.CreateFixtures(value, Body);
            }
        }

        public Vector2 Position
        {
            get => Body.Position.ToVector2();
            set => Body.Position = value.ToXnaVector2();
        }

        public Vector2 Center;


        public float Rotation
        {
            get => Body.Rotation;
            set => Body.Rotation = value;
        }
        
        public float Alpha { get; internal set; } = 1.0f;

        public Matrix3x2 Transform = Matrix3x2.Identity;

        public Sprite(XResource xResource)
        {
            XResource = xResource;
            Body = new Body(xResource.World)
            {
                UserData = this
            };
        }

        public bool IsMouseOver()
        {
            return Shape.TestPoint(Shapes, XResource.InvertTransformPoint(
                Transform * XResource.RenderTarget.Transform,
                XResource.MouseClientPosition));
        }

        public virtual void UpdateLogic(float dt)
        {
            foreach (var behavior in Behaviors.Values)
            {
                behavior.UpdateLogic(dt);
            }

            foreach (var child in Children)
            {
                child.UpdateLogic(dt);
            }

            Transform = Matrix3x2.Rotation(Rotation, Center) * Matrix3x2.Translation(Position - Center);

            if (Hit != null && Body.Enabled)
            {
                foreach (var contact in GetContacts())
                {
                    Hit(this, contact);
                }

                IEnumerable<Sprite> GetContacts()
                {
                    var c = Body.ContactList;
                    while (c != null)
                    {
                        if (c.Contact.IsTouching())
                        {
                            yield return (Sprite)c.Other.UserData;
                        }
                        c = c.Next;
                    }
                }
            }
        }

        public virtual void Draw(Direct2D1.DeviceContext renderTarget)
        {
            var old = XResource.RenderTarget.Transform;
            renderTarget.Transform = Transform * old;

            if (Frames != null)
            {
                renderTarget.DrawBitmap(
                    XResource.Bitmaps.Get(Frames[FrameId]),
                    Alpha,
                    Direct2D1.InterpolationMode.Linear);
            }
            
            foreach (var behavior in Behaviors.Values)
            {
                behavior.Draw(renderTarget);
            }
            renderTarget.Transform = old;

            foreach (var child in Children)
            {
                child.Draw(renderTarget);
            }
        }

        public virtual void Dispose()
        {
            foreach (var child in Children)
                child.Dispose();
            Children.Clear();

            if (Hit != null)
            {
                foreach (EventHandler<Sprite> d in Hit.GetInvocationList())
                {
                    Hit -= d;
                }
            }

            foreach (var behavior in Behaviors.Values)
            {
                behavior.Dispose();
            }
            XResource.World.RemoveBody(Body);
            IsDisposed = true;
        }

        public override string ToString()
        {
            return $"{SpriteType}:{Id}";
        }
    }
}
