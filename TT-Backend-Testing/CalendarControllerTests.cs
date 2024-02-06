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
        private Mock<UserManager<IdentityUser>> _userManagerMock;

        private Mock<IDbContextFactory<TeamTacticsDBContext>> _contextFactoryMock;

        public CalendarControllerTests()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<TeamTacticsDBContext>>();
        }


        [Fact]
        public async Task TestCreation_General()
        {
            //SCENARIO: A team owner(coach) creates a new event for the people on their team
            // Arrange
            var options = new DbContextOptionsBuilder<TeamTacticsDBContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "CreateCalendarEventDB").Options;
            var context = new TeamTacticsDBContext(options);

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                    .Returns(new TeamTacticsDBContext(options));

            var controller = new CalendarController(_userManagerMock.Object, _contextFactoryMock.Object);

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
                TeamId = new Guid("T1")
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
                TeamId = new Guid("T1")
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
                TeamId = new Guid("T2")
            };

            await _contextFactoryMock.Object.CreateDbContext().Users.AddAsync(coachUser);
            await _contextFactoryMock.Object.CreateDbContext().Users.AddAsync(teamUser);
            await _contextFactoryMock.Object.CreateDbContext().Users.AddAsync(otherTeamUser);

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

            var result = await controller.CreateCalendarEvent(newCalendarEventDTO);

            // Assert
            Assert.IsType<OkObjectResult>(result);

        }


    }
}