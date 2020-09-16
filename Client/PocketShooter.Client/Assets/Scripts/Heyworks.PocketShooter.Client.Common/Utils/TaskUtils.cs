using System;
using Microsoft.Extensions.Logging;
using UniRx.Async;

namespace Heyworks.PocketShooter.Utils
{
    public static class TaskUtils
    {
        public static async UniTask RetryOnExceptionAsync(TimeSpan[] delays, Func<UniTask> operation, ILogger logger)
        {
            await RetryOnExceptionAsync<Exception>(delays, operation, logger);
        }

        public static async UniTask RetryOnExceptionAsync<TException>(
            TimeSpan[] delays,
            Func<UniTask> operation,
            ILogger logger) where TException : Exception
        {
            int times = delays.Length;

            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times));
            }

            for (var attempts = 1;; attempts++)
            {
                try
                {
                    await operation();
                    break;
                }
                catch (TException ex)
                {
                    if (attempts > times)
                    {
                        throw;
                    }

                    TimeSpan delay = delays[attempts - 1];
                    logger.LogWarning(
                        ex,
                        "Exception on attempt {attempts} of {times}. Will retry after {delay}.",
                        attempts,
                        times,
                        delay);
                    await UniTask.Delay(delay);
                }
            }
        }
    }
}