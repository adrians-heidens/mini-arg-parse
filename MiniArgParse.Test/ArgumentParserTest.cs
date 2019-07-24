using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniArgParse;
using System.Linq;
using System.IO;

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

        [TestMethod]
        public void SubParsers()
        {
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", action: "toggle", help: "foo help");

            var subparsers = parser.AddSubparsers(help: "sub-command help");

            var parserA = subparsers.AddParser("a", help: "a help");
            parserA.AddArgument("bar", "single", help: "bar help");

            var parserB = subparsers.AddParser("b", help: "b help");
            parserB.AddArgument("--baz", "single", help: "baz help");

            var args = parser.ParseArgs(new string[] {"--foo", "b", "--baz", "1"});
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual(true, args["foo"]);
            Assert.AreEqual("1", args["baz"]);

            args = parser.ParseArgs(new string[] {"--foo", "a", "1"});
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual(true, args["foo"]);
            Assert.AreEqual("1", args["bar"]);
        }

        [TestMethod]
        public void Defaults()
        {
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", action: "single", help: "foo help");
            parser.SetDefault("foo", "12");
            parser.SetDefault("bar", "spam");

            var args = parser.ParseArgs(new string[] {});

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("12", args["foo"]);
            Assert.AreEqual("spam", args["bar"]);
        }

        [TestMethod]
        public void OverrideDefaults()
        {
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", action: "single", help: "foo help");
            parser.SetDefault("foo", "12");
            parser.SetDefault("bar", "spam");

            var args = parser.ParseArgs(new string[] {"--foo", "a"});

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("a", args["foo"]);
            Assert.AreEqual("spam", args["bar"]);
        }

        [TestMethod]
        public void SubParserDefaults()
        {
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", action: "toggle", help: "foo help");

            var subparsers = parser.AddSubparsers(help: "sub-command help");

            var parserA = subparsers.AddParser("a", help: "a help");
            parserA.AddArgument("bar", "single", help: "bar help");
            parserA.SetDefault("func", "FuncA()");

            var parserB = subparsers.AddParser("b", help: "b help");
            parserB.AddArgument("--baz", "single", help: "baz help");
            parserB.SetDefault("func", "FuncB()");

            var args = parser.ParseArgs(new string[] {"--foo", "b", "--baz", "1"});
            Assert.AreEqual("FuncB()", args["func"]);

            args = parser.ParseArgs(new string[] {"--foo", "a", "1"});
            Assert.AreEqual("FuncA()", args["func"]);
        }

        [TestMethod]
        public void HelpTextMinimal()
        {
            var parser = new ArgumentParser();
            parser.ProgName = "prog";
            Assert.AreEqual("Usage: prog [OPTION]... [ARG]...", parser.HelpText);
        }

        [TestMethod]
        public void HelpTextWithArguments()
        {
            var parser = new ArgumentParser();
            parser.AddArgument("--foo", help: "foo help");
            parser.AddArgument("--bar", action: "toggle", help: "bar help");
            parser.AddArgument("spam", action: "single", help: "spam help");
            parser.ProgName = "prog";
            parser.Description = "This is prog description.";

            var expected = @"Usage: prog [OPTION]... [ARG]...

This is prog description.

Positional arguments:
  spam            spam help

Optional arguments:
  --foo FOO       foo help
  --bar           bar help".Replace("\r", string.Empty);

            Assert.AreEqual(expected, parser.HelpText);
        }
    }
}
