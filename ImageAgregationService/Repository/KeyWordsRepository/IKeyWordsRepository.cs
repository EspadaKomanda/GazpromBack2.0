using ImageAgregationService.Models;

namespace ImageAgregationService.Repository.KeyWordsRepository
{
    public interface IKeyWordsRepository
    {
        public Task<bool> CreateKeyWord(KeyWordModel obj);
        public IQueryable<KeyWordModel> GetUniqueKeyWords();
    }
}