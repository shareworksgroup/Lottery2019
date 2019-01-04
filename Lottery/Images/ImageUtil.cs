using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WIC = SharpDX.WIC;
using D2D = SharpDX.Direct2D1;
using Lottery2019.UI.Details;
using SharpDX;
using Newtonsoft.Json;
using Lottery2019.State;

namespace Lottery2019.Images
{
    public static class ImageUtil
    {
        public static void ProcessImages()
        {
            if (!Directory.Exists(ImageDefines.PsdFolder))
            {
                EnsureDirectory(ImageDefines.PsdFolder);
                DoImageProcessWIC();
            }
        }

        public static void WritePersonJson()
        {
            var persons = PersonProvider.GetPersonsFromImage()
                .ToDictionary(k => k.Name, v => v);
            File.WriteAllText("person.json", JsonConvert.SerializeObject(new
            {
                Persons = persons, 
            }, Formatting.Indented));
        }
        
        private static void DoImageProcessWIC()
        {
            using (var wic = new WIC.ImagingFactory())
            using (var d2dFactory = new D2D.Factory(D2D.FactoryType.MultiThreaded))
            {
                var db = new LotteryDatabase("state.json");
                Parallel.ForEach(db.GetAllPerson(), person =>
                {
                    using (var converter = DirectXTools.CreateWicImage(wic, person.RawImage))
                    {
                        DrawPsd(person, wic, d2dFactory, converter);
                        //DrawSmallRaw(person, wic, d2dFactory, converter);
                    }   
                });
            }
        }

        private static void DrawSmallRaw(Person person, WIC.ImagingFactory wic, D2D.Factory d2dFactory, 
            WIC.FormatConverter converter)
        {
            var whRate = 1.0f * converter.Size.Width / converter.Size.Height;
            var smallRawSize = new Vector2(whRate * ImageDefines.SmallRawY, ImageDefines.SmallRawY);
            var scale = ImageDefines.SmallRawY / converter.Size.Height;

            using (var wicBitmap = new WIC.Bitmap(wic,
                (int)smallRawSize.X, (int)smallRawSize.Y,
                WIC.PixelFormat.Format32bppPBGRA,
                WIC.BitmapCreateCacheOption.CacheOnDemand))
            using (var target = new D2D.WicRenderTarget(d2dFactory,
                wicBitmap, new D2D.RenderTargetProperties()))
            using (var bmp = D2D.Bitmap.FromWicBitmap(target, converter))
            using (var bmpBrush = new D2D.BitmapBrush(target, bmp))
            {
                target.BeginDraw();
                target.Transform = Matrix3x2.Scaling(scale, scale);
                target.DrawBitmap(bmp, 1.0f, D2D.BitmapInterpolationMode.Linear);
                target.EndDraw();

                DirectXTools.SaveD2DBitmap(wic, wicBitmap, person.SmallRawImage);
            }
        }

        private static void DrawPsd(Person person, WIC.ImagingFactory wic, D2D.Factory d2dFactory,
            WIC.FormatConverter converter)
        {
            using (var wicBitmap = new WIC.Bitmap(wic,
                (int)ImageDefines.Size, (int)ImageDefines.Size,
                WIC.PixelFormat.Format32bppPBGRA,
                WIC.BitmapCreateCacheOption.CacheOnDemand))
            using (var target = new D2D.WicRenderTarget(d2dFactory,
                wicBitmap, new D2D.RenderTargetProperties()))
            using (var color = new D2D.SolidColorBrush(target, SexColor[person.Sex]))
            using (var bmp = D2D.Bitmap.FromWicBitmap(target, converter))
            using (var bmpBrush = new D2D.BitmapBrush(target, bmp))
            {
                target.BeginDraw();
                var offset = (ImageDefines.Size - ImageDefines.RealSize) / 2;
                bmpBrush.Transform = Matrix3x2.Scaling(
                    ImageDefines.RealSize / bmp.Size.Width,
                    ImageDefines.RealSize / (bmp.Size.Height - 497.0f))
                    * Matrix3x2.Translation(offset, offset);
                target.FillEllipse(new D2D.Ellipse(
                    new Vector2(ImageDefines.Size / 2.0f, ImageDefines.Size / 2.0f),
                    ImageDefines.RealSize / 2.0f,
                    ImageDefines.RealSize / 2.0f),
                    bmpBrush);
                target.DrawEllipse(new D2D.Ellipse(
                    new Vector2(ImageDefines.Size / 2.0f, ImageDefines.Size / 2.0f),
                    ImageDefines.RealSize / 2.0f,
                    ImageDefines.RealSize / 2.0f),
                    color, ImageDefines.LineWidth);
                target.EndDraw();

                DirectXTools.SaveD2DBitmap(wic, wicBitmap, person.PsdImage);
            }
        }

        private static Dictionary<Sex, Color4> SexColor = new Dictionary<Sex, Color4>
        {
            [Sex.Male] = SharpDX.Color.Blue.ToColor4(),
            [Sex.Female] = SharpDX.Color.Orange.ToColor4(),
        };

        private static void EnsureDirectory(string T30Directory)
        {
            if (Directory.Exists(T30Directory))
            {
                foreach (var file in Directory.EnumerateFiles(T30Directory))
                {
                    File.Delete(file);
                }
            }
            Directory.CreateDirectory(T30Directory);
        }
    }
}
