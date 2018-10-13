#  CustomQueryInterface Sample

> In the CLR v4 there is a new interface called `ICustomQueryInterface`.
> This very cool new feature will enable developers to provide their
> own managed implementation of custom and standard COM interfaces
> (except `IUnknown`).


## CustomQueryInterface Sample

One interesting scenario, which will be illustrated in the sample below,
is dynamic managed aggregation with a flexible implementation of `IDispatch`.

In this sample, we will show an implementation of `ICustomQueryInterface` through
a managed `IDispatch` implementation that overrides the default `IDispatch`
implementation by the CLR a managed COM aggregation system where both
Outer and Inner objects are managed objects.


## CustomIMarshal Sample

To further demo the power of **CustomQueryInterface** and the ability to use
the .NET technology (WCF) within COM world, this sample targets the customization
of `IMarshal` interface by using WCF.

`IMarshal` enables a COM object to define and manage the marshaling of its interface
pointers. On the other hand, WCF provides serialization facilities that enable loose
coupling, which significantly eases the customization of `IMarshal` and makes it
more flexible.

In this sample, we provide three important things:
* A managed class acts as the COM component which also implements a WCF service.
* A managed class acts as the COM proxy which contains the WCF client.
* A program that demos the usage of the two classes above by passing the `IUnknown` pointer through the global stream.

## License

The **CustomQueryInterface** and **CustomIMarshal** samples are licensed under Microsoft Limited Public License (Ms-LPL).

Ms-LPL is based on Microsoft Public License (Ms-PL), but there is a Windows-platform limitation:
> 4. (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only
> to the software or derivative works that you create that run on a Microsoft Windows
> operating system product.
