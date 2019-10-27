using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class ToggleArgument : ArgumentBase
    {
        public override void Parse(ArgumentList argumentList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');
            argumentList.DropOne();
            parsedArgs[key] = true;
        }
    }
}