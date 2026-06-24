using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IGenericService<TSaveViewModel, TViewModel>
    where TSaveViewModel : class
    where TViewModel : class
{
    Task<Result<IEnumerable<TViewModel>>> GetAllAsync();
    Task<Result<TViewModel>> GetByIdAsync(int id);
    Task<Result<TViewModel>> AddAsync(TSaveViewModel vm);
    Task<Result> UpdateAsync(TSaveViewModel vm, int id);
    Task<Result> DeleteAsync(int id);
}