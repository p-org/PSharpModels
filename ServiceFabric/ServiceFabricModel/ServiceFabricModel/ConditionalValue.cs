using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ServiceFabric.Data
{
    //
    // Summary:
    //     Result class returned by DistributedCollections APIs that may or may not return
    //     a value.
    //
    // Type parameters:
    //   TValue:
    //     Value to initialize the result with.
    public struct ConditionalValue<TValue>
    {
        bool hasValue;
        private bool v;
        TValue value;

        public ConditionalValue(bool v) : this()
        {
            this.v = v;
        }

        //
        // Summary:
        //     Initializes a new instance of the ConditionalValue class with the given value.
        //
        // Parameters:
        //   hasValue:
        //     Indicates whether the value is valid.
        //
        //   value:
        //     The value.
        public ConditionalValue(bool hasValue, TValue value) : this()
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        //
        // Summary:
        //     Gets a value indicating whether the value is valid.
        //
        // Returns:
        //     Whether the value is valid.
        public bool HasValue { get; }
        //
        // Summary:
        //     Gets the value.
        //
        // Returns:
        //     The value.
        public TValue Value { get; }
    }
}