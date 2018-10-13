///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using WcfServiceLibrary;

namespace ConsoleApplication1
{
    /// <summary>
    /// The program demos the managed implementation of IMarshal by using customized
    /// QueryInterface and WCF. 
    /// 
    /// The scenario is to pass an IUnknown pointer from thread A to thread B. Therefore,
    /// the IMarshal methods (implemented in managed code) can be invoked. The invoke on 
    /// the IUnknown pointer from thread B can successfully be dispatched to the object
    /// created in thread A.
    /// </summary>
    class Program
    {
        #region static variables

        /// <summary>
        /// Reference of the WCF service
        /// </summary>
        static Service1 g_service;

        /// <summary>
        /// Pointer to native COM IStream
        /// </summary>
        static IntPtr g_pStm = IntPtr.Zero;

        #endregion static variables

        #region P/Invoke definition

        [DllImportAttribute("ole32.dll", EntryPoint = "CoMarshalInterThreadInterfaceInStream", CallingConvention = CallingConvention.StdCall)]
        public static extern int CoMarshalInterThreadInterfaceInStream(ref Guid riid, IService1 pUnk, out IntPtr ppStm);

        [DllImportAttribute("ole32.dll", EntryPoint = "CoGetInterfaceAndReleaseStream", CallingConvention = CallingConvention.StdCall)]
        public static extern int CoGetInterfaceAndReleaseStream(IntPtr pStm, ref Guid iid, out System.IntPtr ppv);

        #endregion P/Invoke definition

        static void Main(string[] args)
        {
            // Thread A constructs an object which has customized IMarshal Implementation.
            // The object is marshalled to IStream.
            EnableServiceInThreadA();

            // Thread B (Main thread) unmarshal the object and then invoke it
            UnmarshalAndInvokeInMainThread();

            // Close the client and Service
            Cleanup();
        }

        static void EnableServiceInThreadA()
        {
            Thread t = new Thread(ThreadProc);
            t.Start();
            t.Join();
        }

        static void UnmarshalAndInvokeInMainThread()
        {
            // Unmarshal service through IMarshal
            IntPtr pV = IntPtr.Zero;
            Guid guid = typeof(IService1).GUID;
            CoGetInterfaceAndReleaseStream(g_pStm, ref guid, out pV);

            if (pV != IntPtr.Zero)
            {
                IService1 service = (IService1)Marshal.GetObjectForIUnknown(pV);

                // Invoke the method
                Util.WriteLineWithThreadId(service.GetData(10).ToString());

                // Release the pV so that service can be GCed
                Marshal.Release(pV);
            }
        }

        static void Cleanup()
        {
            // Collect the Client so that the it could Close the WCF connection
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Close the WCF service (started in ThreadProc)
            g_service.Close();
        }

        /// <summary>
        /// Start the WCF service and marshal it to the global stream.
        /// </summary>
        public static void ThreadProc()
        {
            Util.WriteLineWithThreadId("Enable the WCF service");

            // Start the WCF service
            g_service = Service1.GetInstance();

            // Marshal it to the global stream
            Guid guid = typeof(IService1).GUID;
            int hr = CoMarshalInterThreadInterfaceInStream(ref guid, (IService1)g_service, out g_pStm);
            if (hr < 0)
            {
                Util.WriteLineWithThreadId(String.Format("Failed during the testsetup: Unable to create the RCW! {0} ", hr));
            }

            Util.WriteLineWithThreadId(String.Format("Server: gstream {0}", g_pStm));
        }

        
    }
}
