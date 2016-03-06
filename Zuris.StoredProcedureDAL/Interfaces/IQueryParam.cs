namespace Zuris.SPDAL
{
    public interface IQueryParam<T> : IObjectQueryParam
    {
        T Value { get; set; }

        void ClearValue();
    }
}