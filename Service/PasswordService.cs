using System.Security.Cryptography;
using System.Text;

namespace SimpleSocialFeed
{

	public class PasswordService
	{
		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;             // salt
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // hash
			}
		}

		public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
		{
			using (var hmac = new HMACSHA512(storedSalt)) // ใช้ salt 
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(storedHash);
			}
		}

	}
}
