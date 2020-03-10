using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Landing.API.Database;
using Landing.API.PublicAPI.Requests;
using Landing.API.PublicAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;

namespace Landing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly ILogger<ContactUsController> _logger;
        private readonly LandingDbContext landingDbContext;

        public ContactUsController(ILogger<ContactUsController> logger, LandingDbContext landingDbContext)
        {
            _logger = logger;
            this.landingDbContext = landingDbContext;
        }

        [HttpPost]
        public async Task<ContactUsResponse> Post(CreateContactUsRequest request)
        {
            var proxyIp = HttpContext.Request.Headers["X-Real-IP"].ToString();
            var senderIp = string.IsNullOrEmpty(proxyIp) ?
                HttpContext.Connection.RemoteIpAddress.ToString()
                :
                proxyIp;
            var createdModel = new Models.ContactUsMessage
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message,
                SendTime = DateTime.UtcNow,
                SenderIp = senderIp
            };
            landingDbContext.ContactUsMessages.Add(createdModel);
            await landingDbContext.SaveChangesAsync();
            return new ContactUsResponse
            {
                Name = createdModel.Name,
                Email = createdModel.Email,
                Message = createdModel.Message
            };
        }
    }
}
