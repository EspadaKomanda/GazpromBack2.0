using ImageAgregationService.Models;

namespace ImageAgregationService.Repository.MarkRepository
{
    public interface IMarkRepository
    {
        public IQueryable<MarkModel> GetMarks();
        public Task<bool> CreateMark(MarkModel obj);
        public Task<bool> UpdateMark(MarkModel obj);
        public Task<bool> DeleteMark(MarkModel obj);
        public Task<bool> Save();
    }
}