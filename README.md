# ScreenlyyID .NET Core Blazor WASM SDK

This document provides detailed information about the ScreenlyyID .NET Core Blazor WASM. The .NET Core SDK enables developers to integrate image capture and processing functionality in their mobile web applications.


## Supported Devices
- Global requirement: To work with the SDK, the device/browser must support WASM/WebAssembly.

- Android: All devices supported. 

- iOS devices still supported by Apple: All devices supported. 

- iOS devices no longer supported by Apple: Not officially supported, however testing does show capture and verification work.


## Overview

The SDK is made up of 2 processing parts:

1. Javascript that opens the camera, processes the document image, crops it, and assesses it for blur and glare. It also opens the front camera for the selfie and crops the selfie image.

2. A set of API calls that take the processed image from step 1, and perform OCR extraction, validation, facematch and liveness.

## Javascript Modules

This section describes step 1 in the Overview. The javascript that is used to crop and detect image quality.

It makes use of a 3rd party javascript library to analyze the captured image quality and crop the image for processing. These are contained in the /public folder.

**Acuant JavaScript SDK (AcuantJavaScriptSdk.min.js):**

 - Main script used to call all the other parts of the SDK

**Acuant Camera (AcuantCamera.min.js/AcuantCamera.js)**

- Document Library to detect documents, crop, calculate sharpness and glare

**Acuant Passive Liveness (AcuantPassiveLiveness.min.js)**

- Performs selfie and face detection for liveness and facematch

**Acuant Initializer Worker (AcuantInitializerWorker.min.js, AcuantInitializerService.js, AcuantInitializerService.wasm)**

- WASM-based Web Worker used to initialize the SDK
- Used by the Implementer via Acuant JavaScript SDK

**Acuant Image Worker (AcuantImageWorker.min.js, AcuantImageService.js, AcuantImageService.wasm):**
- WASM-based Web Worker used to run Cropping and Document Detection
- Used by Acuant Camera or by the Implementer via Acuant JavaScript SDK

**Acuant Metrics Worker (AcuantMetricsWorker.js, AcuantMetricsService.js, AcuantMetricsService.wasm):**

- Web Worker used to run image quality metrics like sharpness and glare
- Used by Acuant Image Worker after cropping or by the Implementer via Acuant JavaScript SDK


## Setup

The below describes the steps to create your own SDK. These steps are already implemeted in this project and can be used as a reference guide as to the setup of the SDK.

1. Add the Javascript and WASM files to the /public folder of your application.

2. Load the main script files in index.html;

```
    <script  src="/AcuantJavascriptWebSdk.min.js" ></script>
    <script  src="/AcuantCamera.js" ></script>
    <script  src="/AcuantPassiveLiveness.min.js"></script>
```
3. Define a custom path to load files (if different than root). This can be seen in SdkConfig.js

```
const acuantConfig = {
   path: "/",
   jpegQuality: 1.0 //strongly advised not to modify (can be left out will default to 1.0)
};
```

4. Define a callback before the script tag in step 2. This is an optional global JavaScript function that is executed after WASM is loaded. Also in SdkConfig.js

```
 var onAcuantSdkLoaded = function() {
    //sdk has been loaded;
  }
  ```

  Note: The SDK loads using a listener for DOMContentLoaded. If the scripts will be added to the page in a way that the listener won't be called (for example, in a single-page react application), once the SDK scripts are loaded in the page, manually call the following function:

  ```
  loadAcuantSdk();
  ```

  5. Define a method as a callback for unexpected errors in situations where one of the other error callbacks could not be called. This callback should rarely, if ever, be called. If the callback is getting called, review the implementation as it more often than not indicates a flaw in the implementation.

  ```
  AcuantJavascriptWebSdk.setUnexpectedErrorCallback((error) => {
    //handle the error
  });
  ```


## Initialize and Start the SDK

1. The Javascript cropping SDK is initialized in wwwroot/js/base.js:

**Note:** The credentials passed here are arbitrary and are just dummy values required for the initialization process.

```
export async function init() {
    AcuantJavascriptWebSdk.initialize(base64Token, Credential.acas_endpoint, {
        onSuccess: function () { ... }
    }
```

## Manual Capture

This is the default way to open the camera and capture an image, and crop and detect blur/glare. 

All of the below code snippets can be found in the wwwroot/js/base.js and /Pages/Index.razor.cs

**Prerequisite:** Initialize the SDK

- This opens the device's native camera app or a file explorer on desktop. It does not provide a frame analysis or document detection on screen - which for lower end devices is not recommneded anyway. It does process the image after capture.

```
index.razor.cs:
  private async Task StartCamera()
    {
        await AppInsights.TrackEvent($"Button Click - Start Manual Capture");
        await JS.InvokeVoidAsync("startManualCapture");
    }

base.js:
    function startManualCapture() {
        AcuantCamera.startManualCapture(cameraCallback);
    }

```

 After the document image is captured, it is automatically processed with crop, sharpness, glare, and barcode scan. It will fire the methods defined in the cameraCallback:

 ```
 base.js;
    const cameraCallback = {
        onCaptured: onCaptured,
        onCropped: onCropped,
        // eslint-disable-next-line no-unused-vars
        onFrameAvailable: function (response) {
        }
    }
```

**Cropping, Sharpness, Glare**

The processed image and data are returned via the camera onCropped callback. The image can be used to verify the crop, sharpness, and glare of the image, and then upload the document

Here is the response from the onCropped callback. You can see these values being checked in the onCropped function.

```
response = {
  image: { 
    data: String,
    bytes: ByteArray,
    width: Number,
    height: Number,
    barcodeText: String,
  }, 
  glare: Number, 
  sharpness: Number,
  cardType: Number, //define card type, None = 0, ID = 1, Passport = 2
  dpi: Number
}
```

If the sharpness value is greater than 50, then the image is considered sharp (not blurry). If the glare value is 100, then the image does not contain glare. If the glare value is 0, then image contains glare. When image is obtained and has passed metrics, it is ready for upload to the API.


## API Structure

Once document has been cropped by the Javascript SDK, it can be passed to the API for assessment. The APIs must be called in the following order;

1. **Create Document Instance** - call this first to create a unique token for your document (note this is different to the authentication instance API).
2. **Document Image** - post the cropped document image to this end point using the identifier in step 1.
3. **Classify Document** - classify the type of document, this will tell you if you need to also capture the back of the document, or you can continue to selfie. If you need to capture the back of the document, just repeat steps 2 and 3.
4. **Get Document Image Field** - this is an optional step - which allows you to retrieve the passport size photo or the signature, if identified, from the document. You can call this to save the image for later use if you require it.
5. **Get Document Analysis** - this is the call that will analyse the document and return you all the data points and any issues found with the document.

It is recommended you thoroughly read the [API documentation](https://admin.screenlyyid.com/docs/api) to understand these calls as well as API Authentication in detail.

## Project Setup

For local development setup.

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur) recommended.

Node 20 is recommended.

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Compile and Minify for Production

```sh
npm run build
```

### Run Unit Tests with [Vitest](https://vitest.dev/)

```sh
npm run test:unit
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```
