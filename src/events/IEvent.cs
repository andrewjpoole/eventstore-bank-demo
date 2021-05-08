namespace events
{
    public interface IEvent
    {
        string StreamName();
        int Version();
    }
}