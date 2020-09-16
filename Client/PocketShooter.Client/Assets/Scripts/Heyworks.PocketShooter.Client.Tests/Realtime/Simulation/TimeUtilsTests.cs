using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    [TestFixture]
    public class TimeUtilsTests
    {
        private const double Epsilon = 0.0001;

        [SetUp]
        public void Setup()
        {
            LoggerFilterOptions options = new LoggerFilterOptions();

            var providers = new ILoggerProvider[] {
                new ConsoleLoggerProvider(),
                new FileLoggerProvider(new FileLoggerSettings("sample.log"))
            };

            var loggerFactory = new LoggerFactory(providers, options);

            //may need hook Debug.logger.logHandler
            MLog.Setup(loggerFactory, options, new LoggerConfiguration());
        }


        [Test]
        public void CalculateInitialTickersOffset_Values_Offset()
        {
            var offset = TimeUtils.CalculateInitialTickersOffset(60, 30, 30);

            Assert.AreEqual(7, offset);
        }

        [Test]
        public void CalculateInitialTickersOffset_ZeroCurrentTime_DefaultOffset()
        {
            var offset = TimeUtils.CalculateInitialTickersOffset(-100, 30, 30);

            Assert.AreEqual(5, offset);
        }

        [Test]
        public void CalculateInitialTickersOffset_LargeCurrentTime_MaxOffset()
        {
            var offset = TimeUtils.CalculateInitialTickersOffset(900, 30, 30);

            Assert.AreEqual(30, offset);
        }

        [Test]
        public void CalculateInitialTickersOffset_GreaterDuration_GreaterOffset()
        {
            var offset1 = TimeUtils.CalculateInitialTickersOffset(60, 30, 30);
            var offset2 = TimeUtils.CalculateInitialTickersOffset(110, 30, 30);

            Assert.Greater(offset2, offset1);
        }

        [Test]
        public void CalculateInitialTickersOffset_GreaterInterval_LessOffset()
        {
            var offset1 = TimeUtils.CalculateInitialTickersOffset(60, 30, 30);
            var offset2 = TimeUtils.CalculateInitialTickersOffset(60, 60, 30);

            Assert.Less(offset2, offset1);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_ZeroMeanGreaterRtt_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.5, 16, out var b1);
            var adj2 = TimeUtils.CalculateClientStopwatchAdjustment(120, 30, -1, 0, 0.5, 16, out var b2);

            Assert.Less(adj2, adj1);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_ZeroMeanGreaterInterval_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.5, 16, out var b1);
            var adj2 = TimeUtils.CalculateClientStopwatchAdjustment(60, 60, -1, 0, 0.5, 16, out var b2);

            Assert.Greater(adj2, adj1);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_ZeroMeanLessBuffer_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.5, 16, out var b1);
            var adj2 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -2, 0, 0.5, 16, out var b2);

            Assert.AreEqual(adj2, adj1, Epsilon);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_ZeroMeanGreaterMean_LessAdjustment()
        {
            var adj1 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.5, 16, out var b1);
            var adj2 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 1, 0.5, 16, out var b2);

            Assert.Less(adj2, adj1);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_ZeroMeanGreaterVariance_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.5, 16, out var b1);
            var adj2 = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 0, 0.8, 16, out var b2);

            Assert.Greater(adj2, adj1);
        }

        [Test]
        public void CalculateClientStopwatchAdjustment_NegativeBufferMean_PositiveAdjustment()
        {
            var adj = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, -10, 0.5, 16, out var b1);

            Assert.Greater(adj, 0);
        }


        [Test]
        public void CalculateClientStopwatchAdjustment_PositiveBufferMean_NegativeAdjustment()
        {
            var adj = TimeUtils.CalculateClientStopwatchAdjustment(60, 30, -1, 10, 0.5, 16, out var b1);

            Assert.Less(adj, 0);
        }

        [Test]
        public void CalculateWorldStopwatchAdjustment_BufferOverflowGreaterInterval_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, 10, 0.5, 1000, out var b1);
            var adj2 = TimeUtils.CalculateWorldStopwatchAdjustment(60, -1, 10, 0.5, 1000, out var b2);

            Assert.Greater(adj2, adj1);
        }

        [Test]
        public void CalculateWorldStopwatchAdjustment_BufferOverflowGreaterBuffer_SameAdjustment()
        {
            var adj1 = TimeUtils.CalculateWorldStopwatchAdjustment(30, -2, 10, 0.5, 1000, out var b1);
            var adj2 = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, 10, 0.5, 1000, out var b2);

            Assert.AreEqual(adj2, adj1, Epsilon);
        }

        [Test]
        public void CalculateWorldStopwatchAdjustment_BufferOverflowGreaterMean_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, 10, 0.5, 1000, out var b1);
            var adj2 = TimeUtils.CalculateWorldStopwatchAdjustment(60, -1, 10, 0.5, 1000, out var b2);

            Assert.Greater(adj2, adj1);
        }

        [Test]
        public void CalculateWorldStopwatchAdjustment_BufferOverflowGreaterVariance_GreaterAdjustment()
        {
            var adj1 = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, 10, 0.5, 1000, out var b1);
            var adj2 = TimeUtils.CalculateWorldStopwatchAdjustment(60, -1, 10, 0.8, 1000, out var b2);

            Assert.Greater(adj2, adj1);
        }


        [Test]
        public void CalculateWorldStopwatchAdjustment_NegativeBufferMean_NegativeAdjustment()
        {
            var adj = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, -10, 0.5, 1000, out var b1);

            Assert.Less(adj, 0);
        }


        [Test]
        public void CalculateWorldStopwatchAdjustment_PositiveBufferMean_PositiveAdjustment()
        {
            var adj = TimeUtils.CalculateWorldStopwatchAdjustment(30, -1, 10, 0.5, 1000, out var b1);

            Assert.Greater(adj, 0);
        }
    }
}
