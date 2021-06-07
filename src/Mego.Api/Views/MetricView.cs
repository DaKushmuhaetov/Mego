using Mego.Domain.Services;
using System;

namespace Mego.Api.Views
{
    public class MetricView
    {
        public Guid Id { get; set; }
        public Result Result { get; set; }
        public double DurationInSeconds { get; set; }
    }
}
