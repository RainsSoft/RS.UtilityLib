# cli-parser

Getopt but for C#.

## Installation

Simply copy the CommandLineHelpers.cs file onto your project. All objects are in `namespace CliParser`.

## Usage

To use `cli-parser`, specify an array of `CommandLineOption` and the command string array to parse (usually `string[] args` in `Main`). Then, pass an `Action<string, string[]>` which receives the current option being parsed and its corresponding arguments.

## Example

```csharp
using CliParser;

CommandLineOption[] opts = new []
{
  new()
  {
    Option = "help",
    IsLongOption = true,
    RequiresArgument = false
  },
  new()
  {
    Option = "test",
    IsLongOption = true,
    RequiresArgument = true
  },
  new()
  {
    Option = "b",
    IsLongOption = false,
    RequiresArgument = false
  }
};

CommandLineHelpers.ParseCommandLine(opts, args, (opt, arg) =>
{
  switch (opt)
  {
    case "help":
      Console.WriteLine("--help passed");
      break;
    case "test":
      Console.WriteLine("--test passed with args: {0}", string.Join(",", arg));
      break;
    case "b":
      Console.WriteLine("-b passed");
      break;
  }
}
```

This example program will print `--help passed` when --help is passed, `-b passed` when -b is passed, and `--test passed with args: <csv-args>` when --test is passed with an argument (multiple --test options can be passed).

## Notes

- A long option and its argument (if applicable) may be separated by an equals sign, a colon, or space/s.
- Short options can be combined as if they were one single option.
- Separating the argument for a short option with space/s is optional.
- Arguments for options are represented as an array, in case duplicate options appear.

Full example:
```
program --long arg --long arg2 -DDebug --long1:arg --xyz=arg -abc
```

```
{
  "long": ["arg", "arg2"],
  "D": ["Debug"],
  "long1": ["arg"],
  "xyz": ["arg"],
  "a": [],
  "b": [],
  "c": []
}
```
