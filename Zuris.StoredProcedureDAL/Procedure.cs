namespace Zuris.SPDAL
{
    public abstract class Procedure<P> : BaseProcedure, IProcedure<P>
        where P : new()
    {
        protected P _parameters = new P();

        public Procedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public virtual P Parameters { get { return _parameters; } }

        protected override IParameterGroup QueryParameters
        {
            get { return Parameters as IParameterGroup; }
        }
    }
}