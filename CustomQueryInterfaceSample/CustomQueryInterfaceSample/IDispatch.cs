// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==

using System;
using System.Runtime.InteropServices;

namespace SampleCcws
{
    /// <summary>
    /// This interface is the managed version of the native com IDispatch, it has the same vtable layout
    /// as well as the GUID of the native IDispatch interface.
    /// </summary>
    [ComImport]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDispatch
    {
        void GetTypeInfoCount(out uint pctinfo);

        void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

        void GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
                string[] names,
            int cNames,
            int lcid,
            [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
                int[] rgDispId);

        void Invoke(
            int dispId,
            ref Guid riid,
            int lcid,
            ushort wFlags,
            ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
            out object result,
            IntPtr pExcepInfo,
            IntPtr puArgErr);
    }
}