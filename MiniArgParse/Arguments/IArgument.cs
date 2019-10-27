using System.Collections.Generic;

namespace MiniArgParse.Arguments
{
    interface IArgument
    {
        string Name { get; set; }

        string Action { get; set; }

        string Help { get; set; }

        bool IsPositional { get; }

        void Parse(ArgumentList argumentList, Dictionary<string, dynamic> parsedArgs);
    }
}