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

    [ComVisible(true)]
    [Guid("4C55877F-7F27-42d0-898B-DEAC51F85DE6")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFoo
    {
        void FooMethod(int i, string str);
    }

    public class LazyFoo : IFoo
    {
        public void FooMethod(int i, string str)
        {
            Console.WriteLine("Hello LazyFoo! the pass in arg is " + i);
            Console.WriteLine(str);
        }
    }
}
