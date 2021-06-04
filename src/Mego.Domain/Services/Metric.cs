using System;

namespace Mego.Domain.Services
{
    public sealed class Metric
    {
        public Guid Id { get; }
        public string Name { get; }
        public Result Result { get; }
        public TimeSpan TimeRequest { get; }

        public Metric(Guid id, string name, Result result, TimeSpan timeRequest)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Result = result;
            TimeRequest = timeRequest;
        }
    }
}
