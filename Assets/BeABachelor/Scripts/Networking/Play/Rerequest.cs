namespace BeABachelor.Networking.Play
{
    public record Rerequest : InteractionObject
    {
        public Rerequest(int tickCount) => (ReRequestFlag, TickCount) = (true, tickCount); }
}