using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PTClient.Generator
{
    class Program
    {
        private const string CurrentNamespaceName = "Telegram.Td.Generator";
        private const string FunctionResourceName = CurrentNamespaceName + ".Function.tmp";
        private const string TdApiResouceName = CurrentNamespaceName + ".td_api.tl";
        private const string FunctionPattern = @"\b(?:[a-z1-9:]{2,}|[ai])\b";
        internal static void Main()
        {
            ColoredWriteLine(Context.INFORMATION, ConsoleColor.DarkGreen); ;
            while (true)
            {
                ColoredWriteLine("Enter command : ", ConsoleColor.Yellow);
                string command = ReadLine();
                if (command == "Generate")
                {
                    Generate();
                }
                else if (command == "Exit")
                {
                    break;
                }
            }
        }
        private static void Generate()
        {
            string functionBody = ReadResouce(FunctionResourceName);
            string[] schemeContent = ReadResouce(TdApiResouceName).Split("\n");
            string outPutDirectory = GetDirectory();
            List<string> functions = schemeContent.Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith("/")).ToList();
            foreach (string function in functions)
            {
                MatchCollection matches = Regex.Matches(function, FunctionPattern, RegexOptions.IgnoreCase);
                string schemeFunctionName = matches.First().Value;
                string functionName = char.ToUpper(schemeFunctionName[0]) + schemeFunctionName[1..];
                string returnTypeName = matches.Last().Value;
                string generatedFunction = functionBody.Replace("RETURN_TYPE", returnTypeName).Replace("FUNCTION_NAME", functionName).Replace("FUNCTION_PARAMETER_NAME", schemeFunctionName);
                File.WriteAllText(outPutDirectory + "/" + functionName + ".cs", generatedFunction);
            }
            ColoredWriteLine("Successful", ConsoleColor.Green);
        }
        private static string GetDirectory()
        {
            ColoredWriteLine("Please enter directory path : ", ConsoleColor.Yellow);
            string directoryPath = ReadLine();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                ColoredWriteLine("Directory created", ConsoleColor.Green);
            }
            return directoryPath;
        }
        private static void ColoredWriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        private static string ReadLine()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("> ");
            Console.ResetColor();
            return Console.ReadLine();
        }
        private static string ReadResouce(string resouceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resouceName);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
