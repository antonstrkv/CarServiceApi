namespace DataAccess
{
    public class Car
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsAvailable { get; set; } 
        public decimal RentalPricePerDay { get; set; }
        public DateTime? RentalStartDate { get; set; } 
        public DateTime? RentalEndDate { get; set; } 
    }
}