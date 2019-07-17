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
        public void OptionWithNonValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");

            try
            {
                var args = parser.ParseArgs(new string[] {"--foo", "--bar"});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("Argument --foo: expected one argument", e.Message);
            }
        }

        [TestMethod]
        public void OptionWithMissingValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");

            try
            {
                var args = parser.ParseArgs(new string[] {"--foo"});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("Argument --foo: expected one argument", e.Message);
            }
        }

        [TestMethod]
        public void UnknownOption()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--foo");

            try
            {
                var args = parser.ParseArgs(new string[] {"--bar", "2"});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("Unrecognized arguments: --bar 2", e.Message);
            }
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
        public void PositionalNoneValue()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "bar");

            try
            {
                var args = parser.ParseArgs(new string[] {"--foo"});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("the following arguments are required: bar", e.Message);
            }
        }

        [TestMethod]
        public void PositionalMissing()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "bar");

            try
            {
                var args = parser.ParseArgs(new string[] {});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("the following arguments are required: bar", e.Message);
            }
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
        public void ToggleOptionWithValueIsError()
        {
            var parser = new ArgumentParser();
            parser.AddArgument(name: "--bar", action: "toggle");
            
            try 
            {
                var args = parser.ParseArgs(new string[] {"--bar", "1"});
                Assert.Fail("ArgumentParseException expected");
            }
            catch (ArgumentParseException e)
            {
                Assert.AreEqual("Unrecognized arguments: 1", e.Message);
            }
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
