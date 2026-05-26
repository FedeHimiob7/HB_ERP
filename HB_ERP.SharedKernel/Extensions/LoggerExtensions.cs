
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Extensions
{
    public static class LoggerExtensions
    {        
        public static void LogWithData(this ILogger logger, LogLevel level, string message, object data)
        {
            var serializedData = JsonSerializer.Serialize(data);

            using (LogContext.PushProperty("Payload", serializedData))
            {
                logger.Log(level, message);
            }
        }
    }
}
