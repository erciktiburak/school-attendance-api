using Microsoft.AspNetCore.SignalR;

namespace SchoolAttendance.API.Hubs;

public class AttendanceHub : Hub
{
    public async Task SendAttendanceUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveAttendanceUpdate", message);
    }

    public async Task NotifyCheckIn(object data)
    {
        await Clients.All.SendAsync("StudentCheckedIn", data);
    }

    public async Task NotifyCheckOut(object data)
    {
        await Clients.All.SendAsync("StudentCheckedOut", data);
    }

    public async Task NotifyNewStudent(object data)
    {
        await Clients.All.SendAsync("NewStudentAdded", data);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
