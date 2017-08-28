# Torutek.Auth

## `RequireAuthorizeAttributeFilter`

Checks that every Controller API has a `[Authorize]` or `[AllowAnonymous]` attribute. Throws an exception when you try to access them if they do not.

This is useful to help ensure that developers don't forget to add these.

### Usage

```cs
//public void ConfigureServices(IServiceCollection services)
services.AddMvc(config => { config.Filters.Add(new RequireAuthorizeAttributeFilter()); });
```

# Torutek.Auth.Jwt

Provides base functionality for JWT usage.

### Usage

```cs
//public void ConfigureServices(IServiceCollection services)
services.AddJwtServices(issuer, secretKey, Environment);

//public void Configure(IApplicationBuilder app, IHostingEnvironment env)
app.UseAuthentication();


JwtTokenFactory tokenFactory; //Resolve using DI
var token = _tokenFactory.IssueToken(userId, validFor, additionalClaims?);
```

Return the token to the client and have them provide it in the Authorize Header.

```
Authorize: Bearer tokenHere...
```

# Torutek.Auth.Passwordless

Provides Passwordless authentication.

### Usage

```cs
//public void ConfigureServices(IServiceCollection services)
services.AddPasswordless();


IPasswordlessService passwordless; //resolve using DI
var nonce = passwordless.GenerateNonce("userId or emailAddress or something");
var key = passwordless.GetKeyFromNonce(nonce);
```

The usual flow is

1. User enters their email address in a sign in form
2. App generates a nonce for their email (or a userid matching the email) (`GenerateNonce`)
3. App emails the nonce to the user (usually with a clickable link to automatically submit it)
4. User clicks link to submit nonce (or manually types it in)
5. App fetches userId/email using nonce (`GetKeyFromNonce`)
6. User identity is verified, app issues a JWT or cookie