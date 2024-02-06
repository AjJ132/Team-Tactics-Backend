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
using Microsoft.AspNetCore.Http;

namespace TT_Backend_Testing
{
    public class CalendarControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;

        private Mock<IDbContextFactory<TeamTacticsDBContext>> _contextFactoryMock;

        private CalendarController _controller;

        public CalendarControllerTests()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<TeamTacticsDBContext>>();

            _controller = new CalendarController(_userManagerMock.Object, _contextFactoryMock.Object);

            SetupUserForController();
        }

        public void SetupUserForController()
        {
            // Identity User
            var identityUser = new IdentityUser
            {
                Id = "1",
                UserName = "Test User",
                Email = "testing@gmail.com",
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(identityUser);

            // User Claims
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                // Add any other claims as needed.
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Mock HttpContext
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.User).Returns(principal);

            // Mock ControllerContext
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Object.HttpContext = httpContextMock.Object;

            _controller.ControllerContext = controllerContextMock.Object;
        }


        [Fact]
        public async Task TestCreation_General()
        {
            //SCENARIO: A team owner(coach) creates a new event for the people on their team
            // Arrange
            var options = new DbContextOptionsBuilder<TeamTacticsDBContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "CreateCalendarEventDB").Options;
            

            Guid teamID = new Guid();

            //Create three users (1 coach, 1 person on same teamm, 1 person on different team)
            var coachIU = new IdentityUser
            {
                Id = "1",
                UserName = "Coach",
            };
            var coachUser = new User
            {
                Id = "1",
                FirstName = "Coach",
                LastName = "User",
                TeamId = teamID
            };

            var teamUserIU = new IdentityUser
            {
                Id = "2",
                UserName = "TeamUser",
            };
            var teamUser = new User
            {
                Id = "2",
                FirstName = "Team",
                LastName = "User",
                TeamId = teamID
            };

            var otherTeamUserIU = new IdentityUser
            {
                Id = "3",
                UserName = "OtherTeamUser",
            };
            var otherTeamUser = new User
            {
                Id = "3",
                FirstName = "OtherTeam",
                LastName = "User",
                TeamId = new Guid()
            };

            var context = new TeamTacticsDBContext(options);

            await context.Users.AddAsync(coachUser);
            await context.Users.AddAsync(teamUser);
            await context.Users.AddAsync(otherTeamUser);

            await context.SaveChangesAsync();


            _contextFactoryMock.Setup(f => f.CreateDbContext())
                    .Returns(new TeamTacticsDBContext(options));


            //register identity user

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var newCalendarEventDTO = new NewCalendarEventDTO
            {
                Title = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                Color = "#FF0000",
                UserIds = new List<string> { "2" }
            };

            var result = await _controller.CreateCalendarEvent(newCalendarEventDTO);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            //Verify fields
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var calendarEvent = okResult.Value as ReturnCalendarEventDTO;

            Assert.NotNull(calendarEvent);

            Assert.Equal("Test Event", calendarEvent.Title);
            Assert.Equal("Test Description", calendarEvent.Description);
            Assert.Equal("#FF0000", calendarEvent.Color);
            Assert.Equal("Coach User", calendarEvent.AssigneeName);
            Assert.Single(calendarEvent.AssignedUsers);
            Assert.Equal("Team User", calendarEvent.AssignedUsers[0]);
            
        }


    }
}