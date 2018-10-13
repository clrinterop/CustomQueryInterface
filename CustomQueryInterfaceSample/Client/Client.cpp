// ==++==
// 
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// 
// ==--==
// *******************************************************************************************
// This client demonstrates managed aggregation by using a native client talk with it.
// ********************************************************************************************

#define _WIN32_COM
#include <stdio.h>
#include <wtypes.h>
#include <atlbase.h>
#include <Windows.h>

#import "CcwServer.tlb" no_namespace named_guids raw_interfaces_only

#define ReleaseIfNotNull(x) if (x) x->Release();

void LateBind(WCHAR * MethodName, IDispatch * pDisp);

typedef void (*Fp_RetVoid)();
Fp_RetVoid fpCall = NULL;
extern "C" __declspec(dllexport) void hookGC(Fp_RetVoid fpInvokeGC)
{
	fpCall = fpInvokeGC;
}

int _tmain(int argc, _TCHAR* argv[])
{
    IUnknown *pUnk = NULL;
    IFoo *pFoo = NULL;
    IBar *pBar = NULL;
	ISing *pSing = NULL;	
	IDance * pDance = NULL;
	IDispatch * pDisp = NULL;
    HRESULT hresult;

	CComBSTR fooStr("I am Foo");
	CComBSTR barStr("I am Bar");	
	CComBSTR singStr("I am Sing");
	CComBSTR danceStr("I am Dance");

    // 0. initialization
    hresult = CoInitializeEx(NULL, COINIT_MULTITHREADED);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot initialze COM: 0x%x\n", hresult);
        return -1;
    }

    // 1. Get the IUnknown interface
    hresult = CoCreateInstance(CLSID_CcwServer, NULL, CLSCTX_INPROC_SERVER, IID_IUnknown, (void **)&pUnk);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Get the IUnkown interface: 0x%x\n", hresult);
        goto cleanup;
    }
	
	// 2. Get the IFoo interface
    hresult = pUnk->QueryInterface(IID_IFoo, (void **)&pFoo);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Convert to IFoo: 0x%x\n", hresult);
        goto cleanup;
    }
    
	
    // 3. Prepare the argument and call the method	
    pFoo->FooMethod(1, fooStr);
	
    // 4.1 Cast to IBar
    hresult = pFoo->QueryInterface(IID_IBar, (void **)&pBar);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Convert to IBar: 0x%x\n", hresult);
        goto cleanup;
    }
	
	pBar->BarMethod();

	// 4.2 Cast to ISing	
    hresult = pFoo->QueryInterface(IID_ISing, (void **)&pSing);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Convert to ISing: 0x%x\n", hresult);
        goto cleanup;
    }

	pSing->SingMethod(3, singStr);

	// 4.3 Cast to IDance
    hresult = pBar->QueryInterface(IID_IDance, (void **)&pDance);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Convert to IDance: 0x%x\n", hresult);
        goto cleanup;
    }

	pDance->DanceMethod(4, danceStr);
    
	// 5. Later Binding, Server should use the Customized IDispatch implementation
	hresult = pUnk->QueryInterface(IID_IDispatch, (void **)&pDisp);
    if (FAILED(hresult))
    {
        printf("ERROR: cannot Convert to IDispatch interface: 0x%x\n", hresult);
        goto cleanup;
    }

	LateBind(L"FooMethod", pDisp);
	LateBind(L"SingMethod", pDisp);
	LateBind(L"DanceMethod", pDisp);	
	LateBind(L"FooMethod", pDisp);
	LateBind(L"NotExistMethod", pDisp);
	

    // 5. cleaning up
cleanup:
    ReleaseIfNotNull(pUnk);
	ReleaseIfNotNull(pFoo);
	ReleaseIfNotNull(pBar);
    ReleaseIfNotNull(pSing);
    ReleaseIfNotNull(pDance);
	ReleaseIfNotNull(pDisp);
	CoUninitialize();

	printf("\n");
	// Note: we use Reverse PInvoke to invoke the GC in clr, this only works in the inproc scenario
    // Make sure the implementation does not cause the leak of reference, all ccws could be GCed
	// First GC should collect the ccwserver(outer ccw) and make the aggregated ccw(inner ccw) dead
	fpCall();
	printf("First GC should collect the BaseOuter and ccwserve\n");
	
	// Aggregated ccw should be GCed after second GC
	fpCall();	
	printf("Second GC should collect the SimpleDispatch\n");
	return 0;
}

void LateBind(WCHAR * methodName, IDispatch * pDisp)
{
	wprintf(L"\n\nLate Binding for method %ls\n", methodName);
	CComBSTR name(methodName);
	DISPID ids[1];
	pDisp -> GetIDsOfNames(IID_NULL, &name, 1, 111, ids);
	wprintf(L"Get DISPID for %ls is %d\n", methodName, ids[0]);
	
	CComVariant result;
	UINT uArgErr = 0;
	
	CComVariant args [2];
	args[1].vt = VT_INT;
	args[1].intVal = 2009;
	args[0].vt = VT_BSTR;
	args[0].bstrVal = SysAllocString(L"Hello from Native world");

	DISPPARAMS dispparams;
    dispparams.rgvarg = args;
    dispparams.rgdispidNamedArgs = NULL;
    dispparams.cArgs = 2;	
    dispparams.cNamedArgs = 0;
	HRESULT hr = pDisp -> Invoke(ids[0], IID_NULL, 11, DISPATCH_METHOD, &dispparams, &result, NULL, &uArgErr);
}