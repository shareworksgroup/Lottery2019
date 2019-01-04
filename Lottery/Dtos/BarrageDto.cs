namespace Lottery2019.Dtos
{
    public class BarrageDto
    {
        public string UserName { get; set; }

        public string Content { get; set; }

        public string Color { get; set; }

        public string Text => $"{UserName}:{Content}";
    }
}
