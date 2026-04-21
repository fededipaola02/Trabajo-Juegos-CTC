namespace WordGame.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WordsGuessed { get; set; }
    }
}
