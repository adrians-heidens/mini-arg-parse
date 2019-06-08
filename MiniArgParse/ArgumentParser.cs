﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniArgParse
{
    public class ArgumentParser
    {
        class Argument {
            public string Name { get; set; }

            public string Action { get; set; }
        }

        private List<Argument> _arguments = new List<Argument>();

        public IDictionary<string, dynamic> ParseArgs(string[] args)
        {
            var optionArgs = _arguments.FindAll(x => x.Name.StartsWith("-"));
            var positionArgs = _arguments.FindAll(x => !x.Name.StartsWith("-"));

            // Create parsed args dict and set default values.
            var parsedArgs = new Dictionary<string, dynamic>();
            foreach (var arg in optionArgs)
            {
                if (arg.Action == "toggle")
                {
                    parsedArgs[arg.Name.TrimStart('-')] = false;
                }
                else {
                    parsedArgs[arg.Name.TrimStart('-')] = null;
                }
            }

            var argsList = new List<string>(args);
            
            // Got through args in sequence and look for optional ones,
            // remove processed args.
            var argIndex = 0;
            while (argIndex < argsList.Count)
            {
                var argName = argsList[argIndex];
                var argument = optionArgs.Find(x => x.Name == argName);

                if (argument == null)
                {
                    argIndex += 1;
                    continue;
                }
                
                if (argument.Action == "toggle")
                {
                    string key = argument.Name.TrimStart('-');
                    argsList.RemoveAt(argIndex);
                    parsedArgs[key] = true;
                }

                else if (argument.Action == null || argument.Action == "single")
                {
                    string key = argument.Name.TrimStart('-');

                    var valueIndex = argIndex + 1;
                    var value = valueIndex >= argsList.Count ? null : argsList[valueIndex];
                    
                    if (value == null || value.StartsWith("-"))
                    {
                        throw new ArgumentParseException($"Argument {argument}: expected one argument");
                    }

                    argsList.RemoveAt(argIndex);
                    argsList.RemoveAt(argIndex);
                    
                    parsedArgs[key] = value;
                }

                else
                {
                    throw new Exception($"Unexpected argument action: {argument.Action}");
                }                
            }

            // Go through positional args and set values.
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
                parsedArgs[argument.Name] = value;
                
                argsList.RemoveAt(argIndex);
                positionArgs.RemoveAt(argIndex);
            }

            // Report if positional missing.
            if (positionArgs.Count > 0)
            {
                var a = string.Join(", ", positionArgs.Select(x => x.Name));
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

        public void AddArgument(string name, string action = "single")
        {
            _arguments.Add(new Argument {Name = name, Action = action});
        }

        public string HelpText {
            get {
                var progName = AppDomain.CurrentDomain.FriendlyName;
                var builder = new StringBuilder();
                builder.AppendLine($"Usage: {progName} [OPTION]... [ARG]...");

                var optionArgs = _arguments.FindAll(x => x.Name.StartsWith("-"));
                var positionArgs = _arguments.FindAll(x => !x.Name.StartsWith("-"));

                if (positionArgs.Count > 0)
                {
                    builder.AppendLine("\nPositional arguments:");
                    foreach (var arg in positionArgs)
                    {
                        builder.AppendLine($"  {arg.Name}");
                    }
                }
                
                if (optionArgs.Count > 0)
                {
                    builder.AppendLine("\nOptional arguments:");
                    foreach (var arg in optionArgs)
                    {
                        if (arg.Action == "toggle")
                        {
                            builder.AppendLine($"  {arg.Name}");
                        }
                        else
                        {
                            var key = arg.Name.TrimStart('-').ToUpper();
                            builder.AppendLine($"  {arg.Name} {key}");
                        }
                    }
                }

                return builder.ToString().TrimEnd('\n');
            }
        }
    }
}
