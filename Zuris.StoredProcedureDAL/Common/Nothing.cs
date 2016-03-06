namespace Zuris
{
    public sealed class Nothing
    {
        public Nothing()
        {
        }

        private static readonly Nothing _atAll = new Nothing();

        public static Nothing AtAll { get { return _atAll; } }
    }
}