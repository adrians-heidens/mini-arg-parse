using System;
using System.Collections.Generic;

namespace MiniArgParse
{
    public class ArgumentParser
    {
        private List<string> _arguments = new List<string>();

        public IDictionary<string, string> ParseArgs(string[] args)
        {
            Console.WriteLine($"Parsing args: '{string.Join(", ", args)}'.");

            var parsedArgs = new Dictionary<string, string>();

            var optionArgs = _arguments.FindAll(x => x.StartsWith("-"));
            var positionArgs = _arguments.FindAll(x => !x.StartsWith("-"));

            var argsList = new List<string>(args);
            
            // Go through options args, remove parsed params and args.
            var argIndex = 0;
            while (argIndex < optionArgs.Count)
            {
                var argument = optionArgs[argIndex];
                
                parsedArgs[argument.TrimStart('-')] = null;

                var i = argsList.IndexOf(argument);
                if (i == -1)
                {
                    argIndex += 1;
                    continue;
                }

                optionArgs.RemoveAt(argIndex);

                if (argsList.Count < i + 2)
                {
                    throw new ArgumentParseException($"Argument {argument}: expected one argument");
                }

                string key = argument.TrimStart('-');
                string value = argsList[i + 1];

                if (value.StartsWith("-"))
                {
                    throw new ArgumentParseException($"Argument {argument}: expected one argument");
                }

                argsList.RemoveAt(i);
                argsList.RemoveAt(i);
                
                parsedArgs[key] = value;
            }

            // Go through positional args.
            argIndex = 0;
            while (positionArgs.Count > argIndex && argsList.Count > argIndex)
            {
                var value = argsList[argIndex];
                if (value.StartsWith("-"))
                {
                    argIndex += 1;
                    continue;
                }

                var argument = positionArgs[argIndex];
                parsedArgs[argument] = value;
                
                argsList.RemoveAt(argIndex);
                positionArgs.RemoveAt(argIndex);
            }

            // Report if positional missing.
            if (positionArgs.Count > 0)
            {
                var a = string.Join(", ", positionArgs);
                var m = $"the following arguments are required: {a}";
                throw new ArgumentParseException(m);
            }

            // Report error if left over args.
            if (argsList.Count > 0)
            {
                var m = $"Unrecognized arguments: {string.Join(" ", argsList)}";
                throw new ArgumentParseException(m);
            }

            return parsedArgs;
        }

        public void AddArgument(string name)
        {
            Console.WriteLine($"Adding argument '{name}'.");
            _arguments.Add(name);
        }
    }
}
