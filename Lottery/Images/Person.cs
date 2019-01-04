using Newtonsoft.Json;
using System.IO;

namespace Lottery2019.Images
{
    public class Person
    {
        [JsonIgnore]
        public string Name { get; set; }

        public Sex Sex { get; set; }

        public string RawImage { get; set; }

        [JsonIgnore]
        public string PsdImage
            => $"{ImageDefines.PsdFolder}/{Name}.png";

        //public string SmallRawImage
        //    => $"{ImageDefines.SmallRawFolder}/{Name}.png";
        [JsonIgnore]
        public string SmallRawImage
            => RawImage;

        public static Person FromFile(string filename)
        {
            return new Person
            {
                Name = Path.GetFileNameWithoutExtension(filename),
                Sex = default(Sex), 
                RawImage = filename.Replace("\\", "/")
            };
        }
    }

    public enum Sex
    {
        Male, 
        Female
    }
}
