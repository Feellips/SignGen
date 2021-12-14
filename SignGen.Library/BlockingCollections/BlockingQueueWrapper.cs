using System.Collections;
using System.Collections.Generic;

namespace SignGen.Library.BlockingCollections
{
    public class BlockingQueueWrapper<T> : IEnumerable
    {
        #region Fields

        private readonly Queue<T> _queue;
        private readonly object _locker = new object();


        #endregion

        #region Constructors

        public BlockingQueueWrapper() : this(new Queue<T>())
        {
        }

        public BlockingQueueWrapper(IEnumerable<T> collection)
        {
            _queue = new Queue<T>(collection);
        }

        #endregion

        #region Public

        public void Enqueue(T item)
        {
            lock (_locker)
            {
                _queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T item;

            lock (_locker)
            {
                item = _queue.Dequeue();
            }

            return item;
        }

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _queue.Count;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (_locker)
            {
                return _queue.GetEnumerator();
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                _queue.Clear();
            }
        }

        #endregion
    }
}

