namespace BackEnd.Dtos
{
    public class JobForUpdateDto
    {
        public string OrderQuantity { get; set; }
        public string PossibleQuantity { get; set; }
        public string RemainingQuantity { get; set; }
        public string WeightQuantity { get; set; }
        public string WeightLength { get; set; }
        public string WeightRecieved { get; set; }
        public string Oal { get; set; }
        public string CutOff { get; set; }
        public string MainFacing { get; set; }
        public string SubFacing { get; set; }
        public string HeatLot { get; set; }
        public string Bars { get; set; }
        public string ScrapCount { get; set; }
        public string MonthReq { get; set; }
        public string DeliveryDate { get; set; }
        
    }
}