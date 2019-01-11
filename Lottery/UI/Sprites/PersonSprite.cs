using FarseerPhysics.Dynamics;
using FlysEngine.Sprites;
using FlysEngine.Sprites.Shapes;
using Lottery2019.Images;
using Lottery2019.UI.Behaviors;
using Lottery2019.UI.Behaviors.Killings;
using Lottery2019.UI.Forms;
using SharpDX;
using System;
using System.Collections.Generic;

namespace Lottery2019.UI.Sprites
{
    public class PersonSprite : Sprite
    {
        public event EventHandler CanBeDeleted;
        
        public bool CanBeDelete { get; private set; }

        public PersonSprite(SpriteForm window, Person person) : base(window)
        {
            Body.BodyType = BodyType.Dynamic;
            Body.SleepingAllowed = false;
            Name = "Person";
            Frames = new[] { person.PsdImage };
            Center = circleShape.Center;
            Position = new Vector2(r.NextFloat(448, 1488), r.NextFloat(454, 1005));
            UserData = person;
            SetShapes(circleShape.Clone());
            AddBehavior(new AutoBorderBehavior());
            AddBehavior(new GoTransparentKillingBehavior());
        }

        public Person Person => (Person)UserData;
        public KillingBehavior KillingBehavior => this.QueryBehavior<GoTransparentKillingBehavior>();

        public void Kill()
        {
            KillingBehavior.Kill();
        }

        public void SetCanBeDelete()
        {
            CanBeDelete = true;
            CanBeDeleted?.Invoke(this, EventArgs.Empty);
        }

        public override void Dispose()
        {
            if (CanBeDeleted != null)
            {
                foreach (EventHandler d in CanBeDeleted.GetInvocationList())
                {
                    CanBeDeleted -= d;
                }
            }
            base.Dispose();            
        }

        private static Random r = new Random();
        private static CircleShape circleShape = new CircleShape(ImageDefines.PhysicalR / 2.0f)
        {
            Center = new Vector2(ImageDefines.Size / 2.0f, ImageDefines.Size / 2.0f),
            Offset = new Vector2(
                                (ImageDefines.Size - ImageDefines.RealSize) / 2.0f,
                                (ImageDefines.Size - ImageDefines.RealSize) / 2.0f),
        };
    }
}
