using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Hive.Shared.Common.Mongo
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
    {
        private readonly IMongoDatabase _database;

        public MongoRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public Task<bool> AnyAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).AnyAsync(cancellation);
        }

        public Task<bool> AnyAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).AnyAsync(cancellation);
        }

        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellation)
        {
            return GetCollection().DeleteManyAsync(filter, cancellation);
        }

        public Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellation)
        {
            return GetCollection().DeleteManyAsync(filter, cancellation);
        }

        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellation)
        {
            return GetCollection().DeleteOneAsync(filter, cancellation);
        }

        public Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellation)
        {
            return GetCollection().DeleteOneAsync(filter, cancellation);
        }

        public IMongoCollection<TDocument> GetCollection()
        {
            return _database.GetCollection<TDocument>(typeof(TDocument).Name);
        }

        public Task<TDocument> GetFirstAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).FirstAsync(cancellation);
        }

        public Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).FirstAsync(cancellation);
        }

        public Task<TDocument?> GetFirstOrDefaultAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
#pragma warning disable IDE0001
            return GetCollection().Find(filter, options).ToCursorAsync(cancellation).ContinueWith<TDocument?>(value =>
#pragma warning restore IDE0001
            {
                using var cursor = value.Result;
                var source = cursor.Current;

                if (source == null)
                    return null;

                if (source is IList<TDocument> list)
                {
                    if (list.Count > 0)
                    {
                        return list[0];
                    }
                }
                else
                {
                    using var e = source.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                }

                return null;
            }, cancellation);
        }

        public Task<TDocument?> GetFirstOrDefaultAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            var cursorAsync = GetCollection().Find(filter, options).ToCursorAsync(cancellation);
#pragma warning disable IDE0001
            var continuation = cursorAsync.ContinueWith<TDocument?>(value =>
#pragma warning restore IDE0001
            {
                using var cursor = value.Result;
                var source = cursor.Current;
                if (source == null)
                    return null;

                if (source is IList<TDocument> list)
                {
                    if (list.Count > 0)
                    {
                        return list[0];
                    }
                }
                else
                {
                    using var e = source.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                }

                return null;
            }, cancellation);
            return continuation;
        }

        public Task<List<TDocument>> GetListAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).ToListAsync(cancellation);
        }

        public Task<List<TDocument>> GetListAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).ToListAsync(cancellation);
        }

        public Task<TDocument> GetSingleAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).SingleAsync(cancellation);
        }

        public Task<TDocument> GetSingleAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).SingleAsync(cancellation);
        }

        public Task<TDocument?> GetSingleOrDefaultAsync(FilterDefinition<TDocument> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).ToCursorAsync(cancellation).ContinueWith(cursorTask =>
            {
                using var cursor = cursorTask.Result;
#pragma warning disable IDE0001
                return cursor.MoveNextAsync(cancellation).ContinueWith<TDocument?>(moveNextTask =>
#pragma warning restore IDE0001
                {
                    var source = !moveNextTask.Result ? Enumerable.Empty<TDocument>() : cursorTask.Result.Current;

                    if (source == null)
                        return null;

                    if (source is IList<TDocument> list)
                    {
                        switch (list.Count)
                        {
                            case 0:
                                return default;

                            case 1:
                                return list[0];
                        }
                    }
                    else
                    {
                        using var e = source.GetEnumerator();
                        if (!e.MoveNext())
                        {
                            return default;
                        }

                        var result = e.Current;
                        if (!e.MoveNext())
                        {
                            return result;
                        }
                    }
                    return default;
                }, cancellation);
            }, cancellation).Unwrap();
        }

        public Task<TDocument?> GetSingleOrDefaultAsync(Expression<Func<TDocument, bool>> filter, FindOptions? options, CancellationToken cancellation)
        {
            return GetCollection().Find(filter, options).ToCursorAsync(cancellation).ContinueWith(cursorTask =>
            {
                using var cursor = cursorTask.Result;
#pragma warning disable IDE0001
                return cursor.MoveNextAsync(cancellation).ContinueWith<TDocument?>(moveNextTask =>
#pragma warning restore IDE0001
                {
                    var source = !moveNextTask.Result ? Enumerable.Empty<TDocument>() : cursorTask.Result.Current;

                    if (source == null)
                        return null;

                    if (source is IList<TDocument> list)
                    {
                        switch (list.Count)
                        {
                            case 0:
                                return default;

                            case 1:
                                return list[0];
                        }
                    }
                    else
                    {
                        using var e = source.GetEnumerator();
                        if (!e.MoveNext())
                        {
                            return default;
                        }

                        var result = e.Current;
                        if (!e.MoveNext())
                        {
                            return result;
                        }
                    }
                    return default;
                }, cancellation);
            }, cancellation).Unwrap();
        }

        public Task InsertManyAsync(IEnumerable<TDocument> document, InsertManyOptions? options, CancellationToken cancellation)
        {
            return GetCollection().InsertManyAsync(document, options, cancellation);
        }

        public Task InsertOneAsync(TDocument document, InsertOneOptions? options, CancellationToken cancellation)
        {
            return GetCollection().InsertOneAsync(document, options, cancellation);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions? options, CancellationToken cancellation)
        {
            return GetCollection().ReplaceOneAsync(filter, replacement, options, cancellation);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions? options, CancellationToken cancellation)
        {
            return GetCollection().ReplaceOneAsync(filter, replacement, options, cancellation);
        }

        public Task<UpdateResult> UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options, CancellationToken cancellation)
        {
            return GetCollection().UpdateOneAsync(filter, update, options, cancellation);
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options, CancellationToken cancellation)
        {
            return GetCollection().UpdateOneAsync(filter, update, options, cancellation);
        }
    }
}