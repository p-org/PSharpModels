using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serialize.Linq.Extensions;

namespace Orleans.Collections.Utilities
{
    public static class RemoteLambdaExtensions
    {
        public static async Task ExecuteLambda<T>(this ICollectionOperations<T> collection, Expression<Func<CollectionHostedElement<T>, object, Task>> func, object state = null)
        {
            var expNode = func.ToExpressionNode();
            await collection.ExecuteLambda(expNode, state);
        }

        //public static async Task ExecuteLambda<T>(this ICollectionOperations<T> collection, Expression<Action<CollectionHostedElement<T>, object>> func, object state = null)
        //{
        //    Expression<Func<CollectionHostedElement<T>, object, Task>> asyncExpression = (element, o) => Expression.Call(func.me)
        //    var expression = func.Compile();
        //    var expNode = func.ToExpressionNode();
        //    await collection.ExecuteLambda(expNode, state);
        //}

        public static async Task ExecuteBatchLambda<T>(this ICollectionOperations<T> collection, Expression<Func<IEnumerable<CollectionHostedElement<T>>, object, Task>> func, object state = null)
        {
            var expNode = func.ToExpressionNode();
            await collection.ExecuteBatchLambda(expNode, state);
        }
    }
}