using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class WebsiteCounterDTO
    {
        public int Id { get; set; }
        public long NoofCount { get; set; }

        //For Last Updated Date
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? LastUpdate { get; set; }
    }
}
