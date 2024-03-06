namespace TetrisNetwork
{
    public enum GameEventType {
        MatchStart = 0,
        DeleteLine = 1,
        DetonateBomb = 2,
        PlaceTeromino = 3,
    }

    public readonly struct MatchStartEvent : IEvent { }
    public readonly struct DeleteLineEvent : IEvent { }
    public readonly struct DetonateBombEvent : IEvent { }
    public readonly struct PlaceTerominoEvent : IEvent { }
}
