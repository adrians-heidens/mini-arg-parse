using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class SingleValueArgument : ArgumentBase
    {
        public override void Parse(ArgumentList argumentList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');

            var value = argumentList.PeekNext();
            
            if (value == null || value.StartsWith("-"))
            {
                throw new ArgumentParseException($"Argument {Name}: expected one argument");
            }

            argumentList.DropOne();
            argumentList.DropOne();

            parsedArgs[key] = value;
        }
    }
}