using Microsoft.Extensions.DependencyInjection;

namespace Torutek.Auth.Passwordless
{
	/// <summary>
	/// Extension methods
	/// </summary>
    public static class PasswordlessExtensions
	{
		/// <summary>
		/// Registers PasswordlessService and its required memory cache (can be disabled)
		/// </summary>
		public static void AddPasswordless(this IServiceCollection services, bool addDistributedMemoryCache = true)
		{
			if (addDistributedMemoryCache)
				services.AddDistributedMemoryCache(); // CAN REPLACE WITH AddDistrbutedRedisCache for Redis support
			services.Add(ServiceDescriptor.Singleton<PasswordlessService, PasswordlessService>());
		}
	}
}
