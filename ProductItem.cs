namespace CheckDataSystem
{
    public class ProductItem
    {
        public string Barcode { get; set; }
        public string Status { get; set; }

        public ProductItem(string barcode, string status)
        {
            Barcode = barcode;
            Status = status;
        }
    }
}
