namespace Zuris.SPDAL.UnitTests
{
    public class ScalarWithReturn : Procedure<ScalarWithReturnTestParameters>
    {
        protected const string PROCEDURE_NAME = "[dbo].[__ScalarProcWithReturn]";
        public ScalarWithReturn(SampleCommandDataProvider cdp)
            : base(cdp)
        {
        }

        public override string CommandText { get { return PROCEDURE_NAME; } }
        
    }
}