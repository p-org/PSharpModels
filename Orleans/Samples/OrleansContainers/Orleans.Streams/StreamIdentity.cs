using System;

namespace Orleans.Streams
{
    /// <summary>
    /// Stores information about all transactional streams available.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public class StreamIdentity<TItem>
    {
        private const string NamespacePostfix = "MessageStream";

        public Tuple<Guid, string> StreamIdentifier { get; private set; }


        public StreamIdentity(string namespacePrefix, Guid streamIdentifier)
        {
            StreamIdentifier = new Tuple<Guid, string>(streamIdentifier, namespacePrefix + NamespacePostfix);
        }
    }
}