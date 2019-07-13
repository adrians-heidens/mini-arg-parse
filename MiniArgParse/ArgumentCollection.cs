using System.Collections.Generic;

namespace MiniArgParse
{
    class ArgumentCollection
    {
        private List<Argument> _arguments = new List<Argument>();

        public List<Argument> OptionArgs => _arguments.FindAll(x => x.Name.StartsWith("-"));

        public List<Argument> PositionArgs => _arguments.FindAll(x => !x.Name.StartsWith("-"));

        public void Add(Argument argument)
        {
            _arguments.Add(argument);
        }
    }
}