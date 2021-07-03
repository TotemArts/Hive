using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Hive.Shared.Common.Mongo
{
    public interface IMongoRepository<TDocument> where TDocument : class
    {
        Task<bool> AnyAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<bool> AnyAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellation = default);

        Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellation = default);

        Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellation = default);

        Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellation = default);

        IMongoCollection<TDocument> GetCollection();

        Task<TDocument> GetFirstAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument?> GetFirstOrDefaultAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument?> GetFirstOrDefaultAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<List<TDocument>> GetListAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<List<TDocument>> GetListAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument> GetSingleAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument> GetSingleAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument?> GetSingleOrDefaultAsync(FilterDefinition<TDocument> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task<TDocument?> GetSingleOrDefaultAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options = null, CancellationToken cancellation = default);

        Task InsertManyAsync(IEnumerable<TDocument> document, InsertManyOptions? options = null, CancellationToken cancellation = default);

        Task InsertOneAsync(TDocument document, InsertOneOptions? options = null, CancellationToken cancellation = default);

        Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions? options = null, CancellationToken cancellation = default);

        Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions? options = null, CancellationToken cancellation = default);

        Task<UpdateResult> UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null, CancellationToken cancellation = default);

        Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null, CancellationToken cancellation = default);
    }
}