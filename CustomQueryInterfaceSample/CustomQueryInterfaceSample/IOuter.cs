// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

namespace ManagedAggregation
{
    /// <summary>
    /// Call back to inform the subscribled client the event of aggregation of the specified interface
    /// </summary>
    /// <param name="guid">Guid of the aggregated interface</param>
    /// <param name="interfaceType">Type information of the aggregated interface</param>
    /// <param name="classType">Type information of the class that implements aggregated interface</param>
    public delegate void AggregationUpdateDelegate(Guid guid, Type interfaceType, Type classType);

    /// <summary>
    /// This interface defines the methods which are needed to make up a base container of CCWs (Outer).
    /// </summary>
    public interface IOuter
    {
        /// <summary>
        /// Aggregate the com server(ccw) which exposes the specified interface. The Guid of the interface exposed to client 
        /// is the exact guid of the specified interface.
        /// </summary>
        /// <param name="interfaceType">Interface that exposed to client</param>
        /// <param name="classType">Server class that implement the interface</param>
        void Aggregate(Type interfaceType, Type classType);

        /// <summary>
        /// Return all the aggregated interfaces information.
        /// </summary>
        Type[] GetAggregatedInterfaceTypes();

        /// <summary>
        /// Return the aggregated class information for the specified interface.
        /// </summary>
        /// <param name="interfaceType">Interface that registed in the server.</param>
        /// <returns>Type information of the specified interface</returns>
        Type GetAggregatedClassFromInterface(Type interfaceType);

        /// <summary>
        /// Return the aggregated class instance of the specified class.
        /// </summary>
        /// <param name="classType">Class that registered in the server.</param>
        /// <returns>Instance of the class</returns>
        object GetAggregatedInstanceFromClass(Type classType);

        /// <summary>
        /// The event of aggregation of the specified interface
        /// </summary>
        event AggregationUpdateDelegate aggregationEvent;
    }
}