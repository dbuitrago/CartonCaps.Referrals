using Asp.Versioning;
using CartonCaps.Referrals.Common.Services;
using CartonCaps.Referrals.Contracts.Services;
using CartonCaps.Referrals.Domain.Dtos.V1;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Referrals.ApiMock.Controllers.V1
{
    /// <summary>
    /// Referral endpoints (v1). Supports listing referrals, creating share links,
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
        /// <param name="userId">
        /// User identifier (GUID) coming from the client.
        /// </param>
        /// <remarks>
        /// Client must send <c>X-UserId</c> header.
        /// </remarks>
        /// <response code="200">Returns the referral list.</response>
        /// <response code="404">User id missing/invalid.</response>
        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<ReferralDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ReferralDto>>> List([FromHeader(Name = "X-UserId")] string userId)
        {
            _ = Guid.TryParse(userId ?? Guid.Empty.ToString(), out Guid userIdGuid);
            if (userIdGuid == Guid.Empty) return NotFound();
            return Ok(await _referralService.ListAsync(userIdGuid));
        }

        /// <summary>
        /// Resolves a vendor token after install to determine whether a user was referred.
        /// </summary>
        /// <param name="request">Link creation request (VendorToken + FriendDisplayName).</param>
        /// <remarks>
        /// Used by the app on first launch (post-install) to decide the gating variant and pre-populate referral code.
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
        public async Task<ActionResult<ResolveReferralResponseDto>> Resolve([FromBody] ResolveReferralRequestDto request)
        {
            var result = await _referralResolveService.ResolveAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Creates a referral share link (idempotent) for the given user.
        /// </summary>
        /// <param name="request">Link creation request (channel + referrer code).</param>
        /// <param name="userId">User identifier (GUID) from <c>X-UserId</c> header.</param>
        /// <param name="idempotencyKey">
        /// Idempotency key header. If a request is retried with the same key, the same link is returned.
        /// </param>
        /// <remarks>
        /// Rate limited per user. On retries, send the same <c>Idempotency-Key</c>.
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
        public async Task<ActionResult<CreateReferralLinkResponseDto>> CreateLink(
            [FromBody] CreateReferralLinkRequestDto request, [FromHeader(Name = "X-UserId")] string userId, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {
            _ = Guid.TryParse(userId ?? Guid.Empty.ToString(), out Guid userIdGuid);
            if (userIdGuid == Guid.Empty) return NotFound();

            var result = await _referralLinkService.CreateAsync(userIdGuid, request.ReferrerCode, request.Channel, idempotencyKey);
            return Ok(result);
        }
    }
}
