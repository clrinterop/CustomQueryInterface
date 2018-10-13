///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WcfServiceLibrary;

namespace WcfClient
{
    /// <summary>
    /// The client class for the WCF service, ISerivce1.
    /// In COM view, it is the proxy class.
    /// 
    /// The implementation of Custom QueryInterface ensures that our implementation
    /// of IMarshal is defined.
    /// 
    /// Most implementation of IMarshal are dummy. The only interesting one, UnmarshalInterface,
    /// return the IUnknown of the objest itself as the unmarshal result.
    /// 
    /// The implementation of IService1 client then just dispatched all invocation
    /// to the WCF client
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("A6934A6A-F2FA-3DC7-B0F1-D61BA7C2DB75")]
    public class Client : IMarshal, ICustomQueryInterface, IService1, IDisposable
    {
        #region WCF client related implementation

        /// <summary>
        /// The WCF client of IService1
        /// </summary>
        WcfServiceReference.Service1Client client;

        public Client()
        {
            Util.WriteLineWithThreadId("Client: Constructing Service proxy");
            try
            {
                client = new WcfServiceReference.Service1Client();
            }
            catch (Exception e)
            {
                Util.WriteLineWithThreadId(e.Message);
            }
        }

        public string GetData(int value)
        {
            Util.WriteLineWithThreadId("Client: Invoking GetData");
            return client.GetData(value);
        }

        #endregion WCF client related implementation

        #region ICustomQueryInterface implementation

        /// <summary>
        /// The implementation of Custom QueryInterface. It returned the customized implementation of IMarshal
        /// </summary>
        /// <param name="iid"></param>
        /// <param name="intf"></param>
        /// <returns></returns>
        public CustomQueryInterfaceResult GetInterface([In]ref Guid iid, out IntPtr intf)
        {
            Util.WriteLineWithThreadId(String.Format("Client: QI for {0}", iid));

            intf = IntPtr.Zero;
            if (iid == Guids.IID_IMarshal)
            {
                try
                {
                    // return our own implementation of IMarshal
                    intf = Marshal.GetComInterfaceForObject(this, typeof(IMarshal), CustomQueryInterfaceMode.Ignore);
                }
                catch (Exception e)
                {
                    Util.WriteLineWithThreadId(e.ToString());
                    return CustomQueryInterfaceResult.Failed;
                }
                Util.WriteLineWithThreadId(String.Format("Client: QI IMarshal returned {0}", intf));
                // Inform runtime that we've handle the QI of IMarshal
                return CustomQueryInterfaceResult.Handled;
            }

            // let runtime handle the rest of QI
            return CustomQueryInterfaceResult.NotHandled;
        }

        #endregion ICustomQueryInterface implementation

        #region IMarshal related implemtation

        public void UnmarshalInterface(IntPtr pStm, ref Guid riid, out IntPtr ppv)
        {
            ppv = Marshal.GetIUnknownForObject(this);
            Util.WriteLineWithThreadId(String.Format("Client: UnmarshalInterface result: {0}", ppv));
        }

        public void GetUnmarshalClass(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlFlags, ref Guid pCid)
        {
            Util.WriteLineWithThreadId("Client: GetUnmarshalClass");
        }

        public void GetMarshalSizeMax(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags, out uint pSize)
        {
            Util.WriteLineWithThreadId("Client: GetMarshalSizeMax");
            pSize = (uint)IntPtr.Size;
        }

        public void MarshalInterface(IntPtr pStm, ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags)
        {
            Util.WriteLineWithThreadId("Client: MarshalInterface");
            Util.WriteLineWithThreadId(String.Format("Client: pStm {0}, pv {1}", pStm, pv));
        }

        public void ReleaseMarshalData(IntPtr pStm)
        {
            Util.WriteLineWithThreadId("Client: ReleaseMarshalData");
        }

        public void DisconnectObject(uint dwReserved)
        {
            Util.WriteLineWithThreadId("Client: DisconnectObject");
        }

        #endregion IMarshal related implemtation

        #region IDispsable related implementation
        public void Dispose()
        {
            lock (this)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
            }
        }

        #endregion IDispsable related implementation

        ~Client()
        {
            Dispose();
        }
    }
}
