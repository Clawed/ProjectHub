namespace ProjectHub.Util.Enclosure.Algorithm
{
    public class GametileUpdate
    {
        public GametileUpdate(int _X, int _Y, byte _Value)
        {
            X = _X;
            Y = _Y;
            Value = _Value;
        }

        public byte Value { get; private set; }
        public int Y { get; private set; }
        public int X { get; private set; }
    }
}