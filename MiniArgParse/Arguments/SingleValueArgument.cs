using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class SingleValueArgument : ArgumentBase
    {
        public override void Parse(int argIndex, List<string> argsList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');

            var valueIndex = argIndex + 1;
            var value = valueIndex >= argsList.Count ? null : argsList[valueIndex];
            
            if (value == null || value.StartsWith("-"))
            {
                throw new ArgumentParseException($"Argument {Name}: expected one argument");
            }

            argsList.RemoveAt(argIndex);
            argsList.RemoveAt(argIndex);
            
            parsedArgs[key] = value;
        }
    }
}