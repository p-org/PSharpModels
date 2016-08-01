using System;

namespace Orleans.Collections
{
    [Serializable]
    public class ContainerHostedElement<T>
    {

        public ContainerElementReference<T> Reference { get; set; }
        public T Item { get; set; }

        public ContainerHostedElement(ContainerElementReference<T> reference, T item)
        {
            Reference = reference;
            Item = item;
        }
    }
}