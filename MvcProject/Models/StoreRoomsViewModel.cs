using System.Domain.Models;

namespace MvcProject.Models
{
    public class StoreRoomsViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!;
        public List<Room> ActiveRooms { get; set; } = new List<Room>();
        public List<Room> DeletedRooms { get; set; } = new List<Room>();
    }
}