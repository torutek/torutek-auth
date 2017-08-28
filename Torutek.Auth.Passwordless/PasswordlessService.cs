using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Torutek.Auth.Passwordless
{
	public interface IPasswordlessService
	{
		/// <summary>
		/// Generates a nonce for the given key (key will be returned when the returned nonce is provided to GetKeyFromNonce)
		/// </summary>
		Task GenerateNonce(string key);

		/// <summary>
		/// Returns the key belonging to the given nonce if it exists, otherwise null
		/// </summary>
		Task<string> GetKeyFromNonce(string nonce);
	}

	/// <summary>
	/// Manages Passwordless Authentication Concerns. key/nonce pairs are stored for 10 minutes and removed when first accessed.
	/// </summary>
	public class PasswordlessService : IPasswordlessService
	{
		//Based on https://github.com/PwdLess/PwdLess/blob/master/src/PwdLess/Services/AuthService.cs

		private readonly IDistributedCache _cache;

		private readonly RandomNumberGenerator _rng;

		/// <summary>
		/// Constructor
		/// </summary>
		public PasswordlessService(IDistributedCache cache)
		{
			_cache = cache;

			_rng = RandomNumberGenerator.Create();
		}

		/// <inheritdoc />
		public async Task GenerateNonce(string key)
		{
			var nonce = GenerateNonce();

			await _cache.SetAsync(nonce, Encoding.UTF8.GetBytes(key), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
		}

		/// <inheritdoc />
		public async Task<string> GetKeyFromNonce(string nonce)
		{
			var keyBytes = await _cache.GetAsync(nonce);

			if (keyBytes != null)
			{
				await _cache.RemoveAsync(nonce);
				return Encoding.UTF8.GetString(keyBytes);
			}

			return null;
		}

		/// <summary>
		/// Generate and return a 8 character nonce (lower case hex with numbers [a-f0-9])
		/// </summary>
		/// <returns></returns>
		private string GenerateNonce()
		{
			var bytes = new byte[16];
			_rng.GetBytes(bytes);

			var hash = SHA256.Create().ComputeHash(bytes);

			return BitConverter.ToString(hash).Replace("-", "").Substring(0, 8).ToLowerInvariant();
		}
	}

}
