
namespace Chameleon.CommonApiLib
{
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
}