using Chameleon.CommonLib;

namespace Chameleon.CommonApiLib
{
    /* These classes are used to create test data for unit testing and also (in the development environment) for storing in the 
     * Cosmos database for integration/end-to-end testing. */


    /******************/
    /**  TEST HIKER  **/
    /******************/

    public class TestHiker
    {
        public string FirstName = "Bat";
        public string LastName = "Man";
        public Guid HikerId = Guid.Parse("b1754c14-d296-4b0f-a09a-030017f4461f");
        public DateTime BirthDate = DateTime.Parse("01/01/1990");
        public AddressData Address = new();
        public Hike[] FavoriteHikeArr = [
                    new Hike("Dark Cave", "New York"),
                    new Hike("Penguin Rapids", "Idaho"),
                    new Hike("Joker Falls", "New Jersey")
                ];
        public float FavoriteFloat = 25.5f;
        public int FavoriteInt = 8;
        public long FavoriteLong = 987654321L;
        public decimal FavoriteDecimal = 30.33m;
        public double FavoriteDouble = 66.66d;
        public string[] FavoriteEmptyArr = [];
        public string[] FavoriteColorArr = ["blue", "green", "red"];
        public string FavoriteNullValue = null!;
        public Boolean FavoriteBool = true;
        public Boolean LeastFavoriteBool = false;
        public DateTime FavoriteDateTime = DateTime.Parse("10/08/2026 12:30:00.123 PM");

        public static string GetSchemaHashString()
        {
            return "18401841894470789066";
        }
    }

    public class AddressData
    {
        public string Street = "The Big Mansion Street";
        public string City = "Gotham City";
        public string State = "New York";
        public int Zip = 12345;
    }

    public class Hike(string name, string state)
    {
        public string Name = name;
        public string State = state;
    }



    /***************************/
    /**  TEST HIKER RESPONSE  **/
    /***************************/

    public class TestHikerResponse
    {
        public string FirstName;
        public string LastName;
        public int HikerRating;
        public string FavoriteColor;
        public string MatchType;

        public TestHikerResponse(Boolean usePropertyVariables)
        {
            var propVarsArr = GetPropertyVariablesArr();

            FirstName = (usePropertyVariables && propVarsArr.Contains("FirstName")) ? RespDoc.WrapStringWithVariableDelimiter("FirstName") : "In";
            LastName = (usePropertyVariables && propVarsArr.Contains("LastName")) ? RespDoc.WrapStringWithVariableDelimiter("LastName") : "Cognito";
            HikerRating = -1;
            FavoriteColor = (usePropertyVariables && propVarsArr.Contains("FavoriteColorArr[0]")) ? RespDoc.WrapStringWithVariableDelimiter("FavoriteColorArr[0]") : "Grey";
            MatchType = "RouteMatch";
        }

        public static string[] GetPropertyVariablesArr()
        {
            return ["FirstName", "LastName", "FavoriteColorArr[0]"];
        }
    }


    /*
    ***  MESSAGE PROPERTIES  ***
    Flattened Dictionary:
    ====================="
    FirstName = Bat
    LastName = Man
    HikerId = b1754c14-d296-4b0f-a09a-030017f4461f
    BirthDate = 1/1/1990 12:00:00 AM
    Address.Street = The Big Mansion Street
    Address.City = Gotham City
    Address.State = New York
    Address.Zip = 12345
    FavoriteHikeArr[0].Name = Dark Cave
    FavoriteHikeArr[0].State = New York
    FavoriteHikeArr[1].Name = Penguin Rapids
    FavoriteHikeArr[1].State = Idaho
    FavoriteHikeArr[2].Name = Joker Falls
    FavoriteHikeArr[2].State = New Jersey
    FavoriteFloat = 25.5
    FavoriteInt = 8
    FavoriteLong = 987654321
    FavoriteDecimal = 30.33
    FavoriteDouble = 66.66
    FavoriteColorArr[0] = blue
    FavoriteColorArr[1] = green
    FavoriteColorArr[2] = red
    FavoriteBool = True
    LeastFavoriteBool = False
    FavoriteDateTime = 10/8/2026 12:30:00 PM

    Other Property Values:
    ======================
    digital signature hash = 18401841894470789066
    original message = {"FirstName":"Bat","LastName":"Man","HikerId":"b1754c14-d296-4b0f-a09a-030017f4461f","BirthDate":"1990-01-01T00:00:00","Address":{"Street":"The Big Mansion Street","City":"Gotham City","State":"New York","Zip":12345},"FavoriteHikeArr":[{"Name":"Dark Cave","State":"New York"},{"Name":"Penguin Rapids","State":"Idaho"},{"Name":"Joker Falls","State":"New Jersey"}],"FavoriteFloat":25.5,"FavoriteInt":8,"FavoriteLong":987654321,"FavoriteDecimal":30.33,"FavoriteDouble":66.66,"FavoriteEmptyArr":[],"FavoriteColorArr":["blue","green","red"],"FavoriteNullValue":null,"FavoriteBool":true,"LeastFavoriteBool":false,"FavoriteDateTime":"2026-10-08T12:30:00.123"}
    */
}