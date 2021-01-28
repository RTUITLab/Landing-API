using System;
using System.Collections.Generic;
using System.Text;

namespace Landing.API.PublicAPI.Responses
{
    public class ProjectInfoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Images { get; set; }
        public string[] Videos { get; set; }
        public string Date { get; set; }
        public string[] Tags { get; set; }
        public string[] Tech { get; set; }
        public string[] Developers { get; set; }
        public string Site { get; set; }
        public SourceCodeLinkResponse[] SourceCode { get; set; }
    }
    public class SourceCodeLinkResponse
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
