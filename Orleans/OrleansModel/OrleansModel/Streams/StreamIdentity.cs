//-----------------------------------------------------------------------
// <copyright file="StreamIdentity.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

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