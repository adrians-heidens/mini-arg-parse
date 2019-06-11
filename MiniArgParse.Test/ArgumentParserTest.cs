using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniArgParse;
using System.Linq;

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

        [TestMethod]
        public void ToggleOptionEnabled()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "toggle");

            var args = parser.ParseArgs(new string[] {"--bar"});
            Assert.AreEqual(1, args.Count);
            Assert.IsTrue(args.ContainsKey("bar"));
            Assert.AreEqual(true, args["bar"]);
        }

        [TestMethod]
        public void ToggleOptionDisabled()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "toggle");

            var args = parser.ParseArgs(new string[] {});
            Assert.AreEqual(1, args.Count);
            Assert.IsTrue(args.ContainsKey("bar"));
            Assert.AreEqual(false, args["bar"]);
        }

        [TestMethod]
        public void OptionRepeatOverrides()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "single");

            var args = parser.ParseArgs(new string[] {"--bar", "1", "--bar", "2"});
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("2", args["bar"]);
        }

        [TestMethod]
        public void ToggleOptionRepeatDoesNothing()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "toggle");

            var args = parser.ParseArgs(new string[] {"--bar", "--bar"});
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(true, args["bar"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentParseException))]
        public void ToggleOptionWithValueIsError()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "toggle");
            var args = parser.ParseArgs(new string[] {"--bar", "1"});
        }

        [TestMethod]
        public void OptionalAfterPositional()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "foo");
            parser.AddArgument(name: "--bar");
            var args = parser.ParseArgs(new string[] {"1", "--bar", "2"});
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("1", args["foo"]);
            Assert.AreEqual("2", args["bar"]);
        }

        [TestMethod]
        public void ListOption()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo", action: "list");
            parser.AddArgument(name: "--bar", action: "single");
            var args = parser.ParseArgs(new string[] {"--foo", "1", "2", "--bar", "3"});
            Assert.AreEqual(2, args.Count);
            Assert.IsTrue(args["foo"] is IList<string>);

            var l = args["foo"] as IList<string>;
            Assert.IsTrue(l.SequenceEqual(new List<string> {"1", "2"}));
        }
    }
}
