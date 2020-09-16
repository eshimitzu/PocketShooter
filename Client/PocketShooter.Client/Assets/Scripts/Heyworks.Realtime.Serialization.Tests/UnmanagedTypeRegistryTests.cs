using System;
using System.Linq;
using Heyworks.Realtime.Serialization.Data;
using NUnit.Framework;

namespace Heyworks.Realtime.Serialization
{
    public class UnmanagedTypeRegistryTests
    {
        [Test]
        public void RegisterLongByte()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, LongByteComponent>.Parse();
            Assert.AreEqual(infos.Count(), 2);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 8);
            Assert.AreEqual(infos[0].Type, typeof(long));
            Assert.AreEqual(infos[1].Offset, 8);
            Assert.AreEqual(infos[1].Size, 1);
            Assert.AreEqual(infos[1].Type, typeof(byte));
        }

        [Test]
        public void RegisterEnumIntComponent()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, EnumIntComponent>.Parse();
            Assert.AreEqual(infos.Count(), 2);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 1);
            Assert.AreEqual(infos[0].Type, typeof(MyByteEnum));
            Assert.AreEqual(4, infos[1].Offset);
            Assert.AreEqual(infos[1].Size, 4);
            Assert.AreEqual(infos[1].Type, typeof(int));
        }

        [Test]
        public void RegisterZero()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, EmptyComponent>.Parse();
            Assert.AreEqual(infos.Count(), 0);
        }

        [Test]
        public void RegisterByteEnumComponent()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, EnumComponent>.Parse();
            Assert.AreEqual(infos.Count(), 1);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 1);
            Assert.AreEqual(infos[0].Type, typeof(MyByteEnum));
        }

        [Test]
        public void RegisterFloatFloatFloatComponent()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, FloatFloatFloatComponent>.Parse();
            Assert.AreEqual(infos.Count(), 3);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 4);
            Assert.AreEqual(infos[0].Type, typeof(float));
            Assert.AreEqual(8, infos[2].Offset);
            Assert.AreEqual(infos[2].Size, 4);
            Assert.AreEqual(infos[2].Type, typeof(float));
        }

        [Test]
        public void RegisterFloatFloatComponent()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, FloatFloatComponent>.Parse();
            Assert.AreEqual(infos.Count(), 2);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 4);
            Assert.AreEqual(infos[0].Type, typeof(float));
            Assert.AreEqual(infos[1].Offset, 4);
            Assert.AreEqual(infos[1].Size, 4);
            Assert.AreEqual(infos[1].Type, typeof(float));
        }

        [Test]
        public void RegisterComponents()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, Components>.Parse();
            Assert.AreEqual(4, infos.Count());
            Assert.AreEqual(0, infos[0].Offset);
            Assert.AreEqual(8, infos[1].Offset);
            Assert.AreEqual(16, infos[2].Offset);
            Assert.AreEqual(20, infos[3].Offset);
        }

        [Test]
        public void RegisterDoubleFloatComponent()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, DoubleFloatComponent>.Parse();
            Assert.AreEqual(infos.Count(), 2);
            Assert.AreEqual(infos[0].Offset, 0);
            Assert.AreEqual(infos[0].Size, 8);
            Assert.AreEqual(infos[0].Type, typeof(double));
            Assert.AreEqual(infos[1].Offset, 8);
            Assert.AreEqual(infos[1].Size, 4);
            Assert.AreEqual(infos[1].Type, typeof(float));
        }

        [Test]
        public void RegisterByteEnum()
        {
            var infos = UnmanagedTypeRegistry<UnmanagedTypeRegistryTests, MyByteEnum>.Parse();
            Assert.AreEqual(infos.Count(), 1);
        }
    }
}
