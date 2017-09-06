using System;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Torutek.Auth.Jwt
{
	/// <summary>
	/// Provides Method to register Jwt Services
	/// </summary>
	public static class JwtExtensions
	{
		/// <summary>
		/// Adds a JwtBearerOptions and JwtTokenFactory instance to the IServiceCollection
		/// </summary>
		/// <param name="serviceCollection"></param>
		/// <param name="issuer">The Issuer field for the JWTs</param>
		/// <param name="secretKey">The secret key to be used to sign and validate JWTs. Should be big and kept secret</param>
		/// <param name="environment"></param>
		/// <param name="events">Optionally, you can provide an events object so you can handle JWT related events</param>
		public static void AddJwtServices(this IServiceCollection serviceCollection, string issuer, string secretKey, IHostingEnvironment environment, JwtBearerEvents events = null)
		{
			var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
			var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

			serviceCollection.Add(ServiceDescriptor.Singleton(new JwtTokenFactory(issuer, signingCredentials)));
			serviceCollection.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(o => CreateJwtBearerOptions(o, issuer, signingKey, environment, events));
		}

		private static void CreateJwtBearerOptions(JwtBearerOptions o, string issuer, SymmetricSecurityKey signingKey, IHostingEnvironment environment, JwtBearerEvents events)
		{
			if (environment.IsDevelopment())
			{
				o.RequireHttpsMetadata = false;
			}

			o.Audience = issuer;
			o.Authority = null;

			if (events != null)
				o.Events = events;

			o.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = issuer,

				ValidateAudience = true,
				ValidAudience = issuer,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

				RequireExpirationTime = true,
				ValidateLifetime = true,

				ClockSkew = TimeSpan.Zero,

				NameClaimType = ClaimTypes.NameIdentifier
			};
		}
	}

}
