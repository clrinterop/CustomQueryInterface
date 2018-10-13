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
    [Guid("4C55877F-7F27-42d0-898B-DEAC51F34DE6")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISing
    {
        void SingMethod(int i, string str);
    }

    [ComVisible(true)]
    [Guid("4C55877F-7F27-42d0-898B-DEAC51F34DD6")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDance
    {
        void DanceMethod(int i, string str);
    }


    [ComVisible(true)]
    [Guid("4C55877F-7F27-42d0-898B-DEAC51F34DF6")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBar
    {
        void BarMethod();
    }

    public class Bar : ISing, IBar, IDance
    {
        public void BarMethod()
        {
            Console.WriteLine("Hook the GC call back");
            Utility.AssignCallBack();
        }

        public void SingMethod(int i, string str)
        {
            Console.WriteLine("Hello Sing! the pass in arg is " + i);
            Console.WriteLine(str);
        }

        public void DanceMethod(int i, string str)
        {
            Console.WriteLine("Hello Dance! the pass in arg is " + i);
            Console.WriteLine(str);
        }

    }
}