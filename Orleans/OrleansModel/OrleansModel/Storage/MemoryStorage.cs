//-----------------------------------------------------------------------
// <copyright file="MemoryStorage.cs">
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

namespace Orleans.Storage
{
    /// <summary>
    /// Simple in-memory grain implementation
    /// of a storage provider.
    /// </summary>
    public class MemoryStorage : IStorageProvider
    {
        internal const int NumStorageGrainsDefaultValue = 10;

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string Name { get; private set; }
    }
}