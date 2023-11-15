using ScienceAtrium.Domain.RootAggregate.Interfaces;
using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate;
public interface IEntityValidation
{
    /// <summary>
    /// check entity on default value
    /// </summary>
    /// <returns></returns>
    bool IsEmpty();

    /// <summary>
    /// check entity on existing in the database
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    bool IsExist<TReaderEntity>(IReader<TReaderEntity> reader, Expression<Func<TReaderEntity, bool>> func)
        where TReaderEntity : Entity;

    /// <summary>
    /// combine all checks in one
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    bool IsValid<TReaderEntity>(IReader<TReaderEntity> reader)
        where TReaderEntity : Entity;

    /// <summary>
    /// combine all checks in one
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    bool IsValid<TReaderEntity>(IReader<TReaderEntity> reader, Expression<Func<TReaderEntity, bool>> func) 
        where TReaderEntity : Entity;
}
