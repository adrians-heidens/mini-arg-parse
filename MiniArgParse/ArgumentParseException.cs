using System;

namespace MiniArgParse
{
    public class ArgumentParseException : Exception
    {
        public ArgumentParseException(string message) : base(message)
        {
        }
    }
}