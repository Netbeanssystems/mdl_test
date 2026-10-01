namespace Application.Dtos
{
    public class CommonRemarksUpdaterDTO
    {
        public int Id { get; set; }
        public int Flag { get; set; }
        public string Status { get; set; }
        public int? StatusId { get; set; }
        public string Remarks { get; set; }
        //Added for Action taken By
        public string UserName { get; set; }
        //Added for another entity id
        public int Flag1 { get; set; }
        //Added for Name
        public string Param1 { get; set; }
        //Added for Email
        public string Param2 { get; set; }
        //Added for MobileNo
        public string Param3 { get; set; }
        public decimal Param4 { get; set; }
    }
}
