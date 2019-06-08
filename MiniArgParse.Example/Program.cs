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
            parser.AddArgument(name: "--foo"); // Single string value.
            parser.AddArgument(name: "--bar", action: "toggle"); // Boolean.
            parser.AddArgument(name: "spam"); // Positional string value 1.
            parser.AddArgument(name: "eggs"); // Position string value 2.

            IDictionary<string, dynamic> parsedArgs = null;
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
