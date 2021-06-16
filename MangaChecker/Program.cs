using System;
using System.Threading.Tasks;
using Discord;

namespace MangaChecker
{
    class Program
    {
        // Async context creation
        static void Main(string[] args) => new BotMainContext().StartContextAsync().GetAwaiter().GetResult();
    }
}