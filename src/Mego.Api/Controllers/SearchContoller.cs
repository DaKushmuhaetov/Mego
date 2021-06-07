using Mego.Api.Views;
using Mego.Domain.Infrastructure.Services;
using Mego.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mego.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchContoller : ControllerBase
    {
        private readonly TimeSpan _maxSearchDuration;
        private readonly ILogger _logger;

        public SearchContoller(IOptions<SearchServiceOptions> options,
            ILogger<SearchContoller> logger)
        {
            _logger = logger;
            _maxSearchDuration = TimeSpan.FromSeconds(options.Value.MaxSearchDurationInSeconds);
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <response code="200">Successfully</response>
        [HttpGet("/search")]
        [ProducesResponseType(typeof(ConcurrentBag<SearchResultView>), 200)]
        public async Task<IActionResult> Search(
            CancellationToken cancellationToken,
            [FromQuery] string query,
            [FromServices] ExternalA externalA,
            [FromServices] ExternalB externalB,
            [FromServices] ExternalC externalC,
            [FromServices] ExternalD externalD)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            ConcurrentBag<SearchResultView> searchResults = new ConcurrentBag<SearchResultView>();

            await Task.WhenAll(
                Task.Run(async () => {
                    var taskA = Execute(externalA, nameof(externalA), searchResults, cancellationToken);
                    if (await Task.WhenAny(taskA, Task.Delay(_maxSearchDuration)) == taskA) { }
                }),
                Task.Run(async () => {
                    var taskB = Execute(externalB, nameof(externalB), searchResults, cancellationToken);
                    if (await Task.WhenAny(taskB, Task.Delay(_maxSearchDuration)) == taskB) { }
                }),
                Task.Run(async () => {
                    var taskC = Execute(externalC, nameof(externalC), searchResults, cancellationToken);
                    var taskD = taskC.ContinueWith(async result => {
                        if (await result == Result.OK)
                        {
                            await Execute(externalD, nameof(externalD), searchResults, cancellationToken);
                        }
                    });
                    if (await Task.WhenAny(taskD, Task.Delay(_maxSearchDuration)) == taskD) { }
                })
            );

            stopwatch.Stop();

            _logger.LogInformation($"Time spent: {stopwatch.ElapsedMilliseconds}");

            return Ok(searchResults);
        }


        /// <summary>
        /// Get metrics
        /// </summary>
        /// <response code="200">Successfully</response>
        [HttpGet("/metrics")]
        [ProducesResponseType(typeof(IEnumerable<MetricsView>), 200)]
        public async Task<IActionResult> Metrics(
            CancellationToken cancellationToken,
            [FromServices] IMetricsRepository metricsRepository)
        {
            var ts = TimeSpan.FromSeconds(1);

            var metrics = await metricsRepository.GetAll(cancellationToken);

            var metricsGroupped = metrics
                .GroupBy(o => new { o.TimeRequest.TotalSeconds, o.Name })
                .Select(o => new MetricsView
                {
                    Name = o.Key.Name,
                    Count = o.Count(),
                    Metrics = o.Select(o => new MetricView
                    {
                        Result = o.Result,
                        Id = o.Id,
                        DurationInSeconds = o.TimeRequest.TotalSeconds
                    }),
                    DurationInSeconds = o.Key.TotalSeconds
                });


            return Ok(metricsGroupped);
        }

        private static async Task<Result> Execute(IExternal externalService, string serviceName,
            ConcurrentBag<SearchResultView> searchResults, CancellationToken cancellationToken)
        {
            var result = await externalService.Request(cancellationToken);

            searchResults.Add(new SearchResultView
            {
                ServiceName = serviceName,
                Result = result
            });

            return result;
        }
    }
}
