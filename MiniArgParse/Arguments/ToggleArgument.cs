using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class ToggleArgument : ArgumentBase
    {
        public override void Parse(int argIndex, List<string> argsList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');
            argsList.RemoveAt(argIndex);
            parsedArgs[key] = true;
        }
    }
}