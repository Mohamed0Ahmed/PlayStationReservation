namespace System.Shared.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int Points { get; set; }
        public int StoreId { get; set; }
    }
}
