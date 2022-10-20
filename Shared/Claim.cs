namespace Shared
{
    public class Claim
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public int Income { get; set; }
        public string Status { get; set; } = "Not submitted";

    }
}