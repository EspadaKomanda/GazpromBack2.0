using BackGazprom.Database;
using BackGazprom.Database.Models;

namespace BackGazprom.Repositories;

public interface IRegistrationCodeRepository
{
    public IQueryable<RegistrationCode> GetRegistrationCodes();
    public Task<RegistrationCode?> GetRegistrationCodeById(long id);
    public Task<RegistrationCode?> GetRegistrationCodeByCode(string regcode);
    public Task<RegistrationCode?> GetRegistrationCodeByEmail(string email);
    public Task<bool> CreateRegistrationCode(RegistrationCode regcode);
    public Task<bool> UpdateRegistrationCode(RegistrationCode regcode);
    public Task<bool> DeleteRegistrationCode(RegistrationCode regcode);
    public Task<bool> Save();
}