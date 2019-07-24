using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniArgParse.Arguments;

namespace MiniArgParse
{
    public class ArgumentParser
    {
        public string Description { get; set; }

        private ArgumentCollection _arguments = new ArgumentCollection();

        private Dictionary<string, ArgumentParser> _subParsers =
            new Dictionary<string, ArgumentParser>();

        private Dictionary<string, dynamic> _defaults =
            new Dictionary<string, dynamic>();

        public string ProgName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

        public IDictionary<string, dynamic> ParseArgs(string[] args)
        {
            var optionArgs = _arguments.OptionArgs;
            var positionArgs = _arguments.PositionArgs;

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
            foreach (var entry in _defaults)
            {
                parsedArgs[entry.Key] = entry.Value;
            }

            var argsList = new List<string>(args);

            // Got through args in sequence and look for arguments,
            // remove processed args.
            var argIndex = 0;
            while (argIndex < argsList.Count)
            {
                var argName = argsList[argIndex];
                var argument = optionArgs.Find(x => x.Name == argName);

                if (argument != null)
                {
                    argument.Parse(argIndex, argsList, parsedArgs);
                    continue;
                }

                // This should be position arg.

                if (argName.StartsWith("-")) // Unrecognized arg.
                {
                    argIndex += 1;
                    continue;
                }

                if (positionArgs.Count > 0) // Positional arg.
                {
                    argument = positionArgs[0];

                    if (argument.Name == "command")
                    {
                        var commandName = argName;
                        var subParser = _subParsers[commandName];

                        argsList.RemoveAt(argIndex);
                        positionArgs.RemoveAt(argIndex);

                        var o = subParser.ParseArgs(argsList.ToArray());

                        foreach (var entry in o)
                        {
                            parsedArgs[entry.Key] = entry.Value;
                        }
                        return parsedArgs;
                    }

                    parsedArgs[argument.Name] = argName;

                    argsList.RemoveAt(argIndex);
                    positionArgs.RemoveAt(argIndex);
                }
                else
                {
                    argIndex += 1;
                }                
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

        public void SetDefault(string name, dynamic value)
        {
            _defaults[name] = value;
        }

        public void AddSubparser(string name, ArgumentParser subParser)
        {
            _subParsers.Add(name, subParser);
        }

        public void AddArgument(string name, string action = "single", string help = "")
        {
            if (action == null || action == "single")
            {
                _arguments.Add(new SingleValueArgument {Name = name, Action = action, Help = help});
            }
            else if (action == "list")
            {
                _arguments.Add(new ListArgument {Name = name, Action = action, Help = help});
            }
            else if (action == "toggle")
            {
                _arguments.Add(new ToggleArgument {Name = name, Action = action, Help = help});
            }
            else
            {
                throw new Exception($"Unexpected action: '{action}'");
            }
        }

        private void AddArgumentHelp(StringBuilder builder, IArgument argument)
        {
            const int indent = 15;
            string argSpec = null;
            if (argument.Action == "toggle" || argument.IsPositional)
            {
                argSpec = argument.Name;
            }
            else if (argument.Action == "list")
            {
                var key = argument.Name.TrimStart('-').ToUpper();
                argSpec = $"{argument.Name} [{key} [{key} ...]]";
            }
            else
            {
                var key = argument.Name.TrimStart('-').ToUpper();
                argSpec = $"{argument.Name} {key}";
            }

            if (argSpec.Length >= indent)
            {
                builder.AppendLine($"  {argSpec,-indent}".TrimEnd());
                builder.AppendLine($"  {new string(' ', indent)} {argument.Help}".TrimEnd());
            }
            else
            {
                builder.AppendLine($"  {argSpec,-indent} {argument.Help}".TrimEnd());
            }
        }

        public SubParsers AddSubparsers(string help)
        {
            AddArgument("command", help: help);
            return new SubParsers(this);
        }

        public string HelpText {
            get {
                var builder = new StringBuilder();
                builder.AppendLine($"Usage: {ProgName} [OPTION]... [ARG]...");

                if (Description != null)
                {
                    builder.AppendLine($"\n{Description}");
                }

                var optionArgs = _arguments.OptionArgs;
                var positionArgs = _arguments.PositionArgs;

                if (positionArgs.Count > 0)
                {
                    builder.AppendLine("\nPositional arguments:");
                    foreach (var arg in positionArgs)
                    {
                        AddArgumentHelp(builder, arg);
                    }
                }
                
                if (optionArgs.Count > 0)
                {
                    builder.AppendLine("\nOptional arguments:");
                    foreach (var arg in optionArgs)
                    {
                        AddArgumentHelp(builder, arg);
                    }
                }

                return builder.ToString().TrimEnd('\n');
            }
        }
    }
}
