using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization.Data;
using NUnit.Framework;
using NetStack.Serialization;
using static Heyworks.Realtime.Serialization.Data.TypedBitStreamTests;

namespace Heyworks.Realtime.Serialization
{
    public partial class TypedBitStreamTests
    {
        [Test]
        public void WriteGenericReadSpecific()
        {
            var outputGeneric = new TypedBitOutStream<TypedBitStreamTests>(500);
            var input = new SomeState { byte_1 = 13, enum_2 = MyByteEnum.B, f32_3 = 12345.6f, bool_4 = true, x_5 = new LimitedCustomPrimitive { value = 4 } };
            outputGeneric.WriteDiff(default, input);

            var outputFieldByField = new TypedBitOutStream<TypedBitStreamTests>(500);
            outputFieldByField.WriteDiff(default, input.byte_1);
            outputFieldByField.WriteDiff(default, input.enum_2);
            outputFieldByField.WriteDiff(default, input.f32_3);
            outputFieldByField.WriteBool(input.bool_4);
            outputFieldByField.WriteDiff(default, new LimitedCustomPrimitive { value = 4 });

            Assert.AreEqual(outputGeneric.BitsWritten, outputFieldByField.BitsWritten);

            var inputFromGeneric = new TypedBitInStream<TypedBitStreamTests>(500);
            var genericData = outputGeneric.ToArray();
            inputFromGeneric.FromArray(genericData);

            var inputFromByField = new TypedBitInStream<TypedBitStreamTests>(500);
            var byFieldData = outputFieldByField.ToArray();
            inputFromByField.FromArray(byFieldData);

            Assert.AreEqual(input, inputFromGeneric.ReadDiff<SomeState>(default));
            Assert.AreEqual(input, inputFromByField.ReadDiff<SomeState>(default));
            inputFromGeneric.Reset();
            inputFromByField.Reset();

            Assert.AreEqual(13, inputFromGeneric.Stream.u8BDiff(default));
            Assert.AreEqual(MyByteEnum.B, inputFromGeneric.ReadDiff<MyByteEnum>(default));
            Assert.AreEqual(12345.6f, inputFromGeneric.Stream.f32BDiff(default));
            Assert.AreEqual(true, inputFromGeneric.Stream.b());

            Assert.AreEqual(13, inputFromByField.Stream.u8BDiff(default));
            Assert.AreEqual(MyByteEnum.B, inputFromByField.ReadDiff<MyByteEnum>(default));
            Assert.AreEqual(12345.6f, inputFromByField.Stream.f32BDiff(default));
            Assert.AreEqual(true, inputFromByField.Stream.b());
        }

        [Test]
        public void ReadWriteCount()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(500);
            output.WriteIntCount(123, 124);
            output.WriteByteCount(1, (byte)10);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(500);
            input.FromArray(data);
            Assert.AreEqual(123, input.ReadIntCount(124));
            Assert.AreEqual(1, input.ReadByteCount(10));
        }

        [Test]
        public void Limits()
        {
            var noLimited = new TypedBitOutStream<TypedBitStreamTests>(128);
            var notValue = new NotCompressedStruct { angle = 123.01f, x = 42.222f, health = 1000, enumeration = MyByteEnum.B };
            noLimited.WriteOne(in notValue);
            Assert.AreEqual(82, noLimited.BitsWritten);

            var output = new TypedBitOutStream<TypedBitStreamTests>(128);
            var value = new WellCompressedStruct { angle = 123.01f, x = 42.222f, health = 1000, enumeration = MyByteEnum.B };
            output.WriteOne(in value);
            Assert.AreEqual(53, output.BitsWritten);
            var data = output.ToArray();

            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadOne<WellCompressedStruct>();

            Assert.AreEqual(123.01f, result.angle, 2);
            Assert.AreEqual(42.222f, result.x, 2);
            Assert.AreEqual(1000, result.health);
            Assert.AreEqual(MyByteEnum.B, result.enumeration);
        }

        [Test]
        public void SmallEnumsAreCompressed()
        {
            var writer = new TypedBitOutStream<TypedBitStreamTests>(128);
            var value1 = TwoSmallValueEnum.B;
            var value2 = TwoSmallValueEnum.A;
            writer.WriteOne(value1);
            writer.WriteOne(value2);
            Assert.AreEqual(2, writer.BitsWritten);
            var data = writer.ToArray();
            var reader = new TypedBitInStream<TypedBitStreamTests>(128);
            reader.FromArray(data);
            Assert.AreEqual(TwoSmallValueEnum.B, reader.ReadOne<TwoSmallValueEnum>());
            Assert.AreEqual(TwoSmallValueEnum.A, reader.ReadOne<TwoSmallValueEnum>());
        }

        [Test]
        public void IntWriteReadWorks()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(128);
            output.WriteInt(123);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadInt();
            Assert.AreEqual(123, result);
        }

        [Test]
        public void ConvetionalLimitOnCustomValues()
        {
            var noLimited = new TypedBitOutStream<TypedBitStreamTests>(128);
            noLimited.WriteOne(new CustomPrimitive { value = 42 });
            Assert.AreEqual(8, noLimited.BitsWritten);

            var limited = new TypedBitOutStream<TypedBitStreamTests>(128);
            limited.WriteOne(new LimitedCustomPrimitive { value = 42 });
            Assert.AreEqual(7, limited.BitsWritten);

            Assert.True(limited.BitsWritten < noLimited.BitsWritten);
        }

        [Test]
        public void Weapon()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            WeaponComponents baseline = default;
            WeaponComponents update = default;
            output.WriteDiff(in baseline, in update);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<WeaponComponents>(in baseline);
            Assert.AreEqual(update, result);
        }

        [Test]
        public void WeaponEnum()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            WeaponComponents baseline = default;
            WeaponComponents update = default;
            update.StateExpire.ExpireAt = short.MaxValue;
            update.Warmup.State = WarmupWeaponState.WarmingUp;
            output.WriteDiff(in baseline, in update);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<WeaponComponents>(in baseline);
            Assert.AreEqual(update, result);
        }

        [Test]
        public void WeaponAttack()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            WeaponComponents serverBaseLine = default;
            WeaponComponents serverUpdate = default;
            WeaponComponents clientBaseLine = default;
            WeaponComponents clientUpdate = default;

            serverUpdate.Base.State = WeaponState.Attacking;
            output.WriteDiff(in serverBaseLine, serverUpdate);
            Assert.AreEqual(WeaponState.Default, serverBaseLine.Base.State);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(375);
            input.FromArray(data);
            clientUpdate = input.ReadDiff(in clientBaseLine);
            Assert.AreEqual(WeaponState.Default, clientBaseLine.Base.State);
            Assert.AreEqual(WeaponState.Attacking, clientUpdate.Base.State);

            clientBaseLine = clientUpdate;

            serverBaseLine = serverUpdate;
            serverUpdate.Base.State = WeaponState.Attacking;
            serverUpdate.Consumable.AmmoInClip = int.MaxValue;
            serverUpdate.StateExpire.ExpireAt = int.MaxValue;
            output.WriteDiff(in serverBaseLine, in serverUpdate);
            Assert.AreEqual(WeaponState.Attacking, serverBaseLine.Base.State);
            data = output.ToArray();
            input.FromArray(data);

            clientUpdate = input.ReadDiff(in clientBaseLine);
            Assert.AreEqual(WeaponState.Attacking, clientBaseLine.Base.State);
            Assert.AreEqual(WeaponState.Attacking, clientUpdate.Base.State);
            clientBaseLine = clientUpdate;
            serverBaseLine = serverUpdate;

        }

        [Test]
        public void SerializeDeserializeLongByteComponent()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            LongByteComponent baseline = default;
            output.WriteDiff(in baseline, in baseline);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<LongByteComponent>(in baseline);
            Assert.AreEqual(baseline, result);
        }


        [Test]
        public void SerializeDeserializeLongByteComponentLong()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            LongByteComponent baseline = default;
            var updated = new LongByteComponent { a = long.MaxValue };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<LongByteComponent>(in baseline);
            Assert.AreEqual(updated, result);
        }

        [Test]
        public void SerializeDeserializeLongByteComponentBoth()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            var baseline = new LongByteComponent { b = byte.MaxValue };
            var updated = new LongByteComponent { a = long.MaxValue };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<LongByteComponent>(in baseline);
            Assert.AreEqual(updated, result);
        }

        [Test]
        public void SerializeDeserializeEnumIntComponentBoth()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            var baseline = new EnumIntComponent { a = MyByteEnum.B, b = 13 };
            var updated = new EnumIntComponent { a = MyByteEnum.A, b = ushort.MaxValue };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var otherBaseline = new EnumIntComponent { a = MyByteEnum.A, b = 12 };
            var result = input.ReadDiff<EnumIntComponent>(in otherBaseline);
            Assert.AreEqual(updated.b, result.b);
        }

        [Test]
        public void SerializeDeserializeBoolComponent()
        {
            var output = new TypedBitOutStream<BoolComponent>(8);
            var baseline = new BoolComponent { a = true };
            var updated = new BoolComponent { a = false };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<BoolComponent>(in baseline);
            Assert.AreEqual(updated.a, result.a);
        }

        [Test]
        public void SerializeDeserializeComponents()
        {
            var output = new TypedBitOutStream<BoolComponent>(8);
            var baseline = new Components { a = new DoubleFloatComponent { b = 1.0f }, b = new EnumIntComponent { a = MyByteEnum.A } };
            var updated = new Components { a = new DoubleFloatComponent { b = 2.0f }, b = new EnumIntComponent { a = MyByteEnum.B } };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<Components>(in baseline);
            Assert.AreEqual(updated.a, result.a);
            Assert.AreEqual(updated.b, result.b);
        }

        [Test]
        public void SerializeDeserializeFloatFloatComponent()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            var baseline = new FloatFloatComponent { a = 1.0f };
            var updated = new FloatFloatComponent { a = 1.0f, b = 2.0f };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<FloatFloatComponent>(in baseline);
            Assert.AreEqual(updated.a, result.a);
            Assert.AreEqual(updated.b, result.b);
        }

        [Test]
        public void SerializeDeserializeFloatFloatFloatComponent()
        {
            var output = new TypedBitOutStream<TypedBitStreamTests>(8);
            var baseline = new FloatFloatFloatComponent { a = 1.0f };
            var updated = new FloatFloatFloatComponent { a = 1.0f, b = 2.0f };
            output.WriteDiff(in baseline, in updated);
            var data = output.ToArray();
            var input = new TypedBitInStream<TypedBitStreamTests>(data);
            var result = input.ReadDiff<FloatFloatFloatComponent>(in baseline);
            Assert.AreEqual(updated.a, result.a);
            Assert.AreEqual(updated.b, result.b);
            Assert.AreEqual(updated.c, result.c);
        }
    }
}
