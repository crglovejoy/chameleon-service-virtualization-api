
using Chameleon.CommonLib;

namespace UnitTest
{
    internal static class CommonTestVals
    {
        internal const Boolean Debug = true;
        internal const string XmlStringExpectedSignatureHash = "11965195528540695537";
        internal const string HikerExpectedSignatureHash = "18401841894470789066";

        internal static readonly string MessageXmlString =
            @"<?xml version=""1.0""?>
            <customers>
                <customer id=""55000"">
                    <name>Charter Group</name>
                    <address>
                        <street>100 Main</street>
                        <city>Framingham</city>
                        <state>MA</state>
                        <zip>01701</zip>
                    </address>
                    <address>
                        <street>720 Prospect</street>
                        <city>Framingham</city>
                        <state>MA</state>
                        <zip>01701</zip>
                    </address>
                    <address>
                        <street>120 Ridge</street>
                        <state>MA</state>
                        <zip>01760</zip>
                    </address>
                    <wealth>
                        <bank>100.25</bank>
                        <stock>.25</stock>
                        <car>0.0</car>
                        <realestate>0.10</realestate>
                    </wealth>
                </customer>
            </customers>";
    }

    public static class OutputHelpers
    {
        public static void WriteOutMessageProperties(MessageProperties mProps)
        {
            TestContext.Out.WriteLine("***  MESSAGE PROPERTIES  ***");
            TestContext.Out.WriteLine($"Flattened Dictionary:{Environment.NewLine}=====================\"");
            foreach (var kvp in mProps.messageDict)
                TestContext.Out.WriteLine($"{kvp.Key} = {kvp.Value}");
            TestContext.Out.WriteLine($"{Environment.NewLine}Other Property Values:{Environment.NewLine}======================");
            TestContext.Out.WriteLine($"digital signature hash = {mProps.schemaSignatureHash}");
            TestContext.Out.WriteLine($"original message = {mProps.originalMessageStr}");
        }
    }
}
