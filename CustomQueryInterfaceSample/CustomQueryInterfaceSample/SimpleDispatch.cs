// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ManagedAggregation;


namespace SampleCcws
{
    /// <summary>
    /// This class provide a very simple manage implementation of IDispatch.  It is an inner HOR class.
    /// </summary>
    public class SimpleDispatch : BaseInnerHoldingOuterReference, IDispatch
    {
        #region Members
        private volatile int dispSeed = 1;

        // Caches of the ccw information from server
        // <Interface Type, Class Type>
        Dictionary<Type, Type> interfaceClassDictionary = null;

        // <Class Type, Class instance>
        Dictionary<Type, object> classInstanceDictionary = null;

        // <MethodName, DispID>
        Dictionary<string, int> methodNameIDDictionary = null;

        // <DispID, MethodInfo>
        Dictionary<int, MethodInfo> methodIDInfoDictionary = null;
        #endregion Members

        #region Constructor and Destuctor
        public SimpleDispatch()
        {
            // <Interface Type, Class Type>
            interfaceClassDictionary = new Dictionary<Type, Type>();

            // <Class Type, Class instance>
            classInstanceDictionary = new Dictionary<Type, object>();

            // <MethodName, DispID>
            methodNameIDDictionary = new Dictionary<string, int>();

            // <DispID, MethodInfo>
            methodIDInfoDictionary = new Dictionary<int, MethodInfo>();
        }

        ~SimpleDispatch()
        {
            Console.WriteLine("Finalize Inner SimpleDispatch");
        }
        #endregion Constructor and Destuctor

        #region IDispatch implementation
        public void GetTypeInfoCount(out uint pctinfo)
        {
            //NOT IMPLEMENTED
            pctinfo = 1;
        }

        public void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info)
        {
            //NOT IMPLEMENTED
            info = IntPtr.Zero;
        }

        public void GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
                string[] names,
            int cNames,
            int lcid,
            [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
                int[] rgDispId)
        {
            // ignore lcid, let's assume it is enu
            // Add strict check for parameters Here
            for (int i = 0; i < cNames; i++)
            {
                string name = names[i];
                // look for the dispID in cache
                // if not get, go to server
                if (!methodNameIDDictionary.TryGetValue(name, out rgDispId[i]))
                {
                    // if not in cache, go to server to check all the interfaces
                    Type interfaceType = LookForInterface(name);
                    if (interfaceType != null)
                    {
                        PullInInterface(interfaceType);
                        rgDispId[i] = methodNameIDDictionary[name];
                    }
                    else
                        rgDispId[i] = -1;
                }
            }
        }

        public void Invoke(
            int dispId,
            ref Guid riid,
            int lcid,
            ushort wFlags,
            ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
            out object result,
            IntPtr pExcepInfo,
            IntPtr puArgErr)
        {
            // Ignore riid
            // Ignore lcid 
            // Ignore wFlags
            // Ignore pExcepInfo
            // Ignore puArgErr
            // Add strict check for parameters Here
            if (methodIDInfoDictionary.ContainsKey(dispId))
            {
                try
                {
                    MethodInfo mi = methodIDInfoDictionary[dispId];
                    int length = pDispParams.cArgs;
                    Type interfaceType = mi.DeclaringType;
                    Type classType = interfaceClassDictionary[interfaceType];
                    Console.WriteLine("Server ClassType: " + classType);
                    Console.WriteLine("Server InterfaceType: " + interfaceType);
                    object o = classInstanceDictionary[classType];

                    // Get the parameter array from the native array
                    object[] args = Marshal.GetObjectsForNativeVariants(pDispParams.rgvarg, length);

                    // Reverse the array                
                    Array.Reverse(args);

                    result = mi.Invoke(o, args);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Fail to invoke method for DispID " + dispId);
                    Console.WriteLine(ex.ToString());
                    result = 1;
                }
            }
            else
            {
                Console.WriteLine("Method with DISPID " + dispId + " does not exist!");
                throw new InvalidOperationException("Not supported method!");
            }
        }

        #endregion IDispatch implementation

        #region Private Methods
        private int NewDispId()
        {
            return dispSeed++;
        }

        /// <summary>
        ///  Get interface type which have the specified method.
        /// </summary>
        private Type LookForInterface(string methodName)
        {
            Console.WriteLine("Look for interface who provide " + methodName + " in server!");
            Type[] interfaceTypes = Outer.GetAggregatedInterfaceTypes();
            foreach (Type t in interfaceTypes)
            {
                MethodInfo mi = t.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (mi != null)
                {
                    Console.WriteLine("Successfully find interface " + t + " who provides " + methodName);
                    return t;
                }
            }

            Console.WriteLine("Fail to find interface who provide " + methodName);
            return null;
        }

        
        /// <summary>
        /// Get interface information from the ccw server(outer), and cache them in local.
        /// 1) store the "interface    <->   Class"  
        /// 2) store the "Method Name  <->   DispID" 
        /// 3) store the "DispID       <->   MethodInfo"
        /// </summary>
        /// <param name="interfaceType"></param>
        private void PullInInterface(Type interfaceType)
        {
            if(interfaceType == null)
                throw new ArgumentNullException("Pass in parameter should not be null");

            Console.WriteLine("Try to pull in the interface info for " + interfaceType);
            // 1) store the "interface    <->   Class"  
            Type classType = Outer.GetAggregatedClassFromInterface(interfaceType);
            interfaceClassDictionary.Add(interfaceType, classType);
            object o = Outer.GetAggregatedInstanceFromClass(classType);
            classInstanceDictionary[classType] = o;

            // store all the method information
            MethodInfo[] methods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo mi in methods)
            {
                int id = NewDispId();
                Console.WriteLine("Pull in " + mi.Name + " with ID " + id);

                // 2) store the "Method Name  <->   DispID" 
                methodNameIDDictionary.Add(mi.Name, id);

                // 3) store the "DispID       <->   MethodInfo"
                methodIDInfoDictionary.Add(id, mi);
            }
        }
        #endregion Private Methods
    }

}