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
    /// This is a help class which enable native world to invoke a GC call in clr. 
    /// </summary>
    class Utility
    {
        public delegate void Funptr();

        [DllImport("Client.exe")]
        public static extern void hookGC(Funptr f);

        private static Funptr gcCallback = new Funptr(Utility.callGC);
        public static void AssignCallBack()
        {
            hookGC(gcCallback);
        }

        public static void callGC()
        {
            Console.WriteLine("Gcing ....");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}