namespace DotnetSampleApp.Application.Tests.UseCases;

using DotnetSampleApp.Application.Exceptions;
using DotnetSampleApp.Application.Interfaces;
using DotnetSampleApp.Application.UseCases;
using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.Repositories;
using FluentAssertions;
using Moq;

[TestClass]
public class GetAttendeesUseCaseTests
{
    private readonly Mock<IAttendeeRepository> _repositoryMock = new();
    private GetAttendeesUseCase _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _sut = new GetAttendeesUseCase(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_データが存在する場合_DTOリストを返すこと()
    {
        // Arrange
        var attendees = new List<Attendee>
        {
            Attendee.Create("alice"),
            Attendee.Create("bob", isAttended: true)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendees);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].AccountName.Should().Be("alice");
        result[0].IsAttended.Should().BeFalse();
        result[1].AccountName.Should().Be("bob");
        result[1].IsAttended.Should().BeTrue();
    }

    [TestMethod]
    public async Task ExecuteAsync_データが空の場合_空リストを返すこと()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }
}

[TestClass]
public class UpdateAttendeeAttendanceUseCaseTests
{
    private readonly Mock<IAttendeeRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private UpdateAttendeeAttendanceUseCase _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _sut = new UpdateAttendeeAttendanceUseCase(
            _repositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_出席に更新する場合_MarkAsAttendedが呼ばれ保存されること()
    {
        // Arrange
        var attendee = Attendee.Create("alice");
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendee);

        // Act
        await _sut.ExecuteAsync(1, true);

        // Assert
        attendee.IsAttended.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_欠席に更新する場合_MarkAsNotAttendedが呼ばれ保存されること()
    {
        // Arrange
        var attendee = Attendee.Create("alice", isAttended: true);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendee);

        // Act
        await _sut.ExecuteAsync(1, false);

        // Assert
        attendee.IsAttended.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_参加者が存在しない場合_NotFoundExceptionをスローすること()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Attendee?)null);

        // Act
        var act = () => _sut.ExecuteAsync(999, true);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
