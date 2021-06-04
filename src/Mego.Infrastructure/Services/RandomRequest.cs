using Mego.Domain.Services;
using System;

namespace Mego.Domain.Infrastructure.Services
{
    public class RandomRequest
    {
        public static TimeSpan RandomTimeRequest()
        {
            var random = new Random();
            var result = random.Next(1, 11);

            return TimeSpan.FromSeconds(result);
        }

        public static Result RandomResult()
        {
            var random = new Random();
            var result = random.Next(0, 2);

            return (Result)result;
        }
    }
}
