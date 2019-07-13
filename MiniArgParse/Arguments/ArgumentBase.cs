using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    abstract class ArgumentBase : IArgument
    {
        public string Name { get; set; }

        public string Action { get; set; }

        public string Help { get; set; }

        public bool IsPositional => !Name.StartsWith("-");

        public abstract void Parse(int argIndex, List<string> argsList, Dictionary<string, dynamic> parsedArgs);
    }
}