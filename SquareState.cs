namespace Gridlock2
{
    // An enumerator for the possible states a game square can be in.
    public enum SquareState
    {
        Free,   // Represents a free square
        Player1,    // Player one's square
        Player2,    // Player two's square
        Compromised1,   // Player one's compromised square
        Compromised2    // Player two's compromized square.
    }
}
