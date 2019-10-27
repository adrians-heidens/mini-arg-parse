using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    class ListArgument : ArgumentBase
    {
        public override void Parse(ArgumentList argumentList, Dictionary<string, dynamic> parsedArgs)
        {
            string key = Name.TrimStart('-');
            var value = new List<string>();
            parsedArgs[key] = value;
            argumentList.DropOne();
            while (argumentList.Any())
            {
                var v = argumentList.Peek();
                if (v.StartsWith("-"))
                {
                    break;
                }
                value.Add(v);
                argumentList.DropOne();
            }
        }
    }
}