using System.Collections;
using System.Collections.Generic;

namespace SignGen.Workers
{
    public class BlockingQueueWrapper<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private readonly object _locker = new object();

        public BlockingQueueWrapper()
        {
            _queue = new Queue<T>();
        }

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


        public IEnumerator<T> GetEnumerator()
        {
            lock (_locker)
            {
                return _queue.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

