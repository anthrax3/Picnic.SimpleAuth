# Picnic.SimpleAuth
Picnic.SimpleAuth is a simple authentication extension for Picnic that provides a slim authentication layer for the Picnic ASP.NET Core CMS Extension.  

## Getting Started

#### Step 1: Add the Picnic.SimpleAuth.Extensions using directive to your Startup.cs
```csharp
using Picnic.SimpleAuth.Extensions;
```

#### Step 2: Add UseSimpleAuth() to your existing AddPicnic() call
```csharp
public void ConfigureServices(IServiceCollection services)
{            
    //...Existing configuration

    services.AddMvc();
    
    // Add Picnic and Specify Json Store
    services.AddPicnic().UseJsonStore().UseSimpleAuth();
}
```

#### Step 3: Launch the Picnic Management Interface

Launch the picnic management interface in the browser and you will be prompted with a login dialog.
The default username is ```admin``` and the default password is ```password```.  Don't forget to change it.  A user menu item will be added to the picnic management interface allowing you to change your password or logout. 

## Where can I get it?
Install from [Nuget](https://www.nuget.org/packages/Picnic.SimpleAuth/) 
```
Install-Package Picnic.SimpleAuth
```

## License, etc.
Picnic.SimpleAuth is copyright © 2017 Matthew Marksbury and other contributors under the MIT license.


## Roadmap
Coming soon.
