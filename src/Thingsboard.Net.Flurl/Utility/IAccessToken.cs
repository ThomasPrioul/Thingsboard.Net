﻿using System.Threading;
using System.Threading.Tasks;
using Thingsboard.Net.Utility;

namespace Thingsboard.Net.Flurl.Utility;

/// <summary>
/// Represents a policy used to save the token
/// </summary>
public interface IAccessToken
{
    /// <summary>
    /// Retrieve the token from the cache or from the server
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    Task<string> GetAccessTokenAsync(TbCredentials credentials, CancellationToken cancel=default);

    /// <summary>
    /// Clear the token from the cache
    /// </summary>
    /// <returns></returns>
    Task RemoveExpiredTokenAsync(TbCredentials credentials);
}