using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace SimpleSocialFeed
{

	public class PasswordService
	{
		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			
			passwordSalt = GenerateSalt(16);             // salt
			using (var arg = new Argon2id(Encoding.UTF8.GetBytes(password)))
			{
				arg.Salt = passwordSalt;
				arg.DegreeOfParallelism = 8; // Number of threads
				arg.MemorySize = 65536; // 64 MB of memory
				arg.Iterations = 4; // Number of iterations
				passwordHash = arg.GetBytes(32);			 // hash
			}
		}

		static byte[] GenerateSalt(int length)
		{
			return RandomNumberGenerator.GetBytes(length);
		}

		public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
		{
			using (var arg = new Argon2id(Encoding.UTF8.GetBytes(password))) // ใช้ salt 
			{
				arg.Salt = storedSalt;
				arg.DegreeOfParallelism = 8; // Number of threads
				arg.MemorySize = 65536; // 64 MB of memory
				arg.Iterations = 4; // Number of iterations
				var newHash = arg.GetBytes(32);
				return newHash.SequenceEqual(storedHash);
			}
		}

	}
}
