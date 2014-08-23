using System;
using System.Threading;

namespace Alpha.Toolkit
{
    public class DataLock
    {
        readonly ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim();

        public T Read<T>(Func<T> action)
        {
            _dataLock.EnterReadLock();
            T result = action();
            _dataLock.ExitReadLock();
            return result;
        }

        public void Read(Action action)
        {
            _dataLock.EnterReadLock();
            action();
            _dataLock.ExitReadLock();
        }

        public void Write(Action action)
        {
            _dataLock.EnterWriteLock();
            action();
            _dataLock.ExitWriteLock();
            if(_dataLock.WaitingReadCount > 0)
                Thread.Sleep(1);
        }
        public T Write<T>(Func<T> action)
        {
            _dataLock.EnterWriteLock();
            T result = action();
            _dataLock.ExitWriteLock();
            return result;
        }
    }
}
