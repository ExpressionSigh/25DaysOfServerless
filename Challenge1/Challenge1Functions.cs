using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Challenge1
{
    public static class Challenge1Functions
    {
        private static readonly Random RandomGenerator = new Random();

        private static readonly HebrewLetter[] HebrewLetters =
        {
            new HebrewLetter
            {
                Name = "Nun",
                Symbol = (char) 1504
            },
            new HebrewLetter
            {
                Name = "Gimmel",
                Symbol = (char) 1490
            },
            new HebrewLetter
            {
                Name = "Hay",
                Symbol = (char) 1492
            },
            new HebrewLetter
            {
                Name = "Shin",
                Symbol = (char) 1513
            }
        };

        [FunctionName("SpinDreidel")]
        public static IActionResult SpinDreidel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dreidel/spinned")] HttpRequest req)
        {
            return new OkObjectResult(HebrewLetters[RandomGenerator.Next(HebrewLetters.Length)]);
        }
    }
}