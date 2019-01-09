using Lottery2019.Images;
using Lottery2019.UI;
using Lottery2019.UI.Behaviors;
using Lottery2019.UI.Forms;
using Lottery2019.UI.Sprites;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Key = SharpDX.DirectInput.Key;
using SharpDX.Direct2D1;
using Lottery2019.UI.Feature;
using Newtonsoft.Json;
using System.IO;
using Lottery2019.Dtos;
using Lottery2019.State;
using System.Threading.Tasks;
using System.Diagnostics;
using FlysEngine.Sprites;

namespace Lottery2019
{
    public partial class Form1 : SpriteForm
    {
        const string SpritesDef = "./Resources/sprite-static.json";
        const string DefaultStage = "./Resources/stage-123.json";
        LotteryContext Context = new LotteryContext();

        public Form1() : base(SpritesDef, DefaultStage)
        {
            Text = "抽奖";
            WindowState = FormWindowState.Maximized;
        }

        protected override void OnSpriteCreated()
        {
            Context.Reset();

            var lotterySprite = Sprites.FindSingle("LotterySprite");
            // TODO:
            //foreach (var fixture in lotterySprite.Body.FixtureList)
            //    fixture.IsSensor = true;
            foreach (var fixture in lotterySprite.Body.FixtureList)
            {
                fixture.Restitution = 0.1f;
                fixture.Friction = 0.8f;
            }

            Sprites.QueryBehavior<ButtonBehavior>("StartButton")
                .Click += (o, e) => TriggerStart();

            lotterySprite.Hit += PersonWin;
            foreach (var sprite in Sprites.FindAll("Chomper"))
                sprite.Hit += PersonLost;

            if (Context.CurrentPrize != null)
            {
                CreatePersons();
            }
        }

        private void EnterPrize(PrizeDto prize)
        {
            var stagePath = DefaultStage;
            if (prize.Stage != null) stagePath = prize.Stage;

            BeginInvoke(new Action(() =>
            {
                ResetStage(stagePath);
                if (Context.CurrentPrize != null)
                {
                    AddSprites(new Sprite(this)
                    {
                        Name = "TheLottery",
                        Frames = new[] { Context.CurrentPrize.Image },
                        Position = new Vector2(1500, 425),
                    });
                }
            }));
        }

        static Random r = new Random();

        private void CreatePersons()
        {
            var stage = Sprites.FindSingle("StageSprite");
            var count = stage.Children.Count;
            stage.Children.AddRange(Context.Database.GetPersonForPrize(Context.CurrentPrize)
                .Select(person =>
                {
                    var sprite = new PersonSprite(this, person);
                    sprite.CanBeDeleted += Sprite_CanBeDeleted;
                    sprite.AddBehavior(new QuoteBehavior());
                    Context.PersonSprites[person.Name] = sprite;
                    return sprite;
                }));
            var personCount = stage.Children.Count - count;
            Debug.WriteLine($"PersonCount: {personCount}");
        }

        private void Sprite_CanBeDeleted(object sender, EventArgs e)
        {
            var sprite = (Sprite)sender;
            Context.SpriteToRemove.Add(sprite);
        }

        private void PersonLost(object sender, Sprite e)
        {
            if (!(e is PersonSprite sprite)) return;
            e.QueryBehavior<QuoteBehavior>().CreateQuote(new BarrageDto
            {
                UserName = sprite.Person.Name,
                Color = "white",
                Content = Context.WordDef.Lost[r.Next(Context.WordDef.Lost.Count)]
            });
            sprite.Kill();
        }

        private void PersonWin(object sender, Sprite e)
        {
            if (!(e is PersonSprite sprite)) return;
            if (Context.GameOver) return;
            if (Context.WinPersons.ContainsKey(sprite.Person.Name)) return;

            e.QueryBehavior<QuoteBehavior>().CreateQuote(new BarrageDto
            {
                UserName = sprite.Person.Name,
                Color = "yellow",
                Content = Context.WordDef.Win[r.Next(Context.WordDef.Win.Count)]
            });
            Context.WinPersons[sprite.Person.Name] = sprite.Person;
            sprite.Body.Restitution = 0.2f;
            sprite.Body.Friction = 0.5f;

            if (Context.CurrentPrize.Count == Context.WinPersons.Count)
            {
                OnGameOver();
            }
        }

        private async void OnGameOver()
        {
            Context.GameOver = true;
            Context.Database.SetWinPersons(Context.CurrentPrize.Id, Context.WinPersons.Values.ToList());
            Context.StopWorld = true;

            await Task.Delay(1000);
            BeginInvoke(new Action(() =>
            {
                WinnerForm.Show(
                    Context.CurrentPrize,
                    Context.WinPersons.Values.ToList());
                Context.CurrentPrize = null;
                CameraY = 0;
                ResetStage(DefaultStage);
            }));
        }

        private void Start()
        {
            Context.StopWorld = false;
            Context.Started = true;
            Context.AutoCamera = true;

            var maxSpeed = World.BodyList
                .Where(x => x.UserData is PersonSprite)
                .Max(x => x.LinearVelocity.Length);
            if (maxSpeed == 0) maxSpeed = 1;

            Context.GravityRate = 1.0f / maxSpeed;
            foreach (var body in World.BodyList
                .Where(x => x.UserData is PersonSprite))
            {
                body.LinearVelocity /= maxSpeed;
                body.AngularVelocity /= (maxSpeed / 2);
            }
        }

        protected override void OnUpdateLogic(float lastFrameTimeInSecond)
        {
            foreach (var sprite in Context.SpriteToRemove)
            {
                sprite.Dispose();
                Sprites.RemoveChild(sprite);
            }
            foreach (var sprite in Context.SpriteToAdd)
            {
                AddSprites(sprite);
            }

            Context.SpriteToRemove.Clear();

            base.OnUpdateLogic(lastFrameTimeInSecond);

            UpdateCameraY(lastFrameTimeInSecond);
            World.Gravity.Y = 9.82f * Context.GravityRate;
            if (!Context.StopWorld)
            {
                World.Step(lastFrameTimeInSecond);
            }

            if (Context.AutoCamera && Context.PersonSprites.Count > 0)
            {
                CameraY = MathUtil.Clamp(Context.PersonSprites.Values.Max(x => x.Position.Y) - 600.0f,
                    CameraY, MaxStage);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!KeyboardState.IsPressed(Key.LeftShift)) return;

            switch (e.KeyChar)
            {
                case 'R':
                    ResetStage(DefaultStage);
                    break;
                case 'P':
                    Context.StopWorld = !Context.StopWorld;
                    break;
                case ' ':
                    TriggerStart();
                    break;
            }
        }

        private void TriggerStart()
        {
            if (Context.CurrentPrize != null)
            {
                Start();
            }
            else
            {
                Context.CurrentPrize = PickPrizeWindow.PickPrize(
                    Context.Database.GetWinnedPrizes());
                if (Context.CurrentPrize != null)
                    EnterPrize(Context.CurrentPrize);
            }
        }

        protected override void OnDraw(DeviceContext renderTarget)
        {
            base.OnDraw(renderTarget);
            if (!Context.GameOver && Context.CurrentPrize != null)
            {
                renderTarget.DrawText(
                    $"{Context.WinPersons.Count}/{Context.CurrentPrize.Count}",
                    XResource.TextFormats[24.0f],
                    new RectangleF(1550.0f, 2700.0f, 100.0f, 30.0f),
                    XResource.GetColor(Color.Yellow));
            }
        }

        void UpdateCameraY(float dt)
        {
            const float CameraSpeed = 0.5f;
            const float MaxMovementDown = MaxStage;
            var cameraMovement = CameraSpeed * dt * StageHeight;

            var offset = 0.0f;
            if (KeyboardState.IsPressed(Key.Down))
            {
                offset = cameraMovement;
            }
            else if (KeyboardState.IsPressed(Key.Up))
            {
                offset = -cameraMovement;
            }
            offset += -cameraMovement * 5 * MouseState.Z / 120.0f;

            if (offset != 0)
                Context.AutoCamera = false;
            CameraY = MathUtil.Clamp(CameraY + offset, 0, MaxMovementDown);
        }

        private class LotteryContext
        {
            // reset
            public bool GameOver = false;
            public bool Started;
            public float GravityRate = 1.0f;
            public bool AutoCamera = false;
            public bool StopWorld = false;
            public List<Sprite> SpriteToRemove = new List<Sprite>();
            public List<Sprite> SpriteToAdd = new List<Sprite>();
            public Dictionary<string, PersonSprite> PersonSprites
                = new Dictionary<string, PersonSprite>();
            public Dictionary<string, Person> WinPersons
                = new Dictionary<string, Person>();

            // static
            public WordDef WordDef = JsonConvert.DeserializeObject<WordDef>(
                File.ReadAllText("./Resources/words.json"));
            public LotteryDatabase Database = new LotteryDatabase("state.json");

            // mid
            public PrizeDto CurrentPrize;

            public void Reset()
            {
                GameOver = false;
                WinPersons.Clear();
                Started = false;
                AutoCamera = false;
                StopWorld = false;
                GravityRate = 1.0f;
                SpriteToRemove.Clear();
                PersonSprites.Clear();
                SpriteToAdd.Clear();
            }
        }
    }
}
