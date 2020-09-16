using System;

namespace System.Threading
{
    // https://github.com/dotnet/corefx/blob/3b24c535852d19274362ad3dbc75e932b7d41766/src/Common/src/CoreLib/System/Threading/ReaderWriterLockSlim.cs#L233 
    /// <summary>
    /// Common timeout support.
    /// </summary>
    public struct TimeoutTracker
    {
        private readonly int _total;
        private readonly int _start;

        public TimeoutTracker(TimeSpan timeout)
        {
            long ltm = (long)timeout.TotalMilliseconds;
            if (ltm < -1 || ltm > (long)int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timeout));
            _total = (int)ltm;
            if (_total != -1 && _total != 0)
                _start = Environment.TickCount;
            else
                _start = 0;
        }

        public int RemainingMilliseconds
        {
            get
            {
                if (_total == -1 || _total == 0)
                    return _total;

                int elapsed = Environment.TickCount - _start;
                // elapsed may be negative if TickCount has overflowed by 2^31 milliseconds.
                if (elapsed < 0 || elapsed >= _total)
                    return 0;

                return _total - elapsed;
            }
        }

        public bool IsExpired
        {
            get
            {
                return RemainingMilliseconds == 0;
            }
        }
    }
}