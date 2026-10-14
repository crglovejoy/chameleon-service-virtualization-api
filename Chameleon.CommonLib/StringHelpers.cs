
using Newtonsoft.Json;

namespace Chameleon.CommonLib
{
    public static class StringHelpers
    {
        public static string CleanToSingleLineString(string strVal)
        {
            return strVal
                .Replace("\t", "")  // Remove tabs
                .Replace("\n", "")  // Remove newlines
                .Replace("\r", ""); // Remove carriage returns
        }

        public static T? GetObjectFromJsonString<T>(string jsonStr)
        {
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                throw new ArgumentException("String parameter was null or whitespace", nameof(jsonStr));
            }

            return JsonConvert.DeserializeObject<T>(jsonStr);
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
}

