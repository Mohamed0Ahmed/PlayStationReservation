using System.Shared.DTOs.Rooms;

namespace System.Shared.DTOs.Stores
{
    public class StoreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerEmail { get; set; }
        public List<RoomDto> Rooms { get; set; } = new List<RoomDto>();
    }
}
