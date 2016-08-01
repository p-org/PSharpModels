//using System;

//namespace Orleans.Collections.Observable
//{
//    internal class ObservableContainerGrain<T> : ContainerGrain<T>, IObservableContainerGrain<T>
//    {
//        internal override IContainerNodeGrain<T> CreateContainerGrain()
//        {
//            return GrainFactory.GetGrain<IObservableContainerNodeGrain<T>>(Guid.NewGuid());
//        }
//    }
//}