using Microsoft.Extensions.Logging;
using System;
using Tekno.Application.Common.Interfaces;

namespace Tekno.Infrastructure
{
    public class LoggerAdapter<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message, params object[] args)
            => _logger.LogInformation(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.LogWarning(message, args);

        public void LogError(string message, params object[] args)
            => _logger.LogError(message, args);

        public void LogError(Exception exception, string message, params object[] args)
            => _logger.LogError(exception, message, args);

        public void LogWarning(Exception exception, string message, params object[] args)
            => _logger.LogWarning(exception, message, args);
    }
}
