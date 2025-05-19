// AdsController.cs
using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdsController(IAdService adService)
        {
            _adService = adService;
        }

        // GET: api/Ads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdDto>>> GetAds()
        {
            return Ok(await _adService.GetAllAdsAsync());
        }

        // GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdDto>> GetAd(int id)
        {
            var ad = await _adService.GetAdByIdAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            return ad;
        }

        // GET: api/Ads/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AdDto>>> GetAdsByUser(int userId)
        {
            return Ok(await _adService.GetAdsByUserIdAsync(userId));
        }

        // GET: api/Ads/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<AdDto>>> GetAdsByCategory(int categoryId)
        {
            return Ok(await _adService.GetAdsByCategoryIdAsync(categoryId));
        }

        // POST: api/Ads
        [HttpPost]
        public async Task<ActionResult<AdDto>> PostAd(AdDto ad)
        {
            var createdAd = await _adService.CreateAdAsync(ad);

            return CreatedAtAction(
                nameof(GetAd),
                new { id = createdAd.Id },
                createdAd
            );
        }

        // PUT: api/Ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, AdDto ad)
        {
            if (id != ad.Id)
            {
                return BadRequest();
            }

            await _adService.UpdateAdAsync(ad);
            return NoContent();
        }

        // DELETE: api/Ads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _adService.GetAdByIdAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            await _adService.DeleteAdAsync(id);
            return NoContent();
        }
    }
}
