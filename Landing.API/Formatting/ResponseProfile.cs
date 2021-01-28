using AutoMapper;
using Landing.API.Models;
using Landing.API.PublicAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Formatting
{
    public class ResponseProfile : Profile
    {
        public ResponseProfile()
        {
            CreateMap<SourceCodeLink, SourceCodeLinkResponse>();
            CreateMap<ProjectInfo, ProjectInfoResponse>();
        }
    }
}
