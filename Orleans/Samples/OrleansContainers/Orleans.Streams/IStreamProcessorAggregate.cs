namespace Orleans.Streams
{
    /// <summary>
    /// Transforms data from TIn to TOut.
    /// </summary>
    /// <typeparam name="TIn">Data input type.</typeparam>
    /// <typeparam name="TOut">Data output type.</typeparam>
    public interface IStreamProcessorAggregate<TIn, TOut> : IGrainWithGuidKey, ITransactionalStreamConsumerAggregate<TIn>, ITransactionalStreamProviderAggregate<TOut>
    {
    }
}