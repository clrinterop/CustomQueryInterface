// ==++==
//
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==
using System;

namespace ManagedAggregation
{   
    /// <summary>
    /// This class gives a simple implmentation of interface IOuter. It is enough to the inner who wants
    /// to get information from the outer but does not want to subscribe any event.
    /// </summary>
    public class BaseInnerHoldingOuterReference : IInnerHoldingOuterReference
    {
        public virtual IOuter Outer
        {
            set
            {
                // Because Outer will hold the IUnknown pointer of the inner, which is a GC root for inner
                // object, we cannot hold the reference of outer in the inner object. Otherwise, there is 
                // a circular reference between outer and inner, which prevent GC from collecting them.

                // For those inner object who really want to hold the reference of outer, it can use a weak
                // reference here since weak reference is not count during GC.
                aggregator = new WeakReference(value);
            }

            get
            {
                return aggregator.Target as IOuter;
            }
        }

        protected WeakReference aggregator;
    }
}