using System;
using System.Collections.Generic;
using System.Linq;
using MiniArgParse;

namespace MiniArgParse.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ArgumentParser
            {
                Description = "Example argument parsing program."
            };

            // Single string value.
            parser.AddArgument(
                name: "--foo",
                help: "Foo help.");
            
            // Long name parameter.
            parser.AddArgument(
                name: "--long-name-param",
                help: "Long named parameter.");

            // Boolean.
            parser.AddArgument(
                name: "--bar",
                action: "toggle",
                help: "Help on bar."); 

            // Positional string value 1.
            parser.AddArgument(name: "spam", help: "The spam argument.");

            // Position string value 2.
            parser.AddArgument(name: "eggs", help: "Some help on eggs."); 

            if (args.Contains("--help"))
            {
                Console.WriteLine(parser.HelpText);
                Environment.Exit(0);
            }

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
