using System;
using System.Threading.Tasks;

namespace ActorModel
{
    public class DummyTask : Task
    {
        public DummyTask()
            : base(() => { })
        {

        }

        public static DummyTask<TResult> FromResult<TResult>(Func<TResult> function) 
        {
            return new DummyTask<TResult>(function);
        }
    }

    public class DummyTask<T> : Task<T>
    {
        private new T Result;

        public DummyTask()
            : base(() => { return default(T); })
        {

        }

        public DummyTask(Func<T> function)
            : base(function)
        {
            this.Result = function();
        }
    }
}
