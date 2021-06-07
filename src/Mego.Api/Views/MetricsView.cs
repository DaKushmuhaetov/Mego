using Mego.Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mego.Api.Views
{
    public class MetricsView
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double DurationInSeconds { get; set; }
        public IEnumerable<MetricView> Metrics { get; set; }
    }
}
