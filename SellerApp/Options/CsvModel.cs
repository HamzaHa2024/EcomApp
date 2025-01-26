namespace SellerApp.Options
{
    public record CatalogItemCsvModel(string ID, string Name, string Description, string OwnerID, bool Active, dynamic Xp);

    public class PriceScheduleItemCsvModel
    {
        public required string OwnerID { get; set; }
        public required string ID { get; set; }
        public required string Name { get; set; }
        public bool ApplyTax { get; set; }
        public bool ApplyShipping { get; set; }
        public int MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public bool UseCumulativeQuantity { get; set; }
        public bool RestrictedQuantity { get; set; }
        public DateTime? SaleStart { get; set; }
        public DateTime? SaleEnd { get; set; }
        public bool IsOnSale { get; set; }
        public List<PriceBreak>? PriceBreaks { get; set; }
        public string? Currency { get; set; }
        public object? xp { get; set; }
        public string? ProductID { get; set; }
        public string? BuyerID { get; set; }

        public class PriceBreak
        {
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal? SalePrice { get; set; }
        }
    }
}