using System;
using System.Threading;

namespace Alpha.Toolkit
{
    public class DataLock
    {
        readonly ReaderWriterLockSlim _dataLockBase = new ReaderWriterLockSlim();
        readonly ReaderWriterLockSlim _dataLockImmediate = new ReaderWriterLockSlim();

        public void UiRead(Action action)
        {
            _dataLockBase.EnterReadLock();
            action();
            _dataLockBase.ExitReadLock();
        }

        public void AiRead(Action action)
        {
            _dataLockImmediate.EnterReadLock();
            _dataLockBase.EnterReadLock();
            action();
            _dataLockBase.ExitReadLock();
            _dataLockImmediate.ExitReadLock();
        }

        public T AiRead<T>(Func<T> action)
        {
            _dataLockImmediate.EnterReadLock();
            _dataLockBase.EnterReadLock();
            T result = action();
            _dataLockBase.ExitReadLock();
            _dataLockImmediate.ExitReadLock();
            return result;
        }

        public void Write(Action action)
        {
            _dataLockBase.EnterWriteLock();
            action();
            _dataLockBase.ExitWriteLock();
            if(_dataLockBase.WaitingReadCount > 0)
                Thread.Sleep(1);
        }
        public T Write<T>(Func<T> action)
        {
            _dataLockBase.EnterWriteLock();
            T result = action();
            _dataLockBase.ExitWriteLock();
            return result;
        }

        public void ImmediateWrite(Action action)
        {
            _dataLockImmediate.EnterWriteLock();
            _dataLockBase.EnterWriteLock();
            action();
            _dataLockBase.ExitWriteLock();
            _dataLockImmediate.ExitWriteLock();
        }
    }
}
