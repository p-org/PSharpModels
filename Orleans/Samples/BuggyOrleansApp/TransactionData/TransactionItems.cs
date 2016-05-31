using System;

namespace BuggyOrleansApp
{
    //[Serializable]
    public class TransactionItems : ICloneable
    {
        public string name;

        public TransactionItems()
        {

        }

        public TransactionItems(string name)
        {
            this.name = name;
        }

        object ICloneable.Clone()
        {
            return new TransactionItems(this.name);
        }
    }
}