using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams.Messages;
using Orleans.Streams;

namespace Orleans.Streams.Endpoints
{
    /// <summary>
    /// Consumes items of a single stream.
    /// </summary>
    /// <typeparam name="TIn">Type of items to consume.</typeparam>
    public class SingleStreamConsumer<TIn> : ITransactionalStreamConsumer<TIn>
    {
        private readonly Dictionary<int, TaskCompletionSource<Task>> _awaitedTransactions;
        private readonly IStreamProvider _streamProvider;
        private readonly Func<Task> _tearDownFunc;
        private StreamSubscriptionHandle<IStreamMessage> _messageStreamSubscriptionHandle;
        private bool _tearDownExecuted;
        private readonly IStreamMessageVisitor<TIn> _streamMessageVisitor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamProvider">Stream provider to be used.</.param>
        /// <param name="streamMessageVisitor">Owning consumer to call on message arrival.</param>
        /// <param name="tearDownFunc">Asynchronous function to be executed on tear down.</param>
        public SingleStreamConsumer(IStreamProvider streamProvider, IStreamMessageVisitor<TIn> streamMessageVisitor, Func<Task> tearDownFunc = null)
        {
            _streamProvider = streamProvider;
            _tearDownFunc = tearDownFunc;
            _awaitedTransactions = new Dictionary<int, TaskCompletionSource<Task>>();
            _streamMessageVisitor = streamMessageVisitor;
        }

        public async Task SetInput(StreamIdentity<TIn> inputStream)
        {
            _tearDownExecuted = false;
            Console.WriteLine(">>> SetInput GetStream: " + inputStream.StreamIdentifier.Item1 + " " + inputStream.StreamIdentifier.Item2);
            var messageStream = _streamProvider.GetStream<IStreamMessage>(inputStream.StreamIdentifier.Item1, inputStream.StreamIdentifier.Item2);

            //Modified: Await split
            Func<Task> onCompleted = new Func<Task>(async () => await TearDown());
            _messageStreamSubscriptionHandle =
                await messageStream.SubscribeAsync((message, token) => message.Accept(this), onCompleted);
        }

        public virtual async Task TearDown()
        {
            if (!_tearDownExecuted)
            {
                _tearDownExecuted = true;
                if (_messageStreamSubscriptionHandle != null)
                {
                    await _messageStreamSubscriptionHandle.UnsubscribeAsync();
                }

                _messageStreamSubscriptionHandle = null;
                if (_tearDownFunc != null)
                {
                    await _tearDownFunc();
                }
            }
        }

        public Task<bool> IsTearedDown()
        {
            return Task.FromResult(_tearDownExecuted);
        }

        /// <summary>
        /// Returns if transaction is completed.
        /// </summary>
        /// <param name="transactionId">Transaction identifier.</param>
        /// <returns></returns>
        public async Task TransactionComplete(int transactionId)
        {
            if (!_awaitedTransactions.ContainsKey(transactionId))
            {
                _awaitedTransactions[transactionId] = new TaskCompletionSource<Task>();
            }

            await _awaitedTransactions[transactionId].Task;
        }

        private void TransactionMessageArrived(TransactionMessage transactionMessage)
        {
            if (transactionMessage.State == TransactionState.Start)
            {
                if (!_awaitedTransactions.ContainsKey(transactionMessage.TransactionId))
                {
                    _awaitedTransactions[transactionMessage.TransactionId] = new TaskCompletionSource<Task>();
                }
            }

            else if (transactionMessage.State == TransactionState.End)
            {
                _awaitedTransactions[transactionMessage.TransactionId].SetResult(TaskDone.Done);
            }
        }

        public async Task Visit(ItemMessage<TIn> message)
        {
            if (_streamMessageVisitor != null)
            {
                await _streamMessageVisitor.Visit(message);
            }
        }

        public async Task Visit(TransactionMessage transactionMessage)
        {
            TransactionMessageArrived(transactionMessage);
            if (_streamMessageVisitor != null)
            {
                await _streamMessageVisitor.Visit(transactionMessage);
            }
        }
    }
}