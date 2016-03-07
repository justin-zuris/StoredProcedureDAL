using System;

namespace Zuris.SPDAL.UnitTests
{
    public class FirstResultSetRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double? Cost { get; set; }
        public Guid UniqueId { get; set; }
        public long Checksum { get; set; }
    }
}