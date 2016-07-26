//-----------------------------------------------------------------------
// <copyright file="StreamSequenceToken.cs">
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
    //
    // Summary:
    //     Handle representing stream sequence number/token. Consumer may subsribe to the
    //     stream while specifying the start of the subsription sequence token. That means
    //     that the stream infarstructure will deliver stream events starting from this
    //     sequence token.
    public abstract class StreamSequenceToken : IEquatable<StreamSequenceToken>, IComparable<StreamSequenceToken>
    {
        public abstract int CompareTo(StreamSequenceToken other);
        public abstract bool Equals(StreamSequenceToken other);
    }
}