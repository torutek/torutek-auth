using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Torutek.Auth
{
	/// <summary>
	/// Mvc filter that throws an exception if the method or controller handling a request does not explictly state its authorization requirements.
	/// If you get an exception in here, you need to add an [Authorize(...)] attribute or [AllowAnonymous] attribute to your handler method or controller
	/// </summary>
	public class RequireAuthorizeAttributeFilter : IAsyncAuthorizationFilter
	{
		/// <summary>
		/// Checks for the required attributes
		/// </summary>
		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if (context.ActionDescriptor is ControllerActionDescriptor)
			{
				var desc = (ControllerActionDescriptor)context.ActionDescriptor;

				var allAttrs = desc.MethodInfo.CustomAttributes.Concat(desc.ControllerTypeInfo.CustomAttributes);
				if (!allAttrs.Any(attr => IsSameOrSubclass(typeof(AuthorizeAttribute), attr.AttributeType) || IsSameOrSubclass(typeof(AllowAnonymousAttribute), attr.AttributeType)))
					throw new Exception("You (Developer) need to add an [Authorize] or [AllowAnonymous] attribute on this controller or method");
			}
			else
			{
				throw new Exception("Not sure how to enforce [Authorize]/[AllowAnonymous] Attributes on this thing");
			}
			return Task.FromResult(true);
		}

		private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
		{
			return potentialDescendant.IsSubclassOf(potentialBase)
				   || potentialDescendant == potentialBase;
		}
	}
}
