namespace BlazorCardGame.Shared
{
    public enum CardSuit
    {
        Coppe, // Hearts
        Denari, // Diamonds
        Spade, // Swords
        Bastoni // Clubs
    }

    public enum CardValue
    {
        Asso = 1, // Ace
        Tre = 3,
        Quattro = 4,
        Cinque = 5,
        Sei = 6,
        Sette = 7,
        Fante = 8, // Jack
        Cavallo = 9, // Knight
        Re = 10 // King
    }

    public class Card
    {
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }

        public Card(CardSuit suit, CardValue value)
        {
            Suit = suit;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value} di {Suit}";
        }
    }
}
