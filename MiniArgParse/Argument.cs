namespace MiniArgParse
{
    class Argument
    {
        public string Name { get; set; }

        public string Action { get; set; }

        public string Help { get; set; }

        public bool IsPositional
        {
            get
            {
                return !Name.StartsWith("-");
            }
        }
    }
}