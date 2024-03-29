namespace BeABachelor.Networking
{
    public interface IBinariable
    {
        byte[] ToBytes();
        void FromBytes(byte[] bytes);
    }
}