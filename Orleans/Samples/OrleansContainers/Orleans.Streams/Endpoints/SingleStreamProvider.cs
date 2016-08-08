using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams.Messages;

namespace Orleans.Streams.Endpoints
{
    /// <summary>
    ///     Provides a single transactional stream.
    /// </summary>
    /// <typeparam name="T">Data type transmitted via the stream</typeparam>
    public class SingleStreamProvider<T> : ITransactionalStreamProvider<T>
    {
        public const string StreamNamespacePrefix = "SingleStreamProvider";
        private readonly IAsyncStream<IStreamMessage> _messageStream;
        private readonly StreamIdentity<T> _streamIdentity;
        private int _lastTransactionId = -1;
        private bool _tearDownExecuted;

        public SingleStreamProvider(IStreamProvider provider, Guid guid = default(Guid))
        {
            guid = guid == default(Guid) ? Guid.NewGuid() : guid;
            _streamIdentity = new StreamIdentity<T>(StreamNamespacePrefix, guid);
            _messageStream = provider.GetStream<IStreamMessage>(_streamIdentity.StreamIdentifier.Item1,
                _streamIdentity.StreamIdentifier.Item2);
            _tearDownExecuted = false;
        }

        public Task<StreamIdentity<T>> GetStreamIdentity()
        {
            return Task.FromResult(_streamIdentity);
        }

        public async Task TearDown()
        {
            _tearDownExecuted = true;
            await _messageStream.OnCompletedAsync();
        }

        public Task<bool> IsTearedDown()
        {
            return Task.FromResult(_tearDownExecuted);
        }

        public async Task<int> SendItems(IEnumerable<T> items, bool useTransaction = true, int? transactionId = null)
        {
            var curTransactionId = transactionId ?? ++_lastTransactionId;
            if (useTransaction)
            {
                await StartTransaction(curTransactionId);
            }
            var message = new ItemMessage<T>(items);
            await Task.WhenAll(_messageStream.OnNextAsync(message));
            if (useTransaction)
            {
                await EndTransaction(curTransactionId);
            }

            return curTransactionId;
        }

        public Task<int> SendItem(T item, bool useTransaction = true, int? transactionId = null)
        {
            return SendItems(new List<T>(1) { item }, useTransaction, transactionId);
        }

        public async Task StartTransaction(int transactionId)
        {
            await _messageStream.OnNextAsync(new TransactionMessage { State = TransactionState.Start, TransactionId = transactionId });
        }

        public async Task EndTransaction(int transactionId)
        {
            await _messageStream.OnNextAsync(new TransactionMessage { State = TransactionState.End, TransactionId = transactionId });
        }
    }
}