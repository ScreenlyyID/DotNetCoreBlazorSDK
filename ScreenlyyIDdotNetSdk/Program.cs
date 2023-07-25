using BlazorApplicationInsights;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ScreenlyyIDdotNetSdk;
using ScreenlyyIDdotNetSdk.Services.AppWorkflow;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Address;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Device;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Email;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Financial;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Phone;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening.ElectronicIdentity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<IDocumentService, DocumentService>();
builder.Services.AddHttpClient<ILivenessService, LivenessService>();
builder.Services.AddHttpClient<IFacematchService, FacematchService>();
builder.Services.AddHttpClient<IAMLService, AMLService>();
builder.Services.AddHttpClient<IAdditionalDataService, AdditionalDataService>();
builder.Services.AddHttpClient<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddHttpClient<IAddressVerificationService, AddressVerificationService>();
builder.Services.AddHttpClient<IPhoneVerificationService, PhoneVerificationService>();
builder.Services.AddHttpClient<IEIDVerificationService, EIDVerificationService>();
builder.Services.AddHttpClient<IIPDeviceVerificationService, IPDeviceVerificationService>();
builder.Services.AddHttpClient<IBinLookupService, BinLookupService>();
builder.Services.AddHttpClient<IInstanceService, InstanceService>();
builder.Services.AddMatBlazor();
builder.Services.RegisterIntlTelInput();
builder.Services.AddBlazorApplicationInsights();
await builder.Build().RunAsync();