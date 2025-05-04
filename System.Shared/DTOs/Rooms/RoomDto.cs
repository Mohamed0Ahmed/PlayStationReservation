namespace System.Shared.DTOs.Rooms
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int StoreId { get; set; }

    }
}
