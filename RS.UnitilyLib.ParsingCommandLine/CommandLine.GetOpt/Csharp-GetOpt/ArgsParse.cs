using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.GetOpt.Csharp_GetOpt
{
    public abstract class Option
    {
        public string Name {
            get;
            private set;
        }

        private object OptionValue;

        public object Value {
            get {
                return OptionValue;
            }

            set {
                if (value is string)
                    OptionValue = FromString(value as string);
                else
                    OptionValue = value;
            }
        }

        public bool IsRequired = false;

        public bool ExpectsValue = true;

        public Option(string optionName, bool expectsValue, bool isOptionRequired = false, object defaultValue = null) {
            Name = optionName;
            ExpectsValue = expectsValue;
            IsRequired = isOptionRequired;
            Value = defaultValue;
        }

        public abstract object FromString(string val);
    }

    public class Option<T> : Option
    {
        public Option(string optionName, bool expectsValue = true, bool isOptionRequired = false, T defaultValue = default(T)) :
            base(optionName, expectsValue, isOptionRequired, defaultValue) { }

        public override object FromString(string val) {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
                throw new InvalidOperationException($"The type {typeof(T).FullName} does not support converting from string");
            return converter.ConvertFrom(val);
        }
    }

    public class Parser
    {
        public Dictionary<string, Option> Options;
        public List<string> OptionNamePrefixes = new List<string>();

        public Parser(List<string> optionNamePrefixes = null) {
            /* if no path prefixes were specified, use the default, sane list */
            if (optionNamePrefixes == null)
                optionNamePrefixes = new List<string>() { "--", "-", "/" };

            Options = new Dictionary<string, Option>();

            /* convert to a list to clone it */
            OptionNamePrefixes = optionNamePrefixes.ToList();
        }

        public void AddOption(Option opt) {
            if (Options.ContainsKey(opt.Name))
                throw new ArgumentException($"Option {opt.Name} already exists");
            Options.Add(opt.Name, opt);
        }

        public void Parse(string[] args) {
            Option currentOption = null;
            List<Option> foundOptions = new List<Option>();

            foreach (string arg in args) {
                if (currentOption == null) {
                    foreach (string prefix in OptionNamePrefixes) {
                        if (arg.StartsWith(prefix) &&
                            Options.ContainsKey(arg.Substring(prefix.Length))) {
                            currentOption = Options[arg.Substring(prefix.Length)];
                            break;
                        }
                    }

                    if (currentOption == null)
                        throw new Exception($"Unknown option: {arg}");

                    if (!currentOption.ExpectsValue) {
                        currentOption.Value = true;
                        foundOptions.Add(currentOption);
                        currentOption = null;
                    }
                }
                else {
                    if (currentOption.ExpectsValue) {
                        foreach (string prefix in OptionNamePrefixes) {
                            if (arg.StartsWith(prefix))
                                throw new Exception($"Option {currentOption.Name} expects a value.");
                        }

                        currentOption.Value = arg;
                    }
                    else {
                        currentOption.Value = true;
                    }

                    foundOptions.Add(currentOption);
                    currentOption = null;
                }
            }

            if (currentOption != null && currentOption.ExpectsValue)
                throw new ArgumentException($"Option {currentOption.Name} expects an argument");

            foreach (Option opt in Options.Values) {
                if (opt.IsRequired && !foundOptions.Contains(opt))
                    throw new ArgumentException($"Option {opt.Name} is requiered");
            }
        }
    }

    class Csharp_GetOpt_Test
    {
        static void Main(string[] args) {
            string[] data = "--help --name kevin --status single --test2 69".Split(' ');
            //data = "--help --name kevin --status".Split(' ');
            //data = "--help --status single".Split(' ');
            //data = "--help --name kevin".Split(' ');
            //data = "--help hilfe --name kevin --status single".Split(' ');

            Parser argParser = new Parser();

            Option help = new Option<bool>("help", false);
            Option name = new Option<string>("name", true, true);
            Option status = new Option<string>("status", true, false, "single");
            Option testNum = new Option<int>("test", true, false, 123);
            Option test2Num = new Option<int>("test2", true, true, 123);
            Option test3Num = new Option<int>("test3", true, false);
            Option nulltest = new Option<string>("null", true, false, null);
            Option abcd = new Option<string>("abcd", true, false, "A");

            argParser.AddOption(help);
            argParser.AddOption(name);
            argParser.AddOption(status);
            argParser.AddOption(testNum);
            argParser.AddOption(test2Num);
            argParser.AddOption(test3Num);
            argParser.AddOption(nulltest);
            argParser.AddOption(abcd);

            try {
                argParser.Parse(data);

                foreach (var option in argParser.Options.Values) {
                    Console.WriteLine($"{option.Name} => {option.Value}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
