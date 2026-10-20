using Chameleon.CommonLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.CommonApiLib
{
    /* This class is used to create test data for unit testing and also (in the development environment) for storing in the 
     * Cosmos database for integration/end-to-end testing. */


    /**********************/
    /**  TEST XML BOOKS  **/
    /**********************/

    public static class TestXmlBooks
    {
        public const string SchemaHashString = "10693043588415836274";
        public const string XmlRequestMessage =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<bookstore>\r\n    <book category=\"fiction\">\r\n        <title lang=\"en\">The Great Gatsby</title>\r\n        <author>F. Scott Fitzgerald</author>\r\n        <year>1925</year>\r\n        <price>14.99</price>\r\n    </book>\r\n    <book category=\"programming\">\r\n        <title lang=\"en\">Learning XML</title>\r\n        <author>Erik T. Ray</author>\r\n        <year>2003</year>\r\n        <price>39.95</price>\r\n    </book>\r\n\t<book category=\"self improvement\">\r\n\t\t<title lang=\"en\">Mastering the Flop</title>\r\n\t\t<author>Luis Suarez</author>\r\n\t\t<year>2030</year>\r\n\t\t<price>400.00</price>\r\n\t</book>\r\n</bookstore>";

        public static string GetResponseStr(Boolean usePropertyVariables)
        {
            if (usePropertyVariables)
            {
                return @"<?xml version=""1.0"" encoding=""UTF-8""?><bookstore><book category=""fiction""><title lang=""en"">`bookstore.book[0].title.#text`</title><author>F. Scott Fitzgerald</author><year>1925</year><price>14.99</price></book><book category=""programming""><title lang=""en"">Learning XML</title><author>Erik T. Ray</author><year>2003</year><price>`bookstore.book[1].price`</price></book><book category=""self improvement""><title lang=""en"">Mastering the Flop</title><author>`bookstore.book[2].author`</author><year>2030</year><price>400.00</price></book></bookstore>";
            }

            return @"<?xml version=""1.0"" encoding=""UTF-8""?><bookstore><book category=""fiction""><title lang=""en"">The Great Gatsby</title><author>F. Scott Fitzgerald</author><year>1925</year><price>14.99</price></book><book category=""programming""><title lang=""en"">Learning XML</title><author>Erik T. Ray</author><year>2003</year><price>39.95</price></book><book category=""self improvement""><title lang=""en"">Mastering the Flop</title><author>Luis Suarez</author><year>2030</year><price>400.00</price></book></bookstore>";
        }

        public static string[] GetPropertyVariablesArr()
        {
            return ["bookstore.book[0].title.#text", "bookstore.book[1].price", "bookstore.book[2].author"];
        }
    }
}


/*
 <?xml version="1.0" encoding="UTF-8"?>
 <bookstore>
     <book category="fiction">
         <title lang="en">The Great Gatsby</title>
         <author>F. Scott Fitzgerald</author>
         <year>1925</year>
         <price>14.99</price>
     </book>
     <book category="programming">
         <title lang="en">Learning XML</title>
         <author>Erik T. Ray</author>
         <year>2003</year>
         <price>39.95</price>
     </book>
     <book category="self improvement">
         <title lang="en">Mastering the Flop</title>
         <author>Luis Suarez</author>
         <year>2030</year>
         <price>400.00</price>
     </book>
 </bookstore>
 */


/*
***  MESSAGE PROPERTIES  ***
Flattened Dictionary:
====================="
?xml.@version = 1.0
?xml.@encoding = UTF-8
bookstore.book[0].@category = fiction
bookstore.book[0].title.@lang = en
bookstore.book[0].title.#text = The Great Gatsby
bookstore.book[0].author = F. Scott Fitzgerald
bookstore.book[0].year = 1925
bookstore.book[0].price = 14.99
bookstore.book[1].@category = programming
bookstore.book[1].title.@lang = en
bookstore.book[1].title.#text = Learning XML
bookstore.book[1].author = Erik T. Ray
bookstore.book[1].year = 2003
bookstore.book[1].price = 39.95
bookstore.book[2].@category = self improvement
bookstore.book[2].title.@lang = en
bookstore.book[2].title.#text = Mastering the Flop
bookstore.book[2].author = Luis Suarez
bookstore.book[2].year = 2030
bookstore.book[2].price = 400.00

Other Property Values:
======================
digital signature hash = 10693043588415836274
original message = original message = <?xml version="1.0" encoding="UTF-8"?><bookstore><book category="fiction"><title lang="en">The Great Gatsby</title><author>F. Scott Fitzgerald</author><year>1925</year><price>14.99</price></book><book category="programming"><title lang="en">Learning XML</title><author>Erik T. Ray</author><year>2003</year><price>39.95</price></book><book category="self improvement"><title lang="en">Mastering the Flop</title><author>Luis Suarez</author><year>2030</year><price>400.00</price></book></bookstore>

*/