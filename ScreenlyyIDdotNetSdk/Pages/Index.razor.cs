using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using ScreenlyyIDdotNetSdk.Constant;
using ScreenlyyIDdotNetSdk.Models;
using ScreenlyyIDdotNetSdk.Services.AppWorkflow;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Address;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Device;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Email;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Financial;
using ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Phone;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening.ElectronicIdentity;


namespace ScreenlyyIDdotNetSdk.Pages;

public partial class Index
{
    /// <summary>
    /// Inject IJS runtime
    /// </summary>
    [Inject]
    protected IJSRuntime JS { get; set; }


    [Inject]
    public IInstanceService InstanceService { get; set; }
    /// <summary>
    /// Inject document service
    /// </summary>
    [Inject]
    public IDocumentService DocumentService { get; set; }

    /// <summary>
    /// Inject document service
    /// </summary>
    [Inject]
    public ILivenessService LivenessService { get; set; }
    [Inject]
    public IFacematchService FacematchService { get; set; }

    [Inject]
    public IAMLService AMLService { get; set; }

    [Inject]
    public IEmailVerificationService EmailVerificationService { get; set; }

    [Inject]
    public IAddressVerificationService AddressVerificationService { get; set; }

    [Inject]
    public IIPDeviceVerificationService IPDeviceVerificationService { get; set; }

    [Inject]
    public IEIDVerificationService EIDVerificationService { get; set; }

    [Inject]
    public IPhoneVerificationService PhoneVerificationService { get; set; }

    [Inject]
    public IAdditionalDataService AdditionalDataService { get; set; }


    [Inject]
    public IBinLookupService BinLookupService { get; set; }
    
    [Inject] private IApplicationInsights AppInsights { get; set; }


    public static PersonalDataViewModel DocumentDataViewModel
    {
        get;
        set;
    }

    /// <summary>
    /// Local variables
    /// </summary>
    private string instanceId = "";
    private string correlationId = "";
    private string apiKey = "";
    private CaptureStep? _captureStep = null;

    private SdKResults sdkResults = new SdKResults();

    private string firstName;
    private string lastName;
    private string dob;
    private string originalImage;
    private static string documentImage;
    private string CaptureImageText { get; set; } = "Capture ID / Passport";
    private string ErrorMessage { get; set; } = string.Empty;

    [Parameter]
    public Guid Id { get; set; }

    private bool IsIDVSelected { get; set; } = true;

    private bool IsStartPage { get; set; } = true;

    [JSInvokable]
    public static async Task SetDocumentImage(string image)
    {
        documentImage = image;
    } 
    

    /// <summary>
    /// Page life cycle initialize event
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //System.Diagnostics.Trace.TraceInformation("Initializing System");
        await RestartProcess();
    }
    
    private async Task RestartProcess()
    {
        await AppInsights.TrackEvent($"Button Click - Restart Process");
        
        correlationId = await InstanceService.GetCorrelationId();
        IsStartPage = true;
        IsIDVSelected = true;
        _captureStep = CaptureStep.Front;
        
        await JS.InvokeVoidAsync("init", new { id_username = "user", id_password = "password" });

        _amlCheckOnlyEditContext = new EditContext(_personalAmlCheckOnlyDataViewModel);
        
        _additionalInformationEditContext = new EditContext(_additionalDataViewModel);
        
        _dataInputEditContext = new EditContext(_dataInputDataViewModel);
        
        await ResetToStart();

        StateHasChanged();
    }
    
    
    /// <summary>
    /// Starting camera
    /// </summary>
    /// <returns></returns>
    private async Task StartCamera()
    {
        await AppInsights.TrackEvent($"Button Click - Start Manual Capture");
        await JS.InvokeVoidAsync("startManualCapture");
    }

    private async Task StartManualCamera() => await JS.InvokeVoidAsync("startManualCapture");

    /// <summary>
    /// Using captured image
    /// </summary>
    /// <returns></returns>
    // private async Task UseImage() => await JS.InvokeVoidAsync("getImage", DotNetObjectReference.Create(this));

    private async Task TakeSelfie()
    {
        await AppInsights.TrackEvent($"Button Click - Take Selfie");
        await JS.InvokeVoidAsync("openFrontCamera", DotNetObjectReference.Create(this));
    }

    /// <summary>
    /// Getting document classification
    /// </summary>
    /// <returns></returns>
    private async Task GetClassification() => await DocumentService.GetClassification(instanceId, correlationId);

    /// <summary>
    /// Acuant JS image capture specific for creating document instance.
    /// You only need this if you are doing idv document scans. You do not need it for aml, eIdv or customer intelligence checks.
    /// </summary>
    /// <returns></returns>
    private async Task CreateDocumentInstance()
    {
        instanceId = await DocumentService.GetInstanceId(correlationId);
        instanceId = instanceId.Substring(1, instanceId.Length - 2);
    }

    /// <summary>
    /// Capture front-side id
    /// </summary>
    /// <returns></returns>
    private async Task CaptureFrontSideId()
    {
        await this.StartCamera();
    }

    /// <summary>
    /// Capture back-side id
    /// </summary>
    /// <returns></returns>
    private async Task CaptureBackSideId() => await this.StartCamera();


    public async Task ReadOriginalImage()
    {
        var imageName = "image-front";
        this.originalImage = await JS.InvokeAsync<string>("localStorage.getItem", new object[] { imageName });
    }

    [JSInvokable]
    public async Task ProcessImage()
    {
        await AppInsights.TrackEvent($"Button Click - Start Image Processing - {_captureStep}");
        try
        {
            switch (_captureStep)
            {
                case CaptureStep.Front:
                    await FrontImageProcessing();
                    break;
                case CaptureStep.Back:
                    await BackImageProcessing();
                    break;
            }
        

            await PostDocumentImage(documentImage);
            var classification = await ClassifyDocument();

            await LogMessageAsync("Document Class = " + classification.Type.Class);
            if (classification.Type.Class == 1) //Passport class
            {
                _captureStep = CaptureStep.Selfie;
            }
            else //if (classification.Type.Class == 2 || classification.Type.Class == 3) //normal ID i.e drivers licence with 2 sides
            {
                if (_captureStep == CaptureStep.Front)
                {
                    await FrontImageProcessed();
                    _captureStep = CaptureStep.Back;
                }
                else if (_captureStep == CaptureStep.Back)
                {
                    //await BackImageProcessed();
                    _captureStep = CaptureStep.Selfie;
                }
            }
            
            
            StateHasChanged();
        }
        catch (Exception e)
        {
            ErrorMessage = $"Exception while processing the {_captureStep.ToString()} of your document. " + e.Message;
            _captureStep = CaptureStep.Error;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
            StateHasChanged();
        }
       

    }


    /// <summary>
    /// Posting image to server, called from js
    /// </summary>
    /// <param name="imageString">Image data</param>
    /// <returns></returns>
    [JSInvokable]
    public async Task PostDocumentImage(string imageString)
    {
        try
        {
            int side = (_captureStep == CaptureStep.Front) ? 0 : 1;

            var result = await DocumentService.PostDocumentImage(instanceId, correlationId, side, imageString);

          //  results.Add(result);
        }
        catch (Exception e)
        {
            ErrorMessage = $"Exception while saving the {_captureStep.ToString()} of your document. " + e.Message;
            _captureStep = CaptureStep.Error;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
            StateHasChanged();
        }

    }

    public async Task<DocumentClassificationResponse> ClassifyDocument()
    {
        DocumentClassificationResponse result = null;
        try
        {
            await LogMessageAsync("Begin: ClassifyDocument()");
            result = await DocumentService.GetClassification(instanceId, correlationId);

            if (result.Type.ClassName.ToLower() == "unknown")
            {
                // there was a problem classifying the document, we will need to restart the process.
                // show the user a messages, and let them decide to restart.
                ErrorMessage =
                    "Classification returned an unknown document type. This may mean the image was blurry or unreadable." +
                    "Please try again, making sure the picture is in focus, and clear of objects or shadows.";
                _captureStep = CaptureStep.Error;
                
            }

            sdkResults.DocumentType = result.Type.ClassName;
            
            await LogMessageAsync("End: ClassifyDocument()");
            return result;
            //TOD we need to also account for passport and IDs that done have a "back"
        }
        catch (Exception e)
        {
            ErrorMessage = $"Exception while classifying  the {_captureStep.ToString()} of your document. " + e.Message;
            _captureStep = CaptureStep.Error;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
            StateHasChanged();
        }

        return result;

        //results.Add(result);
    }

    [JSInvokable]
    public async Task ProcessSelfie(string selfie)
    {
        try
        {
            await LogMessageAsync("Begin: ProcessSelfie");
            await ProcessSelfie();
            await LogMessageAsync("End: ProcessSelfie");

            await LogMessageAsync("Begin: GetDocument");
            var documentData = await DocumentService.GetDocument(correlationId,
                    instanceId);  
            await LogMessageAsync("End: GetDocument");
 
            await LogMessageAsync("Begin: Extract VIZ Address");
            sdkResults.Address = documentData.DataFields.FirstOrDefault(df =>
                df.Key.Equals("VIZ Address", StringComparison.InvariantCultureIgnoreCase))?.Value;
            await LogMessageAsync("End: Extract VIZ Address");

            await LogMessageAsync("Begin: Extract Document Number");
            sdkResults.DocumentNumber = documentData.DataFields.Where(x => x.Name == "Document Number")
                .Select(x => x.Value).DefaultIfEmpty(string.Empty).First();
            await LogMessageAsync("End: Extract Document Number");

            sdkResults.DocumentType = documentData.Classification.Type.Name == null ? "Undetermined" : documentData.Classification.Type.Name;
            sdkResults.DateOfBirth = documentData.Biographic.BirthDate == null ? "Undetermined" : documentData.Biographic.BirthDate.Value.ToLongDateString();
            sdkResults.NameOnID = documentData.Biographic.FullName == null ? "Undetermined": documentData.Biographic.FullName;

            await LogMessageAsync("Begin: Create PersonalDataViewDataInputStepModel");
            _dataInputDataViewModel = new PersonalDataViewDataInputStepModel()
            {
                FirstName = documentData.DataFields.Where(x => x.Name == "Given Name").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                LastName = documentData.DataFields.Where(x => x.Name == "Surname").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                DateOfBirth = documentData.Biographic.BirthDate, //DateOnly.FromDateTime(documentData.Biographic.BirthDate) ,
                DocumentExpiryDateTime = documentData.Biographic.ExpirationDate, // DateOnly.FromDateTime(documentData.Biographic.ExpirationDate),
                DocumentNumber = documentData.DataFields.Where(x => x.Name == "Document Number").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                Gender = ""
            };
            await LogMessageAsync("End: Create PersonalDataViewDataInputStepModel");

            await LogMessageAsync("Begin: Create PersonalDataViewDataAdditionalStepModel");
            _additionalDataViewModel = new PersonalDataViewDataAdditionalStepModel()
            {
                Email = "",
                AddressLine1 = documentData.DataFields.Where(x => x.Name == "Address Line 1").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(), // .FirstOrDefault().Value,
                AddressLine2 = documentData.DataFields.Where(x => x.Name == "Address Line 2").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                City = documentData.DataFields.Where(x => x.Name == "Address City").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                State = documentData.DataFields.Where(x => x.Name == "Address State").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                ZipCode = documentData.DataFields.Where(x => x.Name == "Address Postal Code").Select(x => x.Value).DefaultIfEmpty(string.Empty).First(),
                CountryCode = "",
            };
            await LogMessageAsync("End: Create PersonalDataViewDataAdditionalStepModel");

            await LogMessageAsync("Begin: GetDocumentImageField()");
            // get the ID data and display it for edit.
            var idPhoto = await DocumentService.GetDocumentImageField(Constant.Constant.DOCUMENT_PHOTO_KEY, correlationId, instanceId);
            sdkResults.Photo = "data:image/jpg;base64," + idPhoto;
            await LogMessageAsync("End: GetDocumentImageField()");
  
            await LogMessageAsync("Begin: LivenessService.ProcessLiveness()");
            var livenessRes = await LivenessService.ProcessLiveness(selfie, correlationId);
            sdkResults.LivenessScore = livenessRes.LivenessResult.Score.ToString() + "%";
            await LogMessageAsync("End: LivenessService.ProcessLiveness()");

            await LogMessageAsync("Begin: FacematchService.ProcessFaceMatch()");
            var facematchRes = await FacematchService.ProcessFaceMatch(selfie, idPhoto, correlationId);
            sdkResults.FacematchScore = facematchRes.Score.ToString() + "%";
            await LogMessageAsync("Begin: FacematchService.ProcessFaceMatch()");
            
            await LogMessageAsync("Begin: GetSignature()");
            var idSignature = await GetSignature();
            if (!string.IsNullOrEmpty(idSignature))
            {
                sdkResults.Signature = "data:image/jpg;base64," + idSignature;
            }
            await LogMessageAsync("End: GetSignature()");
            
            await LogMessageAsync("Begin: Set CaptureStep.DataInput");
            _captureStep = CaptureStep.DataInput;
            await LogMessageAsync("End: Set CaptureStep.DataInput");
            
            StateHasChanged();
        }
        catch (Exception e)
        {
            _captureStep = CaptureStep.Error;
            ErrorMessage = $"An Exception occured while processing facial recognition and liveness test. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
            StateHasChanged();
        }
    }

    private async Task<string> GetSignature()
    {
        var signatureImage = string.Empty;
        try
        {
            signatureImage = await DocumentService.GetDocumentImageField(Constant.Constant.DOCUMENT_SIGNATURE_KEY, correlationId, instanceId);
        }
        catch (Exception e)
        {
            // Its ok for this to fail silently as not all documents have a signature
        }

        return signatureImage;
    }

    private async Task CallAMLCheckAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("aml"))
        //         return;

        await AMLService.CompleteAMLCheck(DocumentDataViewModel, correlationId);
    }

    private async Task CallEmailVerificationAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("EMV"))
        //     return;

        try
        {
            await EmailVerificationService.VerifyAsync(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing Email Verification. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
       
        }
        
    }

    private async Task CallAddressCleanseAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("ADV"))
        //     return;

        try
        {
            await AddressVerificationService.AddressCleansePlusAsync(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing address cleanse. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }
        
    }

    private async Task CallPhoneVerificationAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("PHV"))
        //     return;

        try
        {
            await PhoneVerificationService.ValidateAsync(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing email verification. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }

        try
        {
            await PhoneVerificationService.HLRLookupAsync(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing email HLR lookup. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }

       
    }

    private async Task CallIPDeviceVerificationAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("IPV"))
        //     return;

        var host = await JS.InvokeAsync<string>("getHost").ConfigureAwait(true);
        var userAgent = await JS.InvokeAsync<string>("getUserAgent").ConfigureAwait(true);
        var ipAddress = await JS.InvokeAsync<string>("getIpAddress").ConfigureAwait(true);


        try
        {
            await IPDeviceVerificationService.HostReputationAsync(host, correlationId);
            await IPDeviceVerificationService.IPBlocklistAsync(ipAddress, correlationId);
            await IPDeviceVerificationService.IPProbeAsync(ipAddress, correlationId);
            await IPDeviceVerificationService.UALookupAsync(userAgent, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing IP Device lookup. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }
        
      
    }


    private async Task CallEIDV1VerificationAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("eidv1"))
        //     return;

        try
        {
            await EIDVerificationService.EIDVerify1X1Async(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing EIDV1 checks. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }
        
        
    }

    private async Task CallEIDV2VerificationAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("eidv2"))
        //     return;

        try
        {
            await EIDVerificationService.EIDVerify2X2Async(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing EIDV2 checks. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }
       
    }

    private async Task CallBinLookupAsync()
    {
        // if (!Workflow.ScansAllowed.Contains("BIN"))
        //     return;

        try
        {
            await BinLookupService.LookupAsync(DocumentDataViewModel, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing BIN lookup. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }
       
    }



    public async Task CompleteNonIDVCheck(PersonalDataViewModel personalDataViewModel)
    {
        try
        {
            await AdditionalDataService.SaveAdditionalData(personalDataViewModel, instanceId, correlationId);
        }
        catch (Exception e)
        {
            ErrorMessage = $"An Exception occured while processing additional data. The error message is: " + e.Message;
            await LogErrorAsync(ErrorMessage + " Stack Trace: " + e);
        }

        var tasks = new List<Task>
        {
            CallAMLCheckAsync(),
        
            CallEmailVerificationAsync(),
        
            CallAddressCleanseAsync(),
        
            CallPhoneVerificationAsync(),
        
            CallIPDeviceVerificationAsync(),
        
            CallEIDV1VerificationAsync(),
        
            CallEIDV2VerificationAsync(),
        
            CallBinLookupAsync(),
        };
        
        await Task.WhenAll(tasks);

        await HideCoverSpinner();
        _captureStep = CaptureStep.Complete;
        StateHasChanged();
    }

    
    private async Task ResetToStart()
    {
        
        await JS.InvokeVoidAsync("showElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("showElementWithClass", "capture-image");
        await JS.InvokeVoidAsync("showElementWithClass", "manual-capture");
        await JS.InvokeVoidAsync("showElementWithClass", "title-text");
        
        await JS.InvokeVoidAsync("hideElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");
        await JS.InvokeVoidAsync("hideElementWithClass", "use-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "retry");
        await JS.InvokeVoidAsync("hideElementWithClass", "title-text");
    }
    
    private async Task FrontImageProcessing()
    {
        await JS.InvokeVoidAsync("showElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");
        await JS.InvokeVoidAsync("hideElementWithClass", "use-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "retry");
        await JS.InvokeVoidAsync("hideElementWithClass", "title-text");
    }

    private async Task FrontImageProcessed()
    {
        await JS.InvokeVoidAsync("hideElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");

        await JS.InvokeVoidAsync("showElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("showElementWithClass", "capture-image");
        await JS.InvokeVoidAsync("showElementWithClass", "manual-capture");
        await JS.InvokeVoidAsync("showElementWithClass", "title-text");
        await JS.InvokeVoidAsync("setHeaderText", "Capture a clear image of the back of your ID");
    }

    private async Task BackImageStart()
    {
        await JS.InvokeVoidAsync("hideElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("showElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");
    }

    private async Task BackImageProcessing()
    {
        await JS.InvokeVoidAsync("showElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");
        await JS.InvokeVoidAsync("hideElementWithClass", "title-text");
        await JS.InvokeVoidAsync("hideElementWithClass", "use-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "retry");

        await JS.InvokeVoidAsync("hideElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "capture-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "manual-capture");
    }

    private async Task BackImageProcessed()
    {
        await JS.InvokeVoidAsync("hideElementWithClass", "loader");
        await JS.InvokeVoidAsync("hideElementWithClass", "front-info-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "result-id-img");

        await JS.InvokeVoidAsync("hideElementWithClass", "back-info-image");
        await JS.InvokeVoidAsync("hElementWithClass", "capture-image");
        await JS.InvokeVoidAsync("showElementWithClass", "manual-capture");
        await JS.InvokeVoidAsync("showElementWithClass", "use-image");
        await JS.InvokeVoidAsync("showElementWithClass", "retry");
    }

    private async Task ProcessSelfie()
    {
        await JS.InvokeVoidAsync("showElementWithClass", "analyzing");
        await JS.InvokeVoidAsync("hideElementWithClass", "selfie-image");
        await JS.InvokeVoidAsync("hideElementWithClass", "btn-capture-selfie");
        await JS.InvokeVoidAsync("hideElementWithClass", "selfie-title-text");
    }

    private async Task ShowCoverSpinner()
    {
        await JS.InvokeVoidAsync("showElementWithClass", "cover-spin");
    }

    private async Task HideCoverSpinner()
    {
        await JS.InvokeVoidAsync("hideElementWithClass", "cover-spin");
    }
    
    private async Task LogErrorAsync(string message)
    {
        await this.JsRuntime.InvokeVoidAsync("console.error", message);
    }
    
    // private static async Task LogErrorStaticAsync(string message)
    // {
    //     await JsRuntime.InvokeVoidAsync("console.error", message);
    // }
    
    private async Task LogMessageAsync(string message)
    {
        await this.JsRuntime.InvokeVoidAsync("console.log", message);
    }



}

public class SdKResults
{
    public string Photo { get; set; }
    public string Signature { get; set; }
    public string NameOnID { get; set; }
    public string? DocumentNumber { get; set; }
    public string DocumentType { get; set; }
    public string DocumentExpiry { get; set; }
    public string DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string LivenessScore { get; set; }
    public string FacematchScore { get; set; }
}

