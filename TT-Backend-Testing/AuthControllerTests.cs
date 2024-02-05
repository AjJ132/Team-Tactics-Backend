using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Team_Tactics_Backend.Controllers;
using TeamTacticsBackend.Database;


public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private Mock<IDbContextFactory<TeamTacticsDBContext>> _contextFactoryMock;

    
    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
        _contextFactoryMock = new Mock<IDbContextFactory<TeamTacticsDBContext>>();
    }

    //SCENARIO: Registering a new user with valid model
    [Fact]
    public async Task Register_WithValidModel_ReturnsOkResult()
    {
        // Arrange
         var options = new DbContextOptionsBuilder<TeamTacticsDBContext>()
             .EnableSensitiveDataLogging()
             .UseInMemoryDatabase(databaseName: "RegisterDB").Options;
        var context = new TeamTacticsDBContext(options);

        _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new TeamTacticsDBContext(options));

            
        var controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _contextFactoryMock.Object);
        var registerModel = new RegisterModel
        {
            Email = "testuser2000@gmail.com",
            Password = "Test123$",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await controller.Register(registerModel);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        //ensure userid is returned
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.IsType<string>(okResult.Value);

    }

    //SCENARIO: Registering a new user with invalid email model
    [Fact]
    public async Task Register_WithInvalidEmailModel_ReturnsBadRequestResult()
    {
        // Arrange
        var controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _contextFactoryMock.Object);
        //incorrect email format
        var registerModel1 = new RegisterModel
        {
            Email = "testusergmail.com",
            Password = "Test123$",
            FirstName = "Test",
            LastName = "User"
        };

        //incorrect email format
        var registerModel2 = new RegisterModel
        {
            Email = "testuser@gmail",
            Password = "Test123$",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid Email" }));

        // Act
        var result1 = await controller.Register(registerModel1);
        var result2 = await controller.Register(registerModel2);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result1);

        //ensure error message is returned
        var badRequestResult = result1 as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.NotNull(badRequestResult.Value);

        //assert that status code is 400
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result2);

        //ensure error message is returned
        var badRequestResult2 = result2 as BadRequestObjectResult;
        Assert.NotNull(badRequestResult2);
        Assert.NotNull(badRequestResult2.Value);

        //assert that status code is 400
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult2.StatusCode);



    }

    //SCENARIO: Registering a new user with invalid password model
    [Fact]
    public async Task Register_WithInvalidPasswordModel_ReturnsBadRequestResult()
    {
        // Arrange
        var controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _contextFactoryMock.Object);
        //password too short
        var registerModel1 = new RegisterModel
        {
            Email = "testuser1@gmail.com",
            Password = "Test1$",
            FirstName = "Test",
            LastName = "User"
        };

        //password too long
        var registerModel2 = new RegisterModel
        {
            Email = "testuser1@gmail.com",
            Password = "TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest$",
            FirstName = "Test",
            LastName = "User"
        };

        //missing special character
        var registerModel3 = new RegisterModel
        {
            Email = "testuser1@gmail.com",
            Password = "Test1",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid Password" }));

        // Act
        var result1 = await controller.Register(registerModel1);
        var result2 = await controller.Register(registerModel2);
        var result3 = await controller.Register(registerModel3);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result1);

        //ensure error message is returned
        var badRequestResult = result1 as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.NotNull(badRequestResult.Value);

        //assert that status code is 400
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);


        // Assert
        Assert.IsType<BadRequestObjectResult>(result2);

        //ensure error message is returned
        var badRequestResult2 = result2 as BadRequestObjectResult;
        Assert.NotNull(badRequestResult2);
        Assert.NotNull(badRequestResult2.Value);

        //assert that status code is 400
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult2.StatusCode);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result3);

        //ensure error message is returned
        var badRequestResult3 = result3 as BadRequestObjectResult;
        Assert.NotNull(badRequestResult3);
        Assert.NotNull(badRequestResult3.Value);

        //assert that status code is 400
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult3.StatusCode);

    }
}
