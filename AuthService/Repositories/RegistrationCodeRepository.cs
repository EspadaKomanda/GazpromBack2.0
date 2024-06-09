using AuthService.Database;
using AuthService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public class RegistrationCodeRepository(ApplicationContext db) : IRegistrationCodeRepository
{
    private readonly ApplicationContext _db = db;

    public async Task<bool> CreateRegistrationCode(RegistrationCode regcode)
    {
        _db.RegistrationCodes.Add(regcode);
        return await Save();
    }

    public async Task<bool> DeleteRegistrationCode(RegistrationCode regcode)
    {
        _db.RegistrationCodes.Remove(regcode);
        return await Save();
    }

    public async Task<RegistrationCode?> GetRegistrationCodeByCode(string regcode)
    {
        return await _db.RegistrationCodes.FirstOrDefaultAsync(rc => rc.Code == regcode);
    }

    public async Task<RegistrationCode?> GetRegistrationCodeByEmail(string email)
    {
        return await _db.RegistrationCodes.FirstOrDefaultAsync(rc => rc.Email == email);
    }

    public async Task<RegistrationCode?> GetRegistrationCodeById(long id)
    {
        return await _db.RegistrationCodes.FirstOrDefaultAsync(rc => rc.Id == id);
    }

    public IQueryable<RegistrationCode> GetRegistrationCodes()
    {
        return _db.RegistrationCodes.AsQueryable();
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() >= 0;
    }

    public async Task<bool> UpdateRegistrationCode(RegistrationCode regcode)
    {
        _db.RegistrationCodes.Update(regcode);
        return await Save();
    }
}