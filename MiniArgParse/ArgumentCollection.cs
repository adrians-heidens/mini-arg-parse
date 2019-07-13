using System.Collections.Generic;
using MiniArgParse.Arguments;

namespace MiniArgParse
{
    class ArgumentCollection
    {
        private List<IArgument> _arguments = new List<IArgument>();

        public List<IArgument> OptionArgs => _arguments.FindAll(x => x.Name.StartsWith("-"));

        public List<IArgument> PositionArgs => _arguments.FindAll(x => !x.Name.StartsWith("-"));

        public void Add(IArgument argument)
        {
            _arguments.Add(argument);
        }
    }
}