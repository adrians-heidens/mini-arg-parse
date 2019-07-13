using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class ListArgument : ArgumentBase
    {
        public override void Parse(int argIndex, List<string> argsList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');
            var value = new List<string>();
            parsedArgs[key] = value;
            argsList.RemoveAt(argIndex);
            while (argIndex < argsList.Count)
            {
                if (argsList[argIndex].StartsWith("-"))
                {
                    break;
                }
                value.Add(argsList[argIndex]);
                argsList.RemoveAt(argIndex);
            }
        }
    }
}