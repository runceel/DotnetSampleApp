namespace DotnetSampleApp.Infrastructure.Tests.Repositories;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Infrastructure.Persistence;
using DotnetSampleApp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[TestClass]
public class AttendeeRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [TestMethod]
    public async Task AddAsync_GetAllAsync_追加した参加者が取得できること()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new AttendeeRepository(dbContext);
        var attendee = Attendee.Create("alice");

        // Act
        await repository.AddAsync(attendee);
        await dbContext.SaveChangesAsync();
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].AccountName.Should().Be("alice");
    }

    [TestMethod]
    public async Task GetByIdAsync_存在するIDの場合_参加者を返すこと()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new AttendeeRepository(dbContext);
        var attendee = Attendee.Create("bob");

        await repository.AddAsync(attendee);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(attendee.Id);

        // Assert
        result.Should().NotBeNull();
        result!.AccountName.Should().Be("bob");
    }

    [TestMethod]
    public async Task GetByIdAsync_存在しないIDの場合_nullを返すこと()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new AttendeeRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
