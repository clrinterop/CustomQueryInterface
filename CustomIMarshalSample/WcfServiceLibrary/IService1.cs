///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    /// <summary>
    /// WCF service declaration
    /// </summary>
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);
    }
}
