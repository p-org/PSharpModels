using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Actor2
{
    [DataContract]
    public class Actor2State
    {
        [DataMember]
        public int Value;

        public Actor2State(int val)
        {
            this.Value = val;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Actor2State[Value = {0}]", Value);
        }
    }
}
