// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==
using System;
using System.Runtime.InteropServices;
using ManagedAggregation;
using SampleCcws;

[Guid("10000000-A970-11d2-8B5A-00A000000001"), ComVisible(true)]
public class CcwServer : BaseOuter
{
    public CcwServer(): base()
    {
        this.Aggregate(typeof(IFoo), typeof(LazyFoo));        
        this.Aggregate(typeof(IBar), typeof(Bar));
        this.Aggregate(typeof(ISing), typeof(Bar));
        this.Aggregate(typeof(IDance), typeof(Bar));
        this.Aggregate(typeof(IDispatch), typeof(SimpleDispatch));
    }

    ~CcwServer()
    {
        Console.WriteLine("CcwServer is finalized");
    }
}