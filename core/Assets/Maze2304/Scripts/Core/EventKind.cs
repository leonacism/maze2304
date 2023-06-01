namespace Core
{
    public abstract record EventKind();

    public sealed record GameOver(bool win) : EventKind;
}
