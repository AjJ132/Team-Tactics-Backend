using TeamTacticsBackend.Models.Teams;
using TeamTacticsBackend.Models.Users;

public static class GenerateTestUsers
{
    public static Guid TeamID1 = Guid.NewGuid();
    public static Guid TeamID2 = Guid.NewGuid();
    public static Guid TeamID3 = Guid.NewGuid();
    public static async Task<List<Team>> GenerateTeams()
    {
        //Generate three teams and return
        var teams = new List<Team>
        {
            new Team
            {
                TeamId = TeamID1,
                OwnerId = "1",
                TeamName = "Team 1",
                TeamSport = "Soccer",
                TeamCity = "New York",
                TeamState = "NY",
                TeamJoinCode = "1234",
                DateCreated = DateTime.Now
            },
            new Team
            {
                TeamId = TeamID2,
                OwnerId = "2",
                TeamName = "Team 2",
                TeamSport = "Basketball",
                TeamCity = "Los Angeles",
                TeamState = "CA",
                TeamJoinCode = "5678",
                DateCreated = DateTime.Now
            },
            new Team
            {
                TeamId = TeamID3,
                OwnerId = "3",
                TeamName = "Team 3",
                TeamSport = "Football",
                TeamCity = "Chicago",
                TeamState = "IL",
                TeamJoinCode = "91011",
                DateCreated = DateTime.Now
            }
        };

        return teams;
    }
    public static async Task<List<User>> GenerateUsers()
    {
        //Generate ten users and return
        //Ensure that the user's teamId is set to the teamId of the team they are in and three team owners are selected
        var users = new List<User>
        {
            new User
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID1,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "2",
                FirstName = "Jane",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID2,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "3",
                FirstName = "Jim",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID3,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "4",
                FirstName = "Jack",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID1,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "5",
                FirstName = "Jill",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID2,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "6",
                FirstName = "Joe",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID3,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "7",
                FirstName = "Jenny",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID1,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "8",
                FirstName = "Jesse",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID2,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "9",
                FirstName = "Jared",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID3,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "10",
                FirstName = "Jasmine",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID1,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "11",
                FirstName = "Jared",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID2,
                DateJoined = DateTime.Now
            },
            new User
            {
                Id = "12",
                FirstName = "Jasmine",
                LastName = "Doe",
                UserType = 0,
                TeamId = TeamID3,
                DateJoined = DateTime.Now
            }
        };
    
        return users;
    }
}