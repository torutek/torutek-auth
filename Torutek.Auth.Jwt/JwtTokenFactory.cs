using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Torutek.Auth.Jwt
{
	/// <summary>
	/// Factory for Jwt Tokens based on our signing credentials
	/// </summary>
	public class JwtTokenFactory
	{
		private readonly string _issuer;
		private readonly SigningCredentials _signingCredentials;

		/// <summary>
		/// Constructor
		/// </summary>
		public JwtTokenFactory(string issuer, SigningCredentials signingCredentials)
		{
			_issuer = issuer;
			_signingCredentials = signingCredentials;
		}

		/// <summary>
		/// Issue a token for the given subject and validity time
		/// </summary>
		public string IssueToken(string subject, TimeSpan validFor, params Claim[] additionalClaims)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, subject),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(5))).ToString(), ClaimValueTypes.Integer64)
			}.Concat(additionalClaims);

			// Create the JWT security token and encode it.
			var jwt = new JwtSecurityToken(
				issuer: _issuer,
				audience: _issuer,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.Add(validFor),
				signingCredentials: _signingCredentials
			);

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return encodedJwt;
		}

		private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
	}
}
