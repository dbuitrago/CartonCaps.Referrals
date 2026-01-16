using CartonCaps.Referrals.Business.Services;
using CartonCaps.Referrals.Common.Models;
using CartonCaps.Referrals.Common.Vendor;
using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Domain.Dtos.V1;
using CartonCaps.Referrals.Domain.Entities;
using CartonCaps.Referrals.Domain.Enums;
using Moq;

namespace CartonCaps.Referrals.Tests.Business.Services
{
    [TestFixture]
    public class ReferralLinkServiceTests
    {
        private Mock<IReferralLinkRepository> _referralLinkRepository = null!;
        private Mock<IReferralRepository> _referralRepository = null!;
        private Mock<ILinkVendorClient> _vendor = null!;
        private ReferralLinkService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _referralLinkRepository = new Mock<IReferralLinkRepository>(MockBehavior.Strict);
            _referralRepository = new Mock<IReferralRepository>(MockBehavior.Strict);
            _vendor = new Mock<ILinkVendorClient>(MockBehavior.Strict);

            _sut = new ReferralLinkService(_referralLinkRepository.Object, _referralRepository.Object, _vendor.Object);
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CreateAsync_WhenIdempotencyKeyMissing_ThrowsMissingIdempotencyException(string? idempotencyKey)
        {
            var userId = Guid.NewGuid();

            Assert.ThrowsAsync<MissingIdempotencyException>(async () =>
                await _sut.CreateAsync(userId, referrerCode: "ABC123", channel: "Default", idempotencyKey: idempotencyKey!));
        }

        [Test]
        public async Task CreateAsync_WhenIdempotencyKeyAlreadyExists_ReturnsExistingAndDoesNotCallVendorOrAdd()
        {
            var userId = Guid.NewGuid();
            var existing = new ReferralLinkEntity
            {
                Id = Guid.NewGuid(),
                ReferrerUserId = userId,
                ReferrerCode = "ABC123",
                Channel = ChannelEnum.Email,
                IdempotencyKey = "idempo",
                VendorToken = "vendor-token",
                ReferralLinkUrl = "https://example.com/ref",
                ExpiresUtc = DateTime.UtcNow.AddDays(7),
                Status = ReferralLinkStatusEnum.Active,
                CreatedUtc = DateTime.UtcNow.AddMinutes(-10)
            };

            _referralLinkRepository
                .Setup(r => r.FindByIdempotencyAsync(userId, "idempo"))
                .ReturnsAsync(existing);

            var result = await _sut.CreateAsync(userId, "ABC123", "Email", "idempo");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ReferralLinkUrl, Is.EqualTo(existing.ReferralLinkUrl));
                Assert.That(result.ExpiresUtc, Is.EqualTo(existing.ExpiresUtc));
            });
            _referralLinkRepository.Verify(r => r.FindByIdempotencyAsync(userId, "idempo"), Times.Once);
        }

        [Test]
        public void CreateAsync_WhenRateLimitExceeded_ThrowsRateLimitExceededException()
        {
            var userId = Guid.NewGuid();

            _referralLinkRepository
                .Setup(r => r.FindByIdempotencyAsync(userId, It.IsAny<string>()))
                .ReturnsAsync((ReferralLinkEntity?)null);

            _referralLinkRepository
                .Setup(r => r.CountCreatedSinceAsync(userId, It.IsAny<DateTime>()))
                .ReturnsAsync(20); 
            Assert.ThrowsAsync<RateLimitExceededException>(async () =>
                await _sut.CreateAsync(userId, "ABC123", "Email", "idempo"));

            _referralLinkRepository.Verify(r => r.FindByIdempotencyAsync(userId, It.IsAny<string>()), Times.Once);
            _referralLinkRepository.Verify(r => r.CountCreatedSinceAsync(userId, It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void CreateAsync_WhenChannelInvalid_ThrowsInvalidOperationException()
        {
            var userId = Guid.NewGuid();

            _referralLinkRepository
                .Setup(r => r.FindByIdempotencyAsync(userId, It.IsAny<string>()))
                .ReturnsAsync((ReferralLinkEntity?)null);

            _referralLinkRepository
                .Setup(r => r.CountCreatedSinceAsync(userId, It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _sut.CreateAsync(userId, "ABC123", "NOT_A_REAL_CHANNEL", "idempo"));

            Assert.That(ex!.Message, Is.EqualTo("Invalid channel."));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ResolveAsync_WhenVendorTokenMissing_ThrowsInvalidOperationException(string? vendorToken)
        {
            var request = new ResolveReferralRequestDto
            (
                VendorToken : vendorToken!,
                FriendDisplayName : "Friend"
            );

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.ResolveAsync(request));
            Assert.That(ex!.Message, Is.EqualTo("VendorToken is required."));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ResolveAsync_WhenFriendDisplayNameMissing_ThrowsInvalidOperationException(string? friendDisplayName)
        {
            var request = new ResolveReferralRequestDto
            (
                VendorToken : "token",
                FriendDisplayName : friendDisplayName!
            );

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.ResolveAsync(request));
            Assert.That(ex!.Message, Is.EqualTo("Friend DisplayName is required."));
        }
    }
}
