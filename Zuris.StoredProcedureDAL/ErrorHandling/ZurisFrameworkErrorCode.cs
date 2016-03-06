namespace Zuris
{
    public enum ZurisFrameworkErrorCode
    {
        Unknown = 0,
        ProcedureDoesNotExist = 60001,
        ProcedureParameterMismatch = 60002,
        TransactionalFlushError = 60003,
        AdoNetProviderNotFound = 60004,
    }
}