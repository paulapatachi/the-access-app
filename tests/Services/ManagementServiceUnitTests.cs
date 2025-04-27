using Moq;
using MyApp.Domain.Enums;
using MyApp.Infrastructure.Commands;
using MyApp.Infrastructure.Services.Management;
using Xunit.Abstractions;

namespace MyApp.Tests.Services;

public class ManagementServiceUnitTests
{
    private readonly Mock<IManagementCommand> _mockCommand;
    private readonly ManagementService _service;

    public ManagementServiceUnitTests()
    {
        _mockCommand = new Mock<IManagementCommand>();
        _mockCommand.SetupGet(c => c.Command).Returns(CommandType.Availability);

        var commands = new List<IManagementCommand> { _mockCommand.Object };
        _service = new ManagementService(commands);
    }

    [Fact]
    public async Task ExecuteCommandAsync_ShouldExecuteCorrectCommand_WhenValidInput()
    {
        // Arrange
        var commandInput = "Availability(H1, 20240901, DBL)";

        // Act
        await _service.ExecuteCommandAsync(commandInput, CancellationToken.None);

        // Assert
        _mockCommand.Verify(c => c.ExecuteAsync(It.Is<string[]>(p => p.Length == 3), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task ExecuteCommandAsync_ShouldLogInvalidCommand_WhenCommandFormatIsInvalid()
    {
        // Arrange
        var commandInput = "Invalid Command Format";
        
        // Set up the StringWriter to capture the Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _service.ExecuteCommandAsync(commandInput, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Invalid command format.", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
    
    [Fact]
    public async Task ExecuteCommandAsync_ShouldLogInvalidCommand_WhenCommandTypeIsUnknown()
    {
        // Arrange
        var commandInput = "InvalidCommand(H1)";
        
        // Set up the StringWriter to capture the Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _service.ExecuteCommandAsync(commandInput, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Invalid command type:", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }

    [Fact]
    public async Task ExecuteCommandAsync_ShouldNotExecute_WhenCommandNotFound()
    {
        // Arrange
        var commandInput = "RoomTypes(H1, 20240901, 2)";
        
        // Set up the StringWriter to capture the Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _service.ExecuteCommandAsync(commandInput, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Command not found", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }

    [Fact]
    public async Task ExecuteCommandAsync_ShouldHandleCommandExecutionException_WhenExceptionRaised()
    {
        // Arrange
        var commandInput = "Availability(H1, 20240901, DBL)";
        
        // Make the command throw an exception
        _mockCommand
            .Setup(c => c.ExecuteAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Command execution failed"));

        // Set up the StringWriter to capture the Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        // Act
        await _service.ExecuteCommandAsync(commandInput, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Error executing command type", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
}
