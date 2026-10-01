using System;
using System.Collections.Generic;

namespace Application.Dtos
{
    public class SBMPNotificationDTO
    {
        public int Tender_Id { get; set; }
        public string TenderNo { get; set; }

        public string Description { get; set; }
        public string DescriptionHindi { get; set; }
        public string TenderDate { get; set; }
        public string TenderCloseDate { get; set; }
        public string TenderFee { get; set; }
        public string EMD { get; set; }

        public string TenderNumberMoreInfo { get; set; }
        public string Updated_Work_Description { get; set; }
        public string Document_PDF { get; set; }
        public string Document_Name { get; set; }
        public string TenderClosingTime { get; set; }
        public string TenderNoHindi { get; set; }
        public string EMDHindi { get; set; }
        public string TenderClosingTimeHindi { get; set; }
        public string UpdatedTenderClosingDate { get; set; }
        public string LinkURL { get; set; }
        public string LinkName { get; set; }
        public DateTime Added_on { get; set; }
        public List<DocumentModel> docs { get; set; }
    }

    public class NotificationDTO
    {
        public List<SBMPNotificationDTO> not { get; set; }

        public List<DocumentModel> docs { get; set; }
        public List<INDNotificationDTO> ind { get; set; }
    }

}
