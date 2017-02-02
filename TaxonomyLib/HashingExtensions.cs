using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	internal static class HashingExtensions
	{
		public static async Task<byte[]> ComputeHashAsync(this HashAlgorithm hash, Stream inputStream)
		{
			var buffer = new byte[4096];
			while(true)
			{
				var readCount = await inputStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
				if(readCount == 0)
					break;
				hash.TransformBlock(buffer, 0, readCount, buffer, 0);
			}
			hash.TransformFinalBlock(buffer, 0, 0);
			return hash.Hash;
		}

		private static void Test(string data)
		{
			using(var memoryStream1 = new MemoryStream(Encoding.ASCII.GetBytes(data)))
			using(var memoryStream2 = new MemoryStream(Encoding.ASCII.GetBytes(data)))
			using(var hash1 = new SHA1Managed())
			using(var hash2 = new SHA1Managed())
			{
				if(!hash1.ComputeHash(memoryStream1).SequenceEqual(hash2.ComputeHashAsync(memoryStream2).Result))
				{
					throw new Exception("WROOONG");
				}
			}
		}

		private static void Main()
		{
			Test("");
			Test("a");
			Test("z");
			Test(new string('c', 4095));
			Test(new string('c', 4096));
			Test(new string('a', 4097));
			Test(new string('c', 4197));
		}
	}
}
