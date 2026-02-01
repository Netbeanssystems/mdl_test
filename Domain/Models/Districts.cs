namespace Domain.Models
{
    public class Districts : BaseModel
    {
        public int Id { get; set; }
        public int StateId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public virtual States State { get; set; }
        //[InverseProperty("Per_District")]
        //public virtual List<EmployeeDetails> Per_DistrictId { get; set; }
        //[InverseProperty("Pre_District")]
        //public virtual List<EmployeeDetails> Pre_DistrictId { get; set; }
        //public virtual List<ITAssets> Assets { get; set; }
        //public virtual List<Constituencies> Constituencies { get; set; }
        //public virtual List<SdcClassRooms> SdcClassRooms { get; set; }
        //public virtual List<SkillParks> SkillParks { get; set; }
    }
}
