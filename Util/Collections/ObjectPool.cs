using ProjectHub.Core;
using System;
using System.Collections.Concurrent;

namespace ProjectHub.Util.Collections
{
    public sealed class ObjectPool<T>
    {
        private readonly ConcurrentBag<T> Objects;
        private readonly Func<T> ObjectGenerator;

        public ObjectPool(Func<T> _ObjectGenerator)
        {
            if (_ObjectGenerator == null)
            {
                Logging.LogError("ObjectGenerator was null");
            }

            Objects = new ConcurrentBag<T>();
            ObjectGenerator = _ObjectGenerator;
        }

        public T GetObject()
        {
            T Item;

            if (Objects.TryTake(out Item))
            {
                return Item;
            }

            return ObjectGenerator();
        }

        public void PutObject(T Item)
        {
            Objects.Add(Item);
        }
    }
}
