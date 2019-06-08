using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniArgParse;

namespace MiniArgParse.Test
{
    [TestClass]
    public class ArgumentParserTest
    {
        [TestMethod]
        public void OptionWithEmptyArgs()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");

            var args = parser.ParseArgs(new string[] {});

            Assert.AreEqual(1, args.Count);
            Assert.IsTrue(args.ContainsKey("foo"));
            Assert.IsNull(args["foo"]);
        }

        [TestMethod]
        public void OptionWithValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");

            var args = parser.ParseArgs(new string[] {"--foo", "1"});
            Assert.AreEqual(1, args.Count);
            Assert.IsTrue(args.ContainsKey("foo"));
            Assert.AreEqual("1", args["foo"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void OptionWithNonValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");
            var args = parser.ParseArgs(new string[] {"--foo", "--bar"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void OptionWithMissingValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");
            var args = parser.ParseArgs(new string[] {"--foo"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void UnknownOption()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");
            var args = parser.ParseArgs(new string[] {"--bar", "2"});
        }

        [TestMethod]
        public void OptionWithPositional()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");
            parser.AddArgument(name: "bar");

            var args = parser.ParseArgs(new string[] {"--foo", "1", "2"});

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("1", args["foo"]);
            Assert.AreEqual("2", args["bar"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void PositionalNoneValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "bar");

            var args = parser.ParseArgs(new string[] {"--foo"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void PositionalMissing()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "bar");

            var args = parser.ParseArgs(new string[] {});
        }
    }
}
