namespace DotnetSampleApp.Domain.Entities;

public class Attendee
{
    public int Id { get; private set; }
    public string AccountName { get; private set; }
    public bool IsAttended { get; private set; }

    private Attendee()
    {
        AccountName = null!;
    }

    public static Attendee Create(string accountName, bool isAttended = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(accountName);

        return new Attendee
        {
            AccountName = accountName,
            IsAttended = isAttended
        };
    }

    public void MarkAsAttended()
    {
        IsAttended = true;
    }

    public void MarkAsNotAttended()
    {
        IsAttended = false;
    }
}
