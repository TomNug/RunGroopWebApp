﻿using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;
        public ClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public bool Add(Club club)
        {
            _context.Add(club);
            return Save();
        }

        public bool Delete(Club club)
        {
            _context.Remove(club);
            return Save();
        }

        public async Task<IEnumerable<Club>> GetAllAsync()
        {
            return await _context.Clubs.ToListAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            // Entity doesn't explore navigation properties by default
            //      lazy loading
            //      Include forces it to explore the JOINs
            //      Finds the Addressnreferred to in the Club
            return await _context.Clubs.Include(a => a.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Club> GetByIdNoTrackingAsync(int id)
        {
            return await _context.Clubs.Include(a => a.Address).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Club>> GetAllClubsByCityAsync(string city)
        {
            // Include forces Entity to evaluate the nagivation property
            return await _context.Clubs.Include(c => c.Address).Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Club club)
        {
            _context.Update(club);
            return Save();
        }

        public async Task<IEnumerable<Club>> GetNClubsByCityExludingIdAsync(string city, int n, int id)
        {
            return await _context.Clubs.Where(c => c.Address.City.Contains(city) && c.Id != id).Take(n).ToListAsync();
        }
    }
}
