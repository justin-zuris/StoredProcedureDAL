using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Zuris.SPDAL.SqlServer
{
    public class SqlServerRetryManager : IEvaluateRetryable
    {
        public bool IsRetryable(Exception ex, out Dictionary<int, string> sourceErrorCodes)
        {
            if (ex is SqlException)
            {
                var sex = ex as SqlException;
                var sqlErrorCodesToRetry = new[]
                {
                    -2 /*Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding.*/
				    , 20 /*The instance of SQL Server you attempted to connect to does not support encryption. (PMcE: amazingly, this is transient)*/
				    , 64 /*A connection was successfully established with the server, but then an error occurred during the login process.*/
				    , 233 /*The client was unable to establish a connection because of an error during connection initialization process before login*/
				    , 10053 /*A transport-level error has occurred when receiving results from the server.*/
				    , 10054 /*A transport-level error has occurred when sending the request to the server.*/
				    , 10060 /*A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible.*/
				    , 40143 /*The service has encountered an error processing your request. Please try again.*/
				    , 40197 /*The service has encountered an error processing your request. Please try again.*/
				    , 40501 /*The service is currently busy. Retry the request after 10 seconds.*/
				    , 40613 /*Database '%.*ls' on server '%.*ls' is not currently available. Please retry the connection later.*/
			    };

                sourceErrorCodes = sex.Errors.Cast<SqlError>().Where(sqlError => sqlErrorCodesToRetry.Contains(sqlError.Number))
                    .ToDictionary(pair => pair.Number, pair => pair.Message);
            }
            else
            {
                sourceErrorCodes = new Dictionary<int, string>();
            }
            return sourceErrorCodes.Count > 0;
        }
    }
}