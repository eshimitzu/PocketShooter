using NUnit.Framework;
using UnityEngine;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    [TestFixture]
    public class TelemetryTests
    {
        private const double Epsilon = 0.01; 

        [Test]
        public void Telemetry_ConstantValues_MeanAndVariance()
        {
            var t = new Telemetry();

            for (int i = 0; i < 1000; i++)
            {
                t.Update(100);
            }

            Assert.AreEqual(100, t.Mean, Epsilon);
            Assert.AreEqual(0, t.Variance,Epsilon);
        }


        [Test]
        public void Telemetry_SimpleSequence_MeanAndVariance()
        {
            var t = new Telemetry();

            for (int i = 0; i < 1000; i++)
            {

                t.Update(-1);
                t.Update(1);
                t.Update(0);
            }

            Assert.AreEqual(0.0, t.Mean, 10 * Epsilon);
            Assert.AreEqual(2.0 / 3, t.Variance, 10 * Epsilon);

        }

        [Test]
        public void Telemetry_RandomSequence_MeanAndVariance()
        {
            var t = new Telemetry();

            Random.InitState(0);

            for (int i = 0; i < 1000; i++)
            {
                t.Update(Random.value);
            }

            Assert.AreEqual(0.5, t.Mean, 10 * Epsilon);
            Assert.AreEqual(0.25 , t.Variance, 10 * Epsilon);

        }
    }
}
