using System;
namespace Application.Dtos
{
    public class INDNotificationDTO
    {
        public int Tender_Id { get; set; }
        public string EOINo { get; set; }
        public string EOINoHindi { get; set; }
        public string TenderNumberMoreInfo { get; set; }
        public string Description { get; set; }
        public string DescriptionHindi { get; set; }
        public string EOIDate { get; set; }
        public string EOICloseDate { get; set; }
        public string Updated_Work_Description { get; set; }
        public string TenderClosingTime { get; set; }
        public string UpdatedTenderClosingDate { get; set; }
        public string TenderNoHindi { get; set; }
        public string EMDHindi { get; set; }
        public string TenderClosingTimeHindi { get; set; }
        public DateTime Added_on { get; set; }

    }

    public class DocumentModel
    {
        public string Document_PDF { get; set; }
        public string Document_Name { get; set; }
        public string Document_Name_Hindi { get; set; }
        public string Tender_id { get; set; }
        public string Del_Sts { get; set; }      
    }
}
