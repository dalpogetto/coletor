using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using System.Linq;


namespace CollectorQi.Resources.DataBaseHelper
{

    public class BaseOperations
    {
        public SQLiteAsyncConnection Connection;
        private static readonly AsyncLock AsyncLock = new AsyncLock();

        public BaseOperations()
        {
            var t = Task.Run (async () =>  await ConnectionLoop());

            t.Wait();
        }

        public async Task<int> ConnectionLoop()
        {
            try
            {
              Connection = DependencyService.Get<ISQLite>().GetAsyncConnection();
              return 1;              
            }
            catch (SQLiteException sqliteException)
            {
               return await ConnectionLoop();
            }
        }

        public async Task<int> InsertAsync<T>(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) await Connection.InsertAsync(entity);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await InsertAsync(entity);
                }
                throw;
            }

        }

        public async Task<int> InsertAllAsync<T>(List<T> entityList)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (Connection != null) await Connection.InsertAllAsync(entityList);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await InsertAsync(entityList);
                }
                throw;
            }
        }

        public async Task<int> InsertOrReplaceAsync<T>(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (Connection != null && entity != null) await Connection.InsertOrReplaceAsync(entity);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await InsertOrReplaceAsync(entity);
                }
                throw;
            }
        }


        public async Task<int> DeleteAllAsync<T>()
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (Connection != null) await Connection.DeleteAllAsync<T>();
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await DeleteAllAsync<T>();
                }
                throw;
            }
        }

        public async Task<int> DeleteAsync<T>(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) await Connection.DeleteAsync(entity);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await DeleteAsync(entity);
                }
                throw;
            }

        }


        public async Task<int> UpdateAllAsync<T>(List<T> entityList)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (Connection != null) await Connection.UpdateAllAsync(entityList);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await UpdateAsync(entityList);
                }
                throw;
            }
        }

        public async Task<int> UpdateAsync<T>(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) await Connection.UpdateAsync(entity);
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await UpdateAsync(entity);
                }
                throw;
            }

        }

        public async Task<List<object>> QueryAsync(TableMapping map, string query, params object[] args)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (query != null) 
                        return await Connection.QueryAsync(map, query, args);

                }
                return null;
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await QueryAsync(map, query, args);
                }
                throw;
            }
        }


        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (query != null)
                    {
                        if (args == null)
                        {
                            return await Connection.QueryAsync<T>(query);
                        }
                        else
                        {
                            return await Connection.QueryAsync<T>(query, args);
                        }
                    }

                }
                return null;
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    if (args == null)
                    {
                        return await Connection.QueryAsync<T>(query);
                    }
                    else
                    {
                        return await Connection.QueryAsync<T>(query, args);
                    }
                }
                throw;
            }
        }




        /*
        public async Task<int> Table<T>() where T : new()
        { 
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    await Connection.
                    return 1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                if (sqliteException.Result == SQLite3.Result.Busy ||
                    sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await QueryAsync(map, query, args);
                }
                throw;
            }
        } */
    }

    public class AsyncLock
    {
        private readonly AsyncSemaphore m_semaphore;
        private readonly Task<Releaser> m_releaser;

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
            m_releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                m_releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }

            public void Dispose()
            {
                if (m_toRelease != null)
                    m_toRelease.m_semaphore.Release();
            }
        }
    }

    public class AsyncSemaphore
    {
        private readonly static Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        private int m_currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            m_currentCount = initialCount;
        }

        /// <summary>  
        /// To insert DB Lock  
        /// </summary>  
        /// <returns></returns>  
        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;
                    return s_completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    m_waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        /// <summary>  
        /// To Release DB Lock  
        /// </summary>  
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++m_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}  
