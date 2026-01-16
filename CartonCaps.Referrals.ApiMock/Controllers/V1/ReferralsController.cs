using Asp.Versioning;
using CartonCaps.Referrals.Business.Mocks;
using CartonCaps.Referrals.Common.Services;
using CartonCaps.Referrals.Contracts.Services;
using CartonCaps.Referrals.Domain.Dtos.V1;
using CartonCaps.Referrals.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Referrals.ApiMock.Controllers.V1
{
    /// <summary>
    /// Mock Referral endpoints (v1). Supports listing referrals, creating share links,
    /// and resolving vendor tokens after install.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReferralsController(IReferralService referralService, IReferralResolveService referralResolveService, IReferralLinkService referralLinkService) : ControllerBase
    {
        private readonly IReferralService _referralService = referralService;
        public readonly IReferralResolveService _referralResolveService = referralResolveService;
        private readonly IReferralLinkService _referralLinkService = referralLinkService;

        /// <summary>
        /// Returns the current user's referrals ("My Referrals").
        /// </summary>
        /// <remarks>
        /// Internal parameter <c>UserId</c> | <c>Value</c>="11111111-1111-1111-1111-111111111111"
        /// Mock User identifier (GUID) coming from the client.<br/>
        /// Client must send <c>X-UserId</c> header.
        /// </remarks>
        /// <response code="200">Returns the referral list.</response>
        /// <response code="404">User id missing/invalid.</response>
        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<ReferralDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ReferralDto>>> List()
        {
            if (ReferralsApiDataMock.UserId == Guid.Empty) return NotFound();
            return Ok(await _referralService.ListAsync(ReferralsApiDataMock.UserId));
        }

        /// <summary>
        /// Resolves a vendor token after install to determine whether a user was referred. 
        /// </summary>
        /// <remarks>
        /// Used by the app on first launch (post-install) to decide the gating variant and pre-populate referral code.<br/>
        /// <c>request</c> FromBody Json Link creation request (VendorToken + FriendDisplayName).
        /// </remarks>
        /// <response code="200">Resolve result including gate variant and referral code (if any).</response>
        /// <response code="400">Request invalid (e.g., missing vendor token).</response>
        /// <response code="410">Token expired.</response>
        /// <response code="503">Vendor service unavailable.</response>
        [HttpPost("resolve")]
        [ProducesResponseType(typeof(ResolveReferralResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ResolveReferralResponseDto>> Resolve()
        {
            var request = new ResolveReferralRequestDto(ReferralsApiDataMock.VendorToken, "Diego Buitrago");
            var result = await _referralResolveService.ResolveAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Creates a referral share link (idempotent) for the given user.
        /// </summary>
        /// <remarks>
        /// Body request object <c>request</c> value="{ <c>ReferrerCode</c>:"XY7G4D", <c>Channel</c>:"Email" }">Link creation request (channel + referrer code).<br/>
        /// Internal parameter <c>UserId</c> | <c>Value</c>="11111111-1111-1111-1111-111111111111"<br/>
        /// Mock User identifier (GUID) coming from the client.<br/>
        /// Internal parameter name="idempotencyKey" value="3e33e682fe014c0090cf0d7305994a76" Idempotency key header. If a request is retried with the same key, the same link is returned.<br/>
        /// User Id must send <c>X-UserId</c> header and<br/>
        /// the rate limited per user. On retries, send the same <c>Idempotency-Key</c>.
        /// </remarks>
        /// <response code="200">Referral link created (or returned from idempotency cache).</response>
        /// <response code="400">Invalid input (missing headers, invalid channel, etc.).</response>
        /// <response code="404">User id missing/invalid.</response>
        /// <response code="409">Conflict (e.g., duplicate idempotency key collision).</response>
        /// <response code="429">Rate limit exceeded.</response>
        /// <response code="503">Vendor service unavailable.</response>
        [HttpPost("link")]
        [ProducesResponseType(typeof(CreateReferralLinkResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CreateReferralLinkResponseDto>> CreateLink()
        {
            var result = await _referralLinkService.CreateAsync(ReferralsApiDataMock.UserId, ReferralsApiDataMock.ReferrerCode, ChannelEnum.Email.ToString(), ReferralsApiDataMock.IdempotencyKey);
            return Ok(result);
        }
    }
}
