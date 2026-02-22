namespace DotnetSampleApp.Domain.Tests.Entities;

using DotnetSampleApp.Domain.Entities;
using FluentAssertions;

[TestClass]
public class AttendeeTests
{
    [TestMethod]
    public void Create_有効なパラメータの場合_エンティティが生成されること()
    {
        // Arrange & Act
        var attendee = Attendee.Create("alice");

        // Assert
        attendee.AccountName.Should().Be("alice");
        attendee.IsAttended.Should().BeFalse();
    }

    [TestMethod]
    public void Create_出席フラグ指定の場合_指定値で生成されること()
    {
        // Arrange & Act
        var attendee = Attendee.Create("bob", isAttended: true);

        // Assert
        attendee.AccountName.Should().Be("bob");
        attendee.IsAttended.Should().BeTrue();
    }

    [TestMethod]
    public void Create_accountNameがnullの場合_ArgumentExceptionをスローすること()
    {
        // Arrange & Act
        var act = () => Attendee.Create(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Create_accountNameが空文字の場合_ArgumentExceptionをスローすること()
    {
        // Arrange & Act
        var act = () => Attendee.Create("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void MarkAsAttended_呼び出した場合_IsAttendedがtrueになること()
    {
        // Arrange
        var attendee = Attendee.Create("alice");

        // Act
        attendee.MarkAsAttended();

        // Assert
        attendee.IsAttended.Should().BeTrue();
    }

    [TestMethod]
    public void MarkAsNotAttended_呼び出した場合_IsAttendedがfalseになること()
    {
        // Arrange
        var attendee = Attendee.Create("alice", isAttended: true);

        // Act
        attendee.MarkAsNotAttended();

        // Assert
        attendee.IsAttended.Should().BeFalse();
    }
}
