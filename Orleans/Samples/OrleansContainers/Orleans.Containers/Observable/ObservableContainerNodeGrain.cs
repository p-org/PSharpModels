//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Orleans.Collections.Observable
//{
//    public class ObservableContainerNodeGrain<T> : ContainerNodeGrain<T>, IObservableContainerNodeGrain<T>
//    {
//        public override async Task OnActivateAsync()
//        {
//            await base.OnActivateAsync();
//        }

//        protected override async Task<IReadOnlyCollection<ContainerElementReference<T>>> InternalAddItems(IEnumerable<T> batch)
//        {
//            List<ContainerHostedElement<T>> addArgs;
//            lock (Collection)
//            {
//                var oldCount = Collection.Count;
//                foreach (var item in batch)
//                {
//                    Collection.Add(item);
//                }
//                addArgs = Enumerable.Range(oldCount, Collection.Count - oldCount).Select(i => new ContainerHostedElement<T>(GetReferenceForItem(i, true), Collection[i])).ToList();
//            }
//            await StreamProvider.SendItems(addArgs, false);
//            return addArgs.Select(x => x.Reference).ToList() as IReadOnlyCollection<ContainerElementReference<T>>;
//        }

//        protected override async Task<bool> InternalRemove(T item)
//        {
//            var index = Collection.IndexOf(item);
//            if (!Collection.Remove(item))
//            {
//                return false;
//            }

//            var removeArgs = new List<ContainerHostedElement<T>>(1) { new ContainerHostedElement<T>(GetReferenceForItem(index, false), item) };
//            await StreamProvider.SendItems(removeArgs);

//            return true;
//        }

//        protected override async Task<bool> InternalRemove(ContainerElementReference<T> reference)
//        {
//            if (Collection.Count < reference.Offset)
//            {
//                return false;
//            }
//            var item = Collection[reference.Offset];
//            Collection.RemoveAt(reference.Offset);

//            var removeArgs = new List<ContainerHostedElement<T>>(1) { new ContainerHostedElement<T>(GetReferenceForItem(reference.Offset, false), item)};
//            await StreamProvider.SendItems(removeArgs);

//            return true;
//        }

//        private void Foo()
//        {
//            var t = new Task(async () => await HandleCollectionChange(null, null));
//            t.RunSynchronously();
//        }

//        private async Task HandleCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            switch(e.Action)
//            {
//                case NotifyCollectionChangedAction.Add:
//                    int newItemsCount = e.NewItems.Count;
//                    var argsAdd = Enumerable.Range(e.NewStartingIndex, newItemsCount).Select(i => new ContainerHostedElement<T>(GetReferenceForItem(i, true), Collection[i])).ToList();
//                    await StreamProvider.SendItems(argsAdd);
//                break;
//                case NotifyCollectionChangedAction.Remove:
//                    int oldItemsCount = e.OldItems.Count;
//                    var argsDel = Enumerable.Range(e.OldStartingIndex, oldItemsCount).Select(i => new ContainerHostedElement<T>(GetReferenceForItem(i, false), Collection[i])).ToList();
//                    await StreamProvider.SendItems(argsDel);
//                break;
//                case NotifyCollectionChangedAction.Reset:
//                    throw new NotImplementedException();
//                break;
//            }
//        }
//    }
//}