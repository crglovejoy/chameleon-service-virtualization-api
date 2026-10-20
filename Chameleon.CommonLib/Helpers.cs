
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Chameleon.CommonLib
{
    public static partial class XmlRegexHelpers
    {
        // Visual Studio highlights this as a syntax error (CS8795), but it shouldn't! AI says to build it and in fact, there is no build error.
        [GeneratedRegex(@">\s+<")]
        private static partial Regex XmlWhiteSpaceRegex();
        public static string RemoveXmlWhitespace(string xmlstr) => XmlWhiteSpaceRegex().Replace(xmlstr, "><");
    }


    public static class StringHelpers
    {
        public static string CleanToSingleLineString(string strVal)
        {
            var newStrVal = strVal
                .Replace("\t", "")  // Remove tabs
                .Replace("\n", "")  // Remove newlines
                .Replace("\r", ""); // Remove carriage returns

            if (newStrVal.Contains("<"))
            {
                return XmlRegexHelpers.RemoveXmlWhitespace(newStrVal);
            }
            return newStrVal;
        }

        public static T? GetObjectFromJsonString<T>(string jsonStr)
        {
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                throw new ArgumentException("String parameter was null or whitespace", nameof(jsonStr));
            }

            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        // Convert a stream to a string.
        public static async Task<string> GetStringFromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }


    public static class EnumHelpers
    {
        public static T GetEnumFromString<T>(string inputStr, Boolean ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
                throw new ArgumentException("Input string cannot be null or whitespace", nameof(inputStr));

            try
            {
                return (T)Enum.Parse(typeof(T), inputStr, ignoreCase);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"'{inputStr}' is not a valid value for enum {typeof(T).Name}.");
            }
        }
    }


    public static class ArrayHelpers
    {
        public static T[] AddObjectToArray<T>(object obj, T[] arr)
        {
            var objList = new List<T>(arr);
            objList.AddRange((T)obj);
            return [.. objList];
        }
    }
}

