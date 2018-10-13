// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==

/// We want to demonstrate the power of CustomQueryInterface feature delivered in .Net Framework V4.0. In this smaple, 
/// we will show you how to enable COM Aggregation with managed objects acting as Outer and Inner, 
/// as well as using managed IDispatch implementation to override the default IDispatch implementation by CLR.
/// 
/// With aggregation, we can deliver a very dynamic implementation of the CCW(Managed COM Server) System.
/// 
/// terms we used in this sample
/// 
/// CCW: Com Callable Wrapper which bridging the managed object and the native COM client.
/// Outer: It is the ccw server who aggregates many ccws.
/// Inner: It is the ccw which is aggregated by the ccw server.
/// HOR: It is short for holdingOuterReference.
/// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

namespace ManagedAggregation
{
    /// <summary>
    /// This class serves as a base implementation of a ccw Server(Outer) which aggregates many ccws.
    /// </summary>
    [ComVisible(true)]
    public class BaseOuter : IOuter, ICustomQueryInterface
    {
        #region Members
        // <Interface Guid, Interface Type>
        Dictionary<Guid, Type> guidTypeDictionary = null;

        // <Interface Type, Class Type>
        Dictionary<Type, Type> interfaceClassDictionary = null;

        // <Class Type, Instance>
        Dictionary<Type, object> classInstanceDictionary = null;

        // This link list holds the reference o f all the Aggregated object(innner) 
        // who has hold the reference of outer. .
        LinkedList<IntPtr> innerHORCCWList = null;

        // Event which is fired when Outer aggregates an inner
        public event AggregationUpdateDelegate aggregationEvent = null;

        #endregion Members

        #region Constructor and Destuctor
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseOuter()
        {
            guidTypeDictionary = new Dictionary<Guid, Type>();
            interfaceClassDictionary = new Dictionary<Type, Type>();
            classInstanceDictionary = new Dictionary<Type, object>();
            innerHORCCWList = new LinkedList<IntPtr>();
        }

        /// <summary>
        /// This destructor will release all the HOR inner references 
        /// </summary>
        ~BaseOuter()
        {
            foreach (IntPtr inner in innerHORCCWList)
            {
                Marshal.Release(inner);
            }
            Console.WriteLine("BaseOuter is Finalized");
        }
        #endregion Constructor and Destuctor

        #region IOuter implementation
        public Type[] GetAggregatedInterfaceTypes() 
        { 
            return interfaceClassDictionary.Keys.ToArray(); 
        }

        public Type GetAggregatedClassFromInterface(Type interfaceType)
        { 
            if(interfaceClassDictionary.ContainsKey(interfaceType))
                return interfaceClassDictionary[interfaceType]; 
            return null;
        }
        
        public object GetAggregatedInstanceFromClass(Type classType) 
        {
            object o = null;
            classInstanceDictionary.TryGetValue(classType, out o); 
            return o;
        }

        public void Aggregate(Type interfaceType, Type classType)
        {
            Aggregate(interfaceType.GUID, interfaceType, classType);
        }

        #endregion

        #region ICustomQueryInterface implementation
        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv)
        {
            ppv = IntPtr.Zero;
            // check the aggregated object has implemented the interfaces
            // if true, return the pointer to ccw
            if (guidTypeDictionary.ContainsKey(iid))
            {
                Type interfaceType = guidTypeDictionary[iid];
                Type classType = interfaceClassDictionary[interfaceType];
                object o = classInstanceDictionary[classType];

                // CustomQueryInterfaceMode.Ignore is used below to notify CLR to bypass the invocation of
                // ICustomQueryInterface.GetInterface to avoid infinite loop during QI.
                ppv = Marshal.GetComInterfaceForObject(o, interfaceType, CustomQueryInterfaceMode.Ignore);
                return CustomQueryInterfaceResult.Handled;
            }

            // Let CLR handle the rest of the QI
            return CustomQueryInterfaceResult.NotHandled;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Aggregate the com server which exposes the specified interface, the Guid of the interface that exposed to 
        /// client is the specified guid.
        /// </summary>
        /// <param name="guid">Guid of the interface that specified the interface</param>
        /// <param name="interfaceType">Interface that exposed to client</param>
        /// <param name="classType">Server class that implement the interface</param>
        private void Aggregate(Guid guid, Type interfaceType, Type classType)
        {
            if (guid == null || interfaceType == null || classType == null)
                throw new ArgumentNullException("Parameter should not be null");

            // if already have interface registered with same Guid, action fails
            if (guidTypeDictionary.ContainsKey(guid))
            {
                throw new InvalidOperationException(String.Format("Interface with Guid {0} has already been registered"
                    + " in the server, please unregister the interface before registration!", guid));
            }

            guidTypeDictionary.Add(guid, interfaceType);

            if (interfaceClassDictionary.ContainsKey(interfaceType))
            {
                Type tmpClassType = interfaceClassDictionary[interfaceType];
                // if alreay have another registered class that implements and exposes the same interfae, action fails
                if (tmpClassType != classType)
                {
                    // roll back, remove the previous registration of GUID/Interface
                    guidTypeDictionary.Remove(guid);

                    string errorMsg = String.Format("Can not register the interface {0} which is already exposed and implemented by Class {1} .",
                                                    interfaceType.ToString(),
                                                    tmpClassType.ToString());
                    throw new InvalidOperationException(errorMsg);
                }

                // have already created the instance of the type and maintained the dictionaries.
                return;
            }

            // <Interface Guid, Interface Type>
            interfaceClassDictionary.Add(interfaceType, classType);

            // <Class Type, Instance>
            // construct a new object instance if do not have, aggreate the object and store the mapping
            object o = null;
            if (!classInstanceDictionary.ContainsKey(classType))
            {
                // create the new instance
                ConstructorInfo ci = classType.GetConstructor(Type.EmptyTypes);
                o = ci.Invoke(new Object[0]);
                classInstanceDictionary.Add(classType, o);

                // Set the aggregator
                IInnerHoldingOuterReference ins = o as IInnerHoldingOuterReference;
                if (ins != null)
                {
                    Console.WriteLine("Set Aggregator for " + o.GetType());
                    ins.Outer = this;
                }

                // Aggregate the object 
                IntPtr pUnk = Marshal.GetIUnknownForObject(this);
                IntPtr pInnerIUnkPtr = Marshal.CreateAggregatedObject(pUnk, o);

                // This release is to ensure that ‘this’ don’t hold a ref-count on itself, 
                // which could prevent GC from collecting ‘this’ instance at the end
                Marshal.Release(pUnk);
                innerHORCCWList.AddLast(pInnerIUnkPtr);

                // Inform the aggregated objects who have subscribed to the UnAggregation event
                if (aggregationEvent != null)
                {
                    aggregationEvent(guid, interfaceType, classType);
                }
            }
        }
        #endregion Private Methods
    }
}