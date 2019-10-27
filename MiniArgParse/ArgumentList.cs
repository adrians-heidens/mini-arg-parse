using System.Collections.Generic;
using System.Linq;

namespace MiniArgParse
{
    /// <summary>
    /// Keep track of arguments during parse.
    /// </summary>
    public class ArgumentList
    {
        private readonly List<string> _args;
        
        public ArgumentList(string[] args)
        {
            _args = args.ToList();
        }

        public bool Any()
        {
            return _args.Any();
        }

        public string Peek()
        {
            return _args[0];
        }

        public string PeekNext()
        {
            if (_args.Count < 2)
            {
                return null;
            }
            return _args[1];
        }
        
        public void DropOne()
        {
            _args.RemoveAt(0);
        }

        public string[] ToArray()
        {
            return _args.ToArray();
        }
    }
}