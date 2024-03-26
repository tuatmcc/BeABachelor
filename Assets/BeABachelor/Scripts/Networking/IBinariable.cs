namespace BeABachelor.Networking
{
    public interface IBinariable
    {
        byte[] ToBytes();
        T FromBytes<T>(byte[] bytes) where T : IBinariable;
    }
}