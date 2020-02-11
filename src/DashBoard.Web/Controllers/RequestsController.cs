﻿using System.Threading.Tasks;
using DashBoard.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;
        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet("getportal/{id}")]
        public async Task<ActionResult<object>> GetPortal(int id)
        {
            var response = await _requestService.GetDomainByUrl(id);
            if (response == null)
            {
                return NotFound(new { message = $"Problem reaching portal with id {id}" });
            }
            return response;
        }

        [HttpGet("getservice/{id}")]
        public async Task<ActionResult<object>> GetService(int id)
        {
            var response = await _requestService.GetService(id);
            if (response == null)
            {
                return NotFound(new { message = $"Problem reaching portal with id {id}" });
            }
            return response;
        }
    }
}