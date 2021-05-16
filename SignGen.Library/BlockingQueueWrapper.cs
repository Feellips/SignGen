using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SignGen.Library
{
    public class BlockingQueueWrapper<T> : IEnumerable
    {
        #region Fields

        private Queue<T> queue;
        private object locker;

        #endregion

        #region Constructors

        public BlockingQueueWrapper() : this(new Queue<T>())
        {
        }

        public BlockingQueueWrapper(int initialCapacity) : this(new Queue<T>(initialCapacity))
        {
        }

        public BlockingQueueWrapper(IEnumerable<T> collection)
        {
            queue = new Queue<T>(collection);
            locker = new object();
        }

        #endregion

        #region Public

        public void Enqueue(T item)
        {
            lock (locker)
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T item;

            lock (locker)
            {
                item = queue.Dequeue();
            }

            return item;
        }

        public int Count
        {
            get
            {
                lock (locker)
                {
                    return queue.Count;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (locker)
            {
                return queue.GetEnumerator();
            }
        }

        #endregion

    }
}
