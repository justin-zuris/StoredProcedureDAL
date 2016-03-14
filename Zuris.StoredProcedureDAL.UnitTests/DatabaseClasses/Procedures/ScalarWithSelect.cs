namespace Zuris.SPDAL.UnitTests
{
    public class ScalarWithSelect : Procedure<ScalarTestParameters>
    {
        protected const string PROCEDURE_NAME = "[dbo].[__ScalarProcWithSelect]";
        public ScalarWithSelect(SampleCommandDataProvider cdp)
            : base(cdp)
        {
        }

        public override string CommandText { get { return PROCEDURE_NAME; } }
        
    }
}