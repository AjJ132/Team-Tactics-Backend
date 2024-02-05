using Microsoft.EntityFrameworkCore;
using Xunit;
using TeamTacticsBackend.Database;
using Moq;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Team_Tactics_Backend.CalendarControllers;
using TeamTacticsBackend.Models.Users;
using Team_Tactics_Backend.DTO;
using Team_Tactics_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace TT_Backend_Testing
{
    public class CalendarControllerTests
    {
        private Mock<IDbContextFactory<TeamTacticsDBContext>> _contextFactoryMock;
        private UserManager<IdentityUser> _userManagerMock;
        private Mock<SignInManager<IdentityUser>> _signInManagerMock;

        private CalendarController _calendarController;
        private AuthController _authController;

        public CalendarControllerTests()
        {
            _userManagerMock = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new TeamTacticsDBContext(new DbContextOptions<TeamTacticsDBContext>())), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<TeamTacticsDBContext>>();

            _authController = new AuthController(_userManagerMock, _signInManagerMock.Object, _contextFactoryMock.Object);
            
            GenerateUsers();
            VerifyData();

            _calendarController = new CalendarController(_userManagerMock, _contextFactoryMock.Object);

            Debug.WriteLine("Setup Complete");
        }


        private async Task GenerateUsers()
        {
            var options = new DbContextOptionsBuilder<TeamTacticsDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            // Create a new DbContext with the in-memory database
            var context = new TeamTacticsDBContext(options);

            var teams = await GenerateTestUsers.GenerateTeams();
            var users = await GenerateTestUsers.GenerateUsers();

            foreach (var user in users)
            {
                var identUser = new IdentityUser
                {
                    Id = user.Id,
                    UserName = user.FirstName + user.LastName,
                    Email = user.FirstName + "@gmail.com",
                };

                var newUserID = await _authController.Register(new RegisterModel
                {
                    Email = user.FirstName + "@gmail.com",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = "Password123!"
                });

                user.Id = newUserID.ToString();

                //update team id in db
                var dbUser = await context.Users.FirstOrDefaultAsync(n => n.Id == user.Id);
                dbUser.TeamId = user.TeamId;

                await context.SaveChangesAsync();
            }

            foreach (var team in teams)
            {
                await context.Teams.AddAsync(team);
            }

            await context.SaveChangesAsync();

            // Mock the DbContextFactory to return the new DbContext
            _contextFactoryMock.Setup(m => m.CreateDbContext()).Returns(new TeamTacticsDBContext(options));
        }

        private async Task VerifyData()
        {
            // Access the custom Users table
            var customUsers = await _contextFactoryMock.Object.CreateDbContext().Users.ToListAsync();
            if(customUsers.Count != 12)
            {
                Assert.Fail("Users not created");
            }

            var identUsers = await _userManagerMock.Users.ToListAsync();

            if(identUsers.Count != 12)
            {
                Assert.Fail("Users not created");
            }
        }


        [Fact]
        public async Task TestCreation_General()
        {
            //SCENARIO: A team owner(coach) creates a new event for the people on their team

            //Grab team1 id 
            var team1ID = GenerateTestUsers.TeamID1;
            var team1OwnerID = await _contextFactoryMock.Object.CreateDbContext().Teams.Where(n => n.TeamId == team1ID).Select(n => n.OwnerId).FirstOrDefaultAsync();

            //grab all users on team1
            var userIds = await _contextFactoryMock.Object.CreateDbContext().Users.Where(n => n.TeamId == team1ID).Select(n => n.Id).ToListAsync();

            //Setup
            var mockUserManager = new Mock<UserManager<IdentityUser>>();
            var mockUser = await _contextFactoryMock.Object.CreateDbContext().Users.FirstOrDefaultAsync(n => n.Id == team1OwnerID);

            //Create a new event
            var newEvent = new NewCalendarEventDTO
            {
                Title = "Test Title",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Color = "#FFFFFF",
                UserIds = userIds
            };

            //Act
            var result = await _calendarController.CreateCalendarEvent(newEvent);

            //Assert
            if (result is OkObjectResult)
            {
                
            }
            else
            {
                Assert.Fail("Not OK Result");
            }
            
        }

       
    }
}