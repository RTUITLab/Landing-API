using System;
using System.Collections.Generic;
using System.Text;

namespace Landing.API.Models
{
    public class ContactUsMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public DateTime SendTime { get; set; }
        public string SenderIp { get; set; }
    }
}
