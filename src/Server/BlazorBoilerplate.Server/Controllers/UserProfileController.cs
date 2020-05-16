﻿using BlazorBoilerplate.Infrastructure.Server;
using BlazorBoilerplate.Infrastructure.Server.Models;
using BlazorBoilerplate.Localization;
using BlazorBoilerplate.Shared.Dto.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NSwag.Annotations;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorBoilerplate.Server.Controllers
{
    [OpenApiIgnore]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileManager _userProfileManager;
        private readonly IStringLocalizer<Strings> L;

        public UserProfileController(IUserProfileManager userProfileManager, IStringLocalizer<Strings> l)
        {
            _userProfileManager = userProfileManager;
            L = l;
        }

        // GET: api/UserProfile
        [HttpGet("Get")]
        public async Task<ApiResponse> Get()
            => await _userProfileManager.Get();

        // POST: api/UserProfile
        [HttpPost("Upsert")]
        public async Task<ApiResponse> Upsert(UserProfileDto userProfile)
            => ModelState.IsValid ?
                await _userProfileManager.Upsert(userProfile) :
                new ApiResponse(Status400BadRequest, L["InvalidData"]);
    }
}
