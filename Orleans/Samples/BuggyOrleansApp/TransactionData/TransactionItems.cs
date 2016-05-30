using System;

namespace BuggyOrleansApp
{
    //[Serializable]
    public class TransactionItems
    {
        public string name;

        public TransactionItems()
        {

        }

        public TransactionItems(string name)
        {
            this.name = name;
        }
    }
}