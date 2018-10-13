///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace WcfServiceLibrary
{
    /// <summary>
    /// helper class to dump the information to console.
    /// </summary>
    public class Util
    {
        public static void WriteLineWithThreadId(string msg)
        {
            Console.WriteLine("Thread {0}:\t{1}", Thread.CurrentThread.ManagedThreadId, msg);
        }
    }
}
