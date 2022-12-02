using System;

namespace ISD.ViewModels
{
    public class SerialSearchResultViewModel
    {
        public string ProductCode { get; set; }
        public string Serial { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public decimal? Specifications_Length { get; set; }
        public decimal? Specifications_Width { get; set; }
        public decimal? Specifications_Height { get; set; }
        public decimal? Specifications_Overalls { get; set; }
        public decimal? Specifications_Side { get; set; }
        public string PrintMoldFilm { get; set; }
        public string PrintMoldName { get; set; }
        public string MoldStorage { get; set; }
        public string Bin { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string StamQty { get; set; }
        public string Description { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public int? NumberMaintenanceDateAlert { get; set; }
    }
}
