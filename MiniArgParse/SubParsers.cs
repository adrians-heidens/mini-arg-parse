namespace MiniArgParse
{
    public class SubParsers
    {
        private ArgumentParser _parentParser;

        public SubParsers(ArgumentParser parentParser)
        {
            _parentParser = parentParser;
        }

        public ArgumentParser AddParser(string name, string help = "")
        {
            var parser = new ArgumentParser();
            var commandName = name;
            _parentParser.AddSubparser(commandName, parser);
            return parser;
        }
    }
}