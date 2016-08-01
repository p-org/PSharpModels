using System;

namespace Orleans.Collections
{
    [Serializable]
    public class ContainerElementReference<T>
    {
        private readonly IElementExecutor<T> _executorGrainReference;

        [NonSerialized]
        private readonly IElementExecutor<T> _executorReference;

        public Guid ContainerId { get; private set; }
        public int Offset { get; private set; }

        public IElementExecutor<T> Executor => _executorReference ?? _executorGrainReference;
        public bool Exists { get; }


        public ContainerElementReference(Guid containerId, int offset, IElementExecutor<T> executorReference,
            IElementExecutor<T> executorGrainReference, bool exists = true)
        {
            ContainerId = containerId;
            Offset = offset;
            _executorReference = executorReference;
            _executorGrainReference = executorGrainReference;
            Exists = exists;
        }
    }
}