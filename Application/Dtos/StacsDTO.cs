using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class StacsDTO
    {
        public string DocumentPDF { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameHindi { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
    }
    public class FormatDTO
    {
        public string DocumentPDF { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameHindi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
    }
    public class OthersDTO
    {
        public string DocumentPDF { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameHindi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
    }

    public class CommanStacDTO
    {
        public List<StacsDTO> stacs { get; set; }
        public List<FormatDTO> formats { get; set; }
        public List<OthersDTO> others { get; set; }
    }
}
