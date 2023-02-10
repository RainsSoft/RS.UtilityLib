using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.GetOpt.CmdLine
{
    /// <summary>
    /// Decodes command line options.  Based on GNU getopt
    /// </summary>

    public enum Argument : int { None = 0, Optional, Required }

    public class CommandLineParser
    {
        private int optind;
        private int nextOpt;
        private string optarg;
        private string curArg;
        private string[] arguments;
        private string optionList;
        private char optopt;
        private bool skipRest;
        private Dictionary<char, CommandLineOption> cmdOptions;

        /// <summary>
        /// Current command line argument being processed.
        /// </summary>
        public int OptionIndex {
            get { return optind; }
            set { optind = value; }
        }

        /// <summary>
        /// Argument for the current command line option.
        /// </summary>
        public string OptionArgument {
            get { return optarg; }
            set { optarg = value; }
        }

        /// <summary>
        /// Character for the current command line option.
        /// </summary>
        public char OptionCharacter {
            get { return optopt; }
            set { optopt = value; }
        }

        private CommandLineParser() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="optList">A string consisting of the valid single-letter command line options.
        /// Each letter is followed by a : if the option has a mandatory argument, or
        /// :: if the argument is optional.</param>
        /// <param name="args">The array of command line arguments.  This can be the args[] parameter from Main().</param>
        public CommandLineParser(string options, string[] args) {
            arguments = args;
            optionList = options ?? "";
            Reset();
            int pos = 0;
            while (pos < optionList.Length) {
                if (!char.IsLetterOrDigit(optionList[pos])) {
                    throw new ArgumentException("Option must be a letter or a digit.");
                }
                CommandLineOption opt = new CommandLineOption(optionList[pos++]);
                if (pos < optionList.Length && optionList[pos] == ':') {
                    opt.Argument = Argument.Required;
                    pos++;
                    if (pos < optionList.Length && optionList[pos] == ':') {
                        opt.Argument = Argument.Optional;
                        pos++;
                    }
                }
                cmdOptions.Add(opt.Option, opt);
            }
        }

        public CommandLineParser(string[] args, params CommandLineOption[] options) {
            arguments = args;
            optionList = "";
            foreach (CommandLineOption option in options) {
                if (optionList.IndexOf(option.Option) < 0) {
                    optionList += option.Option;
                    switch (option.Argument) {
                        case Argument.Required:
                            optionList += ":";
                            break;
                        case Argument.Optional:
                            optionList += "::";
                            break;
                    }
                    cmdOptions.Add(option.Option, option);
                }
            }
            Reset();
        }

        /// <summary>
        /// Restart processing
        /// </summary>
        public void Reset() {
            optind = -1;
            curArg = null;
            nextOpt = 0;
            optarg = null;
            optopt = '\0';
            skipRest = false;
            cmdOptions = new Dictionary<char, CommandLineOption>();
        }

        public IEnumerable<OptionResult> Options {
            get {
                char option;
                OptionResult result = new OptionResult();

                while (this.OptionIndex < arguments.Length) {
                    while ((option = this.NextOption()) != '\0') {
                        if (option == '?') {
                            // invalid option, or option missing required parameter
                            result.IsValid = false;
                            result.Value = this.OptionCharacter;
                            yield return result;
                        }
                        else {
                            // valid option
                            result.IsValid = true;
                            result.Value = this.OptionCharacter;
                            result.Argument = (this.OptionArgument != null ? this.OptionArgument : "");
                            yield return result;
                        }
                    }
                    if (this.OptionIndex < arguments.Length) {
                        result.IsValid = true;
                        result.Value = ' ';
                        result.Argument = arguments[this.OptionIndex];
                        this.OptionIndex++;
                        yield return result;
                    }
                }

            }
        }


        /// <summary>
        /// Pulls the next option from the command line.
        /// </summary>
        /// <remarks>Sets the OptionCharacter property to the character found.  
        /// This is set even if the option is not valid, or if the option is missing a required argument.
        /// Sets the OptionArgument property to the argument, if present.</remarks>
        /// <returns>The option character, or 0 if no more options.
        /// Returns '?' if the option is invalid or is missing a required argument.</returns>
        public char NextOption() {
            optarg = null;
            int optPos;

            if (skipRest) {
                if (optind < arguments.Length) {
                    optarg = arguments[optind];
                }
                return '\0';
            }

            if (optind == -1) {
                nextOpt = 0;
                curArg = null;
                optind++;
            }

            optopt = '\0';

            if (curArg == null || nextOpt >= curArg.Length) {
                if (optind >= arguments.Length) {
                    return '\0';
                }

                if (arguments[optind].Length == 1 || arguments[optind][0] != '-') {
                    optarg = arguments[optind];
                    return '\0';
                }

                if (arguments[optind] == "--") {
                    skipRest = true;
                    optind++;
                    if (optind < arguments.Length) {
                        optarg = arguments[optind];
                    }
                    return '\0';
                }

                nextOpt = 1;
                curArg = arguments[optind];
                optind++;
            }

            optopt = curArg[nextOpt++];
            optPos = optionList.IndexOf(optopt);

            if (optPos < 0 || optopt == ':') {
                return '?';
            }

            optPos++;
            if (optPos < optionList.Length) {
                if (optionList[optPos] == ':') {
                    // this option takes a parameter, and there
                    // is stuff left in the current argument.
                    // set optarg to the remainder, and clear current argument.
                    if (nextOpt < curArg.Length) {
                        optarg = curArg.Substring(nextOpt);
                        nextOpt = 0;
                        curArg = null;
                    }
                    // if the next token doesn't start with -, set optarg to it.
                    else if (optind < arguments.Length && arguments[optind][0] != '-') {
                        optarg = arguments[optind];
                        optind++;
                    }
                    else if (optPos + 1 >= optionList.Length || optionList[optPos + 1] != ':') {
                        // parameter for this option is required.
                        return '?';
                    }
                }
            }
            return optopt;
        }

        public static string[] SplitLine(string inputLine) {
            bool inQuote = false;
            StringBuilder curField = new StringBuilder("");
            List<string> result = new List<string>(20);
            int pos = 0;

            if (String.IsNullOrEmpty(inputLine)) {
                return new string[0];
            }

            while (pos < inputLine.Length) {
                switch (inputLine[pos]) {
                    case ' ':
                        if (!inQuote) {
                            // space, end of current field
                            result.Add(curField.ToString().Trim());
                            curField = new StringBuilder("");
                        }
                        else {
                            curField.Append(inputLine[pos]);
                        }
                        break;
                    case '"':
                        // quote, toggle bInQuote
                        inQuote = !inQuote;
                        break;
                    case '\\':
                        // look for \"
                        if (pos + 1 < inputLine.Length && inputLine[pos + 1] == '"') {
                            // append "
                            curField.Append('"');
                            pos++;
                        }
                        else {
                            curField.Append(inputLine[pos]);
                        }
                        break;
                    default:
                        curField.Append(inputLine[pos]);
                        break;
                }
                pos++;
            }
            if (curField.Length != 0) {
                result.Add(curField.ToString());
            }
            return result.ToArray();
        }

        public string GetHelpMessage() {
            if (cmdOptions.Count == 0) {
                return "";
            }
            StringBuilder result = new StringBuilder();

            foreach (CommandLineOption option in cmdOptions.Values) {
                char optChar = option.Option;

                string desc = (String.IsNullOrEmpty(option.ArgumentDescription) ? "argument" : option.ArgumentDescription);
                result.AppendFormat("-{0}", option.Option);

                switch (option.Argument) {
                    case Argument.None:
                        break;
                    case Argument.Optional:
                        result.AppendFormat(" [{0}]", desc);
                        break;
                    case Argument.Required:
                        result.AppendFormat(" {0}", desc);
                        break;

                }
                if (!String.IsNullOrEmpty(option.HelpText)) {
                    result.AppendFormat("\t- {0}", option.HelpText);
                }
                result.Append("\r\n");
            }

            return result.ToString();
        }
    }

    public class OptionResult
    {
        public bool IsValid { get; set; }

        public char Value { get; set; }

        public char Option { get; set; }

        public string Argument { get; set; }

        public OptionResult() {
        }
    }

    public class CommandLineOption
    {
        public char Option { get; set; }

        public Argument Argument { get; set; }

        public string ArgumentDescription { get; set; }

        public string HelpText { get; set; }

        private CommandLineOption() {
        }

        public CommandLineOption(char option, Argument argument, string argumentDescription, string helpText) {
            if (!char.IsLetterOrDigit(option)) {
                throw new ArgumentException("Option must be a letter or a digit.");
            }
            Option = option;
            Argument = argument;
            ArgumentDescription = argumentDescription;
            HelpText = helpText;
        }

        public CommandLineOption(char option, Argument argument, string argumentDescription) : this(option, argument, argumentDescription, null) {
        }

        public CommandLineOption(char option, Argument argument) : this(option, argument, "", null) {
        }

        public CommandLineOption(char option) : this(option, Argument.None, "", null) {
        }
    }
    class CommandLineParserTest
    {
        static void Main(string[] args) {
            CommandLineParser parser;
            char option;

            foreach (string arg in args) {
                Console.WriteLine("Arg: --{0}--", arg);
            }

            parser = new CommandLineParser("abcdef:g::h:i::j:k:", args);

            while (parser.OptionIndex < args.Length) {
                while ((option = parser.NextOption()) != '\0') {
                    if (option == '?') {
                        Console.WriteLine("Option {0} invalid, or missing parameter", parser.OptionCharacter);
                    }
                    else {
                        Console.WriteLine("Option: {0}", option);
                        Console.WriteLine("Parameter: {0}", (parser.OptionArgument != null ? parser.OptionArgument : ""));
                    }
                }
                if (parser.OptionIndex < args.Length) {
                    Console.WriteLine("Non-option: {0}", args[parser.OptionIndex]);
                    parser.OptionIndex++;
                }
            }

            Console.WriteLine("Help:\r\n{0}\r\n", parser.GetHelpMessage());
        }
    }
}
