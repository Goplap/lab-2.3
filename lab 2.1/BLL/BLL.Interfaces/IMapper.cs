namespace BulletinBoard.BLL.Interfaces
{
    public interface IMapper<TEntity, TModel>
    {
        TModel Map(TEntity entity);
        TEntity Map(TModel model);
        IEnumerable<TModel> Map(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Map(IEnumerable<TModel> models);
    }
}