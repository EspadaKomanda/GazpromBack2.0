using ImageAgregationService.Database;
using ImageAgregationService.Models;

namespace ImageAgregationService.Repository.KeyWordsRepository
{
    public class KeyWordsRepository(ApplicationContext db) : IKeyWordsRepository
    {
        
        private readonly ApplicationContext _db = db;
        public async Task<bool> CreateKeyWord(KeyWordModel obj)
        {
            await _db.KeyWords.AddAsync(obj);
            return await Save();
        }

        public IQueryable<KeyWordModel> GetUniqueKeyWords()
        {
            return _db.KeyWords.Distinct();
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0;
        }

    }
}