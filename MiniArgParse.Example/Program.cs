using System;
using System.Collections.Generic;
using MiniArgParse;

namespace MiniArgParse.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");
            parser.AddArgument(name: "--bar");
            parser.AddArgument(name: "spam");
            parser.AddArgument(name: "eggs");

            IDictionary<string, string> parsedArgs = null;
            try
            {
                parsedArgs = parser.ParseArgs(args);
            }
            catch (ArgumentParseException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Environment.Exit(2);
            }
            
            foreach (var entry in parsedArgs)
            {
                Console.WriteLine($"> '{entry.Key}': '{entry.Value}'");
            }
        }
    }
}
