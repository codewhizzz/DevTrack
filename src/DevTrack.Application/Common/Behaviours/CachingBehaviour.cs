using DevTrack.Application.Common.Attributes;
using DevTrack.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Behaviours;

public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

    public CachingBehaviour(ICacheService cacheService, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheableAttribute = request.GetType()
            .GetCustomAttributes(typeof(CacheableAttribute), false)
            .FirstOrDefault() as CacheableAttribute;

        if (cacheableAttribute == null)
        {
            return await next();
        }

        var cacheKey = GenerateCacheKey(request);
        
        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            _logger.LogInformation("Response retrieved from cache. Key: {CacheKey}", cacheKey);
            return cachedResponse;
        }

        var response = await next();

        await _cacheService.SetAsync(
            cacheKey, 
            response, 
            TimeSpan.FromMinutes(cacheableAttribute.DurationInMinutes), 
            cancellationToken);

        _logger.LogInformation("Response cached. Key: {CacheKey}, Duration: {Duration} minutes", 
            cacheKey, cacheableAttribute.DurationInMinutes);

        return response;
    }

    private string GenerateCacheKey(TRequest request)
    {
        var requestName = request.GetType().Name;
        var requestJson = JsonSerializer.Serialize(request);
        return $"{requestName}:{requestJson}";
    }
}