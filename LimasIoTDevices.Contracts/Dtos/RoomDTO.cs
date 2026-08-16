namespace LimasIoTDevices.Facade.Dtos;

public record CreateRoomRequest(string Name);
public record CreateRoomResponse(Guid Id, string Name);
