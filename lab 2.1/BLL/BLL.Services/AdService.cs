using BulletinBoard.DAL;
using BulletinBoard.DAL.Models;
using BulletinBoard.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Services
{
    public class AdService : IAdService
    {
        private readonly BulletinBoardContext _context;

        public AdService(BulletinBoardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ad>> GetAllAdsAsync()
        {
            return await _context.Ads.Include(a => a.User)
                                     .Include(a => a.Category)
                                     .Include(a => a.Tags)
                                     .ToListAsync();
        }

        public async Task<Ad> GetAdByIdAsync(int id)
        {
            return await _context.Ads.Include(a => a.User)
                                     .Include(a => a.Category)
                                     .Include(a => a.Tags)
                                     .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAdAsync(Ad ad)
        {
            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAdAsync(Ad ad)
        {
            _context.Ads.Update(ad);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdAsync(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad != null)
            {
                _context.Ads.Remove(ad);
                await _context.SaveChangesAsync();
            }
        }
    }
}