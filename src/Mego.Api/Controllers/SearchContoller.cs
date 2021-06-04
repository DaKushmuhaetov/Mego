using Mego.Api.Views;
using Mego.Domain.Infrastructure.Services;
using Mego.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        [ProducesResponseType(typeof(IEnumerable<SearchResultView>), 200)]
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

            List<SearchResultView> searchResults = new List<SearchResultView>();

            Task.WaitAll(
                // External service A
                Task.Run(() => {
                    var searchInExternalA = Task.Run(() => externalA.Request(cancellationToken));
                    if (searchInExternalA.Wait(_maxSearchDuration))
                    {
                        var result = searchInExternalA.Result;

                        searchResults.Add(new SearchResultView
                        {
                            ServiceName = "ExternalA",
                            Result = result
                        });
                    }
                }),
                // External service B
                Task.Run(() => {
                    var searchInExternalB = Task.Run(() => externalB.Request(cancellationToken));
                    if (searchInExternalB.Wait(_maxSearchDuration))
                    {
                        var result = searchInExternalB.Result;

                        searchResults.Add(new SearchResultView
                        {
                            ServiceName = "ExternalB",
                            Result = result
                        });
                    }
                }),
                // External service C
                Task.Run(() => {
                    var task = Task.Run(async () =>
                    {
                        var externalCReuslt = await externalC.Request(cancellationToken);
                        searchResults.Add(new SearchResultView
                        {
                            ServiceName = "ExternalC",
                            Result = externalCReuslt
                        });

                        var externalDReuslt = await externalD.Request(cancellationToken);
                        searchResults.Add(new SearchResultView
                        {
                            ServiceName = "ExternalD",
                            Result = externalDReuslt
                        });
                    });

                    task.Wait(_maxSearchDuration);
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
                .GroupBy(o => new { ts, o.Name })
                .Select(o => new MetricsView
                {
                    Name = o.Key.Name,
                    Count = o.Count()
                });
                

            return Ok(metricsGroupped);
        }
    }
}
