using System;
using AJP.MediatrEndpoints;
using Infrastructure.StatisticsGatherer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class DefaultRequestProcessors : IMediatrEndpointsProcessors
{
    public Action<HttpContext, ILogger> PreProcess {get; set;}
    public Action<HttpContext, TimeSpan, ILogger> PostProcess {get; set;}
    public Action<Exception, HttpContext, ILogger> ErrorProcess {get; set;}

    public DefaultRequestProcessors(IStatisticsTaskQueue statisticsTaskQueue)
    {
        PreProcess = (context, logger) =>
        {
            var correlationId = GetOrCreateCorrelationId(context);
            logger.LogDebug($"PreProcess-> {context.Request.Method} {context.Request.Path} request received with queryString:{context.Request.QueryString} and CorrelationId:{correlationId}");

            context.Response.Headers.Add(Constants.HeaderKeys_CorrelationId, correlationId);

            var contextAccessor = context.RequestServices.GetService<IEndpointContextAccessor>();
            if (contextAccessor != null)
                contextAccessor.CurrentContext = context;
        };

        PostProcess = (context, elapsed, logger) =>
        {
            statisticsTaskQueue.QueueStatisticsWorkItem((DateTime.UtcNow, elapsed.Ticks));

            context.Response.Headers.Add(Constants.HeaderKeys_Node, Environment.MachineName);

            logger.LogDebug("PostProcess-> Sending {statusCode} response, request took {elapsedMilliseconds}ms", context.Response.StatusCode, elapsed.Milliseconds);
        };

        ErrorProcess = (ex, context, logger) =>
        {
            logger.LogError($"ErrorProcess-> Message:{ex}");
            throw ex;
        };
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        string correlationId;
        if(context.Request.Headers.ContainsKey(Constants.HeaderKeys_CorrelationId))
        {
            correlationId = context.Request.Headers[Constants.HeaderKeys_CorrelationId].ToString();
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Add(Constants.HeaderKeys_CorrelationId, correlationId);
        }
        return correlationId;
    }
}