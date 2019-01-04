using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Lottery2019.UI.Feature
{
    public class SystemPrizeProvider
    {
        public static readonly Dictionary<string, PrizeDto> Prizes =
            JsonConvert.DeserializeObject<Dictionary<string, PrizeDto>>(
                File.ReadAllText("./Resources/prizes.json"));

        static SystemPrizeProvider()
        {
            foreach (var prize in Prizes)
            {
                prize.Value.Id = prize.Key;
            }
        }
        
        public static readonly PrizeDto Default = new PrizeDto
        {
            Title = "default",
            Content = "Unknown",
            Count = 3,
            Id = "default",
            Image = "./Resources/html/img/default.png"
        };
    }
}
