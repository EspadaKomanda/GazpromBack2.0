using ImageAgregationService.Database;
using ImageAgregationService.Models;

namespace ImageAgregationService.Repository.MarkRepository
{
    public class MarkRepository(ApplicationContext db) : IMarkRepository
    {
        
        private readonly ApplicationContext _db = db;
        public async Task<bool> CreateMark(MarkModel obj)
        {
            await _db.Marks.AddAsync(obj);
            return await Save();
        }

        public async Task<bool> DeleteMark(MarkModel obj)
         {
            _db.Marks.Remove(obj);
            return await Save();
        }

        public IQueryable<MarkModel> GetMarks()
        {
            return _db.Marks;
        }
        public Task<bool> UpdateMark(MarkModel obj)
        {
            _db.Marks.Update(obj);
            return Save();
        }
        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0;
        }
    }
}