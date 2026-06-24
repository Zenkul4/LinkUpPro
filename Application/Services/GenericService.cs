using AutoMapper;
using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Services;

public abstract class GenericService<TEntity, TSaveViewModel, TViewModel> : IGenericService<TSaveViewModel, TViewModel>
    where TEntity : class
    where TSaveViewModel : class
    where TViewModel : class
{
    protected readonly IGenericRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    protected GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<Result<IEnumerable<TViewModel>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        var viewModels = _mapper.Map<IEnumerable<TViewModel>>(entities);
        return Result<IEnumerable<TViewModel>>.Success(viewModels);
    }

    public virtual async Task<Result<TViewModel>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result<TViewModel>.Failure("No encontrado");

        var viewModel = _mapper.Map<TViewModel>(entity);
        return Result<TViewModel>.Success(viewModel);
    }

    public virtual async Task<Result<TViewModel>> AddAsync(TSaveViewModel vm)
    {
        var entity = _mapper.Map<TEntity>(vm);
        var createdEntity = await _repository.AddAsync(entity);
        var viewModel = _mapper.Map<TViewModel>(createdEntity);
        return Result<TViewModel>.Success(viewModel);
    }

    public virtual async Task<Result> UpdateAsync(TSaveViewModel vm, int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Failure("No encontrado");

        _mapper.Map(vm, entity);
        await _repository.UpdateAsync(entity);
        return Result.Success();
    }

    public virtual async Task<Result> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Failure("No encontrado");

        await _repository.DeleteAsync(entity);
        return Result.Success();
    }
}