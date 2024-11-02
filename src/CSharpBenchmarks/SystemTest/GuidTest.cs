#if NET9_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.SystemTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class GuidTest
	{
		public SequentialGuidGenerator sequentialGuidGenerator;

		[GlobalSetup]
		public void Init()
		{
			sequentialGuidGenerator = new();
		}

		[Params(1024, 2048, 4096)]
		public int Count { get; set; }

		[Benchmark(Baseline = true)]
		public void Test1()
		{
			List<Guid> list = new List<Guid>(Count);
			for (int i = 0; i < Count; i++)
			{
				list.Add(Guid.NewGuid()); //第一种:DotNet自带,无序的Guid
			}
		}

		[Benchmark]
		public void Test2()
		{
			List<Guid> list = new List<Guid>(Count);
			for (int i = 0; i < Count; i++)
			{
				list.Add(Guid.CreateVersion7()); //第二种: .Net 9增加的,有序的
			}
		}

		[Benchmark]
		public void Test3()
		{
			List<Guid> list = new List<Guid>(Count);
			for (int i = 0; i < Count; i++)
			{
				list.Add(sequentialGuidGenerator.Create()); //ABP生成有序的Guid
			}
		}
	}
}

/// <summary>
/// Describes the type of a sequential GUID value.
/// </summary>
public enum SequentialGuidType
{
	/// <summary>
	/// The GUID should be sequential when formatted using the <see cref="Guid.ToString()" /> method.
	/// Used by MySql and PostgreSql.
	/// </summary>
	SequentialAsString,

	/// <summary>
	/// The GUID should be sequential when formatted using the <see cref="Guid.ToByteArray" /> method.
	/// Used by Oracle.
	/// </summary>
	SequentialAsBinary,

	/// <summary>
	/// The sequential portion of the GUID should be located at the end of the Data4 block.
	/// Used by SqlServer.
	/// </summary>
	SequentialAtEnd
}

/// <summary>
/// ABP 具体生成有序Guid的代码
/// </summary>
public class SequentialGuidGenerator
{
	private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

	public SequentialGuidGenerator()
	{
	}

	public Guid Create()
	{
		return Create(SequentialGuidType.SequentialAsString);
	}

	public Guid Create(SequentialGuidType guidType)
	{
		// We start with 16 bytes of cryptographically strong random data.
		var randomBytes = new byte[10];
		RandomNumberGenerator.GetBytes(randomBytes);

		// An alternate method: use a normally-created GUID to get our initial
		// random data:
		// byte[] randomBytes = Guid.NewGuid().ToByteArray();
		// This is faster than using RNGCryptoServiceProvider, but I don't
		// recommend it because the .NET Framework makes no guarantee of the
		// randomness of GUID data, and future versions (or different
		// implementations like Mono) might use a different method.

		// Now we have the random basis for our GUID.  Next, we need to
		// create the six-byte block which will be our timestamp.

		// We start with the number of milliseconds that have elapsed since
		// DateTime.MinValue.  This will form the timestamp.  There's no use
		// being more specific than milliseconds, since DateTime.Now has
		// limited resolution.

		// Using millisecond resolution for our 48-bit timestamp gives us
		// about 5900 years before the timestamp overflows and cycles.
		// Hopefully this should be sufficient for most purposes. :)
		long timestamp = DateTime.UtcNow.Ticks / 10000L;

		// Then get the bytes
		byte[] timestampBytes = BitConverter.GetBytes(timestamp);

		// Since we're converting from an Int64, we have to reverse on
		// little-endian systems.
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(timestampBytes);
		}

		byte[] guidBytes = new byte[16];

		switch (guidType)
		{
			case SequentialGuidType.SequentialAsString:
			case SequentialGuidType.SequentialAsBinary:

				// For string and byte-array version, we copy the timestamp first, followed
				// by the random data.
				Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
				Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

				// If formatting as a string, we have to compensate for the fact
				// that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
				// respectively.  That means that it switches the order on little-endian
				// systems.  So again, we have to reverse.
				if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
				{
					Array.Reverse(guidBytes, 0, 4);
					Array.Reverse(guidBytes, 4, 2);
				}

				break;

			case SequentialGuidType.SequentialAtEnd:

				// For sequential-at-the-end versions, we copy the random data first,
				// followed by the timestamp.
				Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
				Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
				break;
		}

		return new Guid(guidBytes);
	}
}
#endif

//public class GuidExtension
//{
//	/// <summary>Gets the value of the variant field for the <see cref="Guid" />.</summary>
//	/// <remarks>
//	///     <para>This corresponds to the most significant 4 bits of the 8th byte: 00000000-0000-0000-F000-000000000000. The "don't-care" bits are not masked out.</para>
//	///     <para>See RFC 9562 for more information on how to interpret this value.</para>
//	/// </remarks>
//	public int Variant => _d >> 4;

//	/// <summary>Gets the value of the version field for the <see cref="Guid" />.</summary>
//	/// <remarks>
//	///     <para>This corresponds to the most significant 4 bits of the 6th byte: 00000000-0000-F000-0000-000000000000.</para>
//	///     <para>See RFC 9562 for more information on how to interpret this value.</para>
//	/// </remarks>
//	public int Version => _c >>> 12;

//	/// <summary>Creates a new <see cref="Guid" /> according to RFC 9562, following the Version 7 format.</summary>
//	/// <returns>A new <see cref="Guid" /> according to RFC 9562, following the Version 7 format.</returns>
//	/// <remarks>
//	///     <para>This uses <see cref="DateTimeOffset.UtcNow" /> to determine the Unix Epoch timestamp source.</para>
//	///     <para>This seeds the rand_a and rand_b sub-fields with random data.</para>
//	/// </remarks>
//	public static Guid CreateVersion7() => CreateVersion7(DateTimeOffset.UtcNow);

//	/// <summary>Creates a new <see cref="Guid" /> according to RFC 9562, following the Version 7 format.</summary>
//	/// <param name="timestamp">The date time offset used to determine the Unix Epoch timestamp.</param>
//	/// <returns>A new <see cref="Guid" /> according to RFC 9562, following the Version 7 format.</returns>
//	/// <exception cref="ArgumentOutOfRangeException"><paramref name="timestamp" /> represents an offset prior to <see cref="DateTimeOffset.UnixEpoch" />.</exception>
//	/// <remarks>
//	///     <para>This seeds the rand_a and rand_b sub-fields with random data.</para>
//	/// </remarks>
//	public static Guid CreateVersion7(DateTimeOffset timestamp)
//	{
//		// This isn't the most optimal way, but we don't have an easy way to get
//		// secure random bytes in corelib without doing this since the secure rng
//		// is in a different layer.
//		Guid result = NewGuid();

//		// 2^48 is roughly 8925.5 years, which from the Unix Epoch means we won't
//		// overflow until around July of 10,895. So there isn't any need to handle
//		// it given that DateTimeOffset.MaxValue is December 31, 9999. However, we
//		// can't represent timestamps prior to the Unix Epoch since UUIDv7 explicitly
//		// stores a 48-bit unsigned value, so we do need to throw if one is passed in.

//		long unix_ts_ms = timestamp.ToUnixTimeMilliseconds();
//		ArgumentOutOfRangeException.ThrowIfNegative(unix_ts_ms, nameof(timestamp));

//		Unsafe.AsRef(in result._a) = (int)(unix_ts_ms >> 16);
//		Unsafe.AsRef(in result._b) = (short)unix_ts_ms;

//		Unsafe.AsRef(in result._c) = (short)((result._c & ~VersionMask) | Version7Value);
//		Unsafe.AsRef(in result._d) = (byte)((result._d & ~Variant10xxMask) | Variant10xxValue);

//		return result;
//	}
//}