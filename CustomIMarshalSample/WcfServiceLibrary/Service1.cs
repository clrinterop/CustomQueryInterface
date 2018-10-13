///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace WcfServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    /// <summary>
    /// WCF service implemtnation. It also implements IMarshal and ICustomQueryInterface.
    /// 
    /// The implementation of ICustomQueryInterface ensure that runtime returns the customized implementation of IMarshal
    /// when handling the QI on IMarshal.
    /// 
    /// Most of the IMarshal implementation are dummy. The only interesting one, GetUnmarshalClass, returns the CLS_ID of
    /// WcfClient.Client to let it handle the unmarshal staff.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Service1 : IMarshal, ICustomQueryInterface, IService1
    {

        #region singleton pattern implementation

        static Service1 instance;

        public static Service1 GetInstance()
        {
            lock (typeof(Service1))
            {
                if (instance == null)
                    instance = new Service1();
            }
            return instance;
        }

        #endregion singleton pattern implementation

        #region WCF related implementation

        ServiceHost selfHost;

        /// <summary>
        /// Use the singleton pattern to assure only one instance is constructed
        /// </summary>
        Service1()
        {
            // Step 1 of the address configuration procedure: Create a URI to serve as the base address.
            Uri baseAddress = new Uri("http://localhost:8000/ServiceModelSamples/Service");

            // Step 2 of the hosting procedure: Create ServiceHost
            selfHost = new ServiceHost(this, baseAddress);

            try
            {
                // Step 3 of the hosting procedure: Add a service endpoint.
                selfHost.AddServiceEndpoint(
                    typeof(IService1),
                    new WSHttpBinding(),
                    "IService");

                // Step 4 of the hosting procedure: Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                // Step 5 of the hosting procedure: Start (and then stop) the service.
                selfHost.Open();
                Util.WriteLineWithThreadId("Server: The service is ready.");
            }
            catch (CommunicationException ce)
            {
                Util.WriteLineWithThreadId(String.Format("An exception occurred: {0}", ce.Message));
                selfHost.Abort();
            }
        }

        public void Close()
        {
            // Close the ServiceHostBase to shutdown the service.
            selfHost.Close();
        }

        #endregion WCF related implementation

        #region IService implementation

        public string GetData(int value)
        {
            Util.WriteLineWithThreadId("Server: Invoking GetData");
            return string.Format("You entered: {0}", value);
        }

        #endregion IService implementation

        #region ICustomQueryInterface implementation

        public CustomQueryInterfaceResult GetInterface([In]ref Guid iid, out IntPtr intf)
        {
            Util.WriteLineWithThreadId(String.Format("Server: QI for {0}", iid));
            intf = IntPtr.Zero;

            if (iid == Guids.IID_IMarshal)
            {
                intf = Marshal.GetComInterfaceForObject(this, typeof(IMarshal), CustomQueryInterfaceMode.Ignore);
                Util.WriteLineWithThreadId(string.Format("Server: QI IMarsahl returned {0}", intf));
                return CustomQueryInterfaceResult.Handled;
            }
            return CustomQueryInterfaceResult.NotHandled;
        }

        #endregion ICustomQueryInterface implementation

        #region IMarshal Implementatin

        public void UnmarshalInterface(IntPtr pStm, ref Guid riid, out IntPtr ppv)
        {
            Util.WriteLineWithThreadId("Server: UnmarshalInterface");
            ppv = IntPtr.Zero;
        }

        public void GetUnmarshalClass(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlFlags, ref Guid pCid)
        {
            Util.WriteLineWithThreadId("Server: GetUnmarshalClass");
            // We return the guid of the class which wraps the Wcf Client.
            pCid = Guids.CLSID_Client;
            Util.WriteLineWithThreadId(string.Format("Server: guid of UnmarshalClass is {0}", pCid));
        }

        public void GetMarshalSizeMax(ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags, out uint pSize)
        {
            Util.WriteLineWithThreadId("Server: GetMarshalSizeMax");
            pSize = (uint) IntPtr.Size;
        }

        public void MarshalInterface(IntPtr pStm, ref Guid riid, System.IntPtr pv, uint dwDestContext, System.IntPtr pvDestContext, uint mshlflags)
        {
            Util.WriteLineWithThreadId("Server: MarshalInterface");
        }

        public void ReleaseMarshalData(IntPtr pStm)
        {
            Util.WriteLineWithThreadId("Server: ReleaseMarshalData");
        }
        public void DisconnectObject(uint dwReserved)
        {
            Util.WriteLineWithThreadId("Server: DisconnectObject");
        }

        #endregion IMarshal Implementatin
    }
}
