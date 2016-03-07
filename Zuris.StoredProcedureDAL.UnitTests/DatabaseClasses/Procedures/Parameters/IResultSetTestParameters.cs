namespace Zuris.SPDAL.UnitTests
{
    public interface IResultSetTestParameters
    {
        int? Id { get; set; }
        string Name { get; set; }
        bool? Enabled { get; set; }
        double? Cost { get; set; }
    }
}