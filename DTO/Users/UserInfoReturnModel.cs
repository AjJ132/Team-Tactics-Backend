namespace TeamTacticsBackend.DTO
{
    public class UserInfoReturnModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string role { get; set; }
        public Guid? teamId { get; set; }
    }
}