namespace eStoreAPI.ViewEntities
{
    public class MemberVE
    {
        public int MemberId { get; set; }
        public string Email { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
