using Mego.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mego.Api.Views
{
    public class MetricView
    {
        public Guid Id { get; set; }
        public Result Result { get; set; }
        public double DurationInSeconds { get; set; }
    }
}
