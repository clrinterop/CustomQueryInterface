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
    /// This interface defines the methods that enable the aggregated object(inner) to get 
    /// informartion from the aggregator(outer). See BaseInnerHoldingOuterReference.cs for
    /// more information.
    /// </summary>
    public interface IInnerHoldingOuterReference
    {
        /// <summary>
        /// Get/Set the Outer referemce who aggregates this object.
        /// </summary>
        IOuter Outer
        {
            set;
            get;
        }
    }
}