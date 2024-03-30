public struct TrackedCondition
{
    private bool current;
    private bool previous;

    public readonly bool Rising => current && !previous;
    public readonly bool Falling => !current && previous;
    public readonly bool Remained => current == previous;
    public readonly bool Changed => current != previous;

    public bool Value
    {
        readonly get => current;
        set
        {
            previous = current;
            current = value;
        }
    }
    public readonly bool Previous => previous;
}
