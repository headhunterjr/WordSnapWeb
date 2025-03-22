namespace WordSnapWeb.Models
{
    public class TestViewModel
    {
        public int CardsetId { get; set; }
        public string CardsetName { get; set; } = string.Empty;
        public IEnumerable<Card> Cards { get; set; } = new List<Card>();
        public double BestScore { get; set; }
    }
}
