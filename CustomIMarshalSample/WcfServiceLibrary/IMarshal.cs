///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Managed declaration of the COM interface IMarshal
    /// </summary>
    [Guid("00000003-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IMarshal
    {
        void GetUnmarshalClass(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlFlags, ref Guid pCid);

        void GetMarshalSizeMax(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags, out uint pSize);

        void MarshalInterface(IntPtr pStm, ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags);

        void UnmarshalInterface(IntPtr pStm, ref Guid riid, out IntPtr ppv);

        void ReleaseMarshalData(IntPtr pStm);

        void DisconnectObject(uint dwReserved);
    }
}
