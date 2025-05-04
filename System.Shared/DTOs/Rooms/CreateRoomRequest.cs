namespace System.Shared.DTOs.Rooms
{
    public class CreateRoomRequest
    {
        public int StoreId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}