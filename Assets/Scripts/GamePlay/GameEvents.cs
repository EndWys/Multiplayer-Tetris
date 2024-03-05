namespace TetrisNetwork
{
    public readonly struct MatchStartEvent : IEvent { }
    public readonly struct DeleteLineEvent : IEvent { }
    public readonly struct DetonateBombEvent : IEvent { }
    public readonly struct PlaceTerominoEvent : IEvent { }
}
