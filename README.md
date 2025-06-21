# backend4frontend


BFF Logout Endpoint

The `/bff/logout` endpoint signs out of the appropriate ASP.NET Core authentication schemes to both delete the BFF’s session cookie and to sign out from the remote identity provider. 


## BFF Back-Channel Logout Endpoint

The `/bff/backchannel` endpoint is an implementation of the `OpenID Connect Back-Channel Logout` specification. The remote identity provider can use this endpoint to end the BFF’s session via a server to server call, without involving the user’s browser. This design avoids problems with 3rd party cookies associated with front-channel logout.


### Typical Usage
The back-channel logout endpoint is invoked by the remote identity provider when it determines that sessions should be ended. IdentityServer will send back-channel logout requests if you configure your client’s BackChannelLogoutUri. When a session ends at IdentityServer, any client that was participating in that session that has a back-channel logout URI configured will be sent a back-channel logout request. This typically happens when another application signs out. Expiration of IdentityServer server side sessions can also be configured to send back-channel logout requests, though this is disabled by default.

### Dependencies
The back-channel logout endpoint depends on server-side sessions in the BFF, which must be enabled to use this endpoint. Note that such server-side sessions are distinct from server-side sessions in IdentityServer.


### Revoke All Sessions
Back-channel logout tokens include a sub (subject ID) and sid (session ID) claim to describe which session should be revoked. By default, the back-channel logout endpoint will only revoke the specific session for the given subject ID and session ID. Alternatively, you can configure the endpoint to revoke every session that belongs to the given subject ID by setting the BackchannelLogoutAllUserSessions option to true.

```c#
/// <summary>
/// User session store
/// </summary>
public interface IUserSessionStore
{
    /// <summary>
    /// Retrieves a user session
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task<UserSession?> GetUserSessionAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user session
    /// </summary>
    /// <param name="session"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task CreateUserSessionAsync(UserSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user session
    /// </summary>
    /// <param name="key"></param>
    /// <param name="session"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task UpdateUserSessionAsync(string key, UserSessionUpdate session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user session
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task DeleteUserSessionAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries user sessions based on the filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task<IReadOnlyCollection<UserSession>> GetUserSessionsAsync(UserSessionsFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes user sessions based on the filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task DeleteUserSessionsAsync(UserSessionsFilter filter, CancellationToken cancellationToken = default);
}
```


In security and authentication, a principal is a fancy word for:

    “an entity (user, app, service) that can be authenticated.”


So in basic terms:

- A user who logs in is a principal.
- A service with a token is also a principal.    

A claim is a statement about the principal. Each claim represents one fact, like:

- "sub" = "user123"
- "email" = "jane@example.com"
- "role" = "admin"


## Token Management

The token management library does essentially two things:

- stores access and refresh tokens in the current session
- refreshes access tokens automatically at the token service when needed

[bff](https://www.kallemarjokorpi.fi/blog/request-routing-in-bff/)


[OAuth 2.0 Security Best Current Practice](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-2.2.2)