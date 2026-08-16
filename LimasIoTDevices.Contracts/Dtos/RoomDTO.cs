namespace LimasIoTDevices.Contracts.Dtos;

public record CreateRoomRequest(string Name);
public record CreateRoomResponse(Guid Id, string Name);
