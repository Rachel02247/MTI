namespace MultiTanentInvetory.Contracts;

public interface ISignalRCallerContext
{
    string? CurrentConnectionId { get; set; }

}
