using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniArgParse.Example
{
    static class SubCommandExample
    {
        public static void Run(string[] args)
        {
            /*
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", action: "toggle", help: "foo help");

            var subparsers = parser.AddSubparsers(help: "sub-command help");

            var parserA = subparsers.AddParser("a", help: "a help");
            parserA.AddArgument("bar", "single", help: "bar help");

            var parserB = subparsers.AddParser("b", help: "b help");
            parserB.AddArgument("--baz", "single", help: "baz help");

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
                if (entry.Value is IList<string>)
                {
                    var s = string.Join(", ", entry.Value as IList<string>);
                    Console.WriteLine($"> '{entry.Key}': '{s}'");
                }
                else
                {
                    Console.WriteLine($"> '{entry.Key}': '{entry.Value}'");
                }
            }
            */
        }
    }
}