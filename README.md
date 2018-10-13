#  CustomQueryInterface Sample

> In the CLR v4 there is a new interface called `ICustomQueryInterface`.
> This very cool new feature will enable developers to provide their
> own managed implementation of custom and standard COM interfaces
> (except `IUnknown`).


One interesting scenario, which will be illustrated in the sample below,
is dynamic managed aggregation with a flexible implementation of `IDispatch`.

In this sample, we will show an implementation of `ICustomQueryInterface` through
a managed `IDispatch` implementation that overrides the default `IDispatch`
implementation by the CLR a managed COM aggregation system where both
Outer and Inner objects are managed objects.


## License

The **CustomQueryInterface** sample is licensed under Microsoft Limited Public License (Ms-LPL).

Ms-LPL is based on Microsoft Public License (Ms-PL), but there is a Windows-platform limitation:
> 4. (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only
> to the software or derivative works that you create that run on a Microsoft Windows
> operating system product.
