

let liveCaptureFailed = false

const acuantConfig = {
    path: "/js/",
    jpegQuality: 1.0 //strongly advised not to modify (can be left out will default to 1.0)
}
const options = {
    text: {
        NONE: "ALIGN",
        SMALL_DOCUMENT: "MOVE CLOSER",
        GOOD_DOCUMENT: null,//null countdown
        CAPTURING: "CAPTURING",
        TAP_TO_CAPTURE: "TAP TO CAPTURE"
    }
}


let cameraCallback = {
    onCaptured: onCaptured,
    onCropped: onCropped,
    onFrameAvailable: function (response) {
    }
}

//this is an example of how to detect an older ios device.
//depending on your enviroment you might be able to get more specific.
function isOldiOS() {
    let ua = navigator.userAgent;
    let keyPhrase = "iPhone OS";
    const keyPhrase2 = "iPad; CPU OS";
    let index = ua.indexOf(keyPhrase);
    if (index < 0) {
        keyPhrase = keyPhrase2;
        index = ua.indexOf(keyPhrase);
    }
    if (index >= 0) {
        let version = ua.substring(index + keyPhrase.length + 1, index + keyPhrase.length + 3);
        try {
            let versionNum = parseInt(version);
            if (versionNum && versionNum < 13) {
                return true;
            } else {
                return false;
            }
        } catch (_) {
            return false;
        }
    } else {
        return false;
    }
}

function initDone() {
   
}

function init(credentials) {

    let base64Token = btoa(credentials.id_username + ':' + credentials.id_password);
    AcuantJavascriptWebSdk.initialize(base64Token, ' https://api.screenlyyid.com', {
        onSuccess: function () {
            console.log("initialize success");
            if (!isOldiOS()) {
                AcuantJavascriptWebSdk.startWorkers(initDone); //no list of workers to start means it will start all the workers.
            } else {
                AcuantJavascriptWebSdk.startWorkers(initDone, [AcuantJavascriptWebSdk.ACUANT_IMAGE_WORKER]); //old ios devices can struggle running metrics. See readme for more info.
            }
        },
        onFail: function (code, description) {
            console.log("initialize failed " + code + ": " + description);
        }
    });
}

window.onbeforeunload = function (event) {
    end();
};

function startCamera() {
    if (AcuantCamera.isCameraSupported && !AcuantCamera.isIOSWebview && !liveCaptureFailed) {
        AcuantCameraUI.start(cameraCallback, onError, options);
    }
    else {
        startManualCapture()
    }
}


function startManualCapture() {
    AcuantCamera.startManualCapture(cameraCallback);
}

function end() {
    AcuantJavascriptWebSdk.endWorkers();
}

function onCaptured(response) {

    hideElementWithClass('capture-image');
    hideElementWithClass('manual-capture');
    showElementWithClass("analyzing");
    
    hideElementWithClass("front-info-image");
    hideElementWithClass("back-info-image");
    hideElementWithClass('title-text');
}

function onCropped(response, dotNetHelper) {

    if (response && response.image) {
        
        hideElementWithClass('analyzing');
        hideElementWithClass('capture-image');
        hideElementWithClass('manual-capture');
        
        showElementWithClass('result-id-img');
        showElementWithClass('use-image');
        showElementWithClass('retry');
        
        setHeaderText("Use this image?");
        showElementWithClass('title-text');
        
        DotNet.invokeMethodAsync('ScreenlyyIDdotNetSdk', 'SetDocumentImage', response.image.data);
        drawImageOnResult(response);

    } else {
        console.log('Document could not be extracted from photo')

        hideElementWithClass('analyzing');
        hideElementWithClass('capture-image');
        hideElementWithClass('manual-capture');

        showElementWithClass('failed-id-cropping');
    }
    closeCamera();
}

function closeCamera() {
   // if using the acuant embedded camera which is not used here.
}

function onError(err, code) {
    liveCaptureFailed = true;
    startManualCapture();
    console.error(err);
    console.error(code);
}



function drawImageOnResult(result, dotNetHelper) {
    
    let displayCanvas = document.getElementById('result-id-img');
    let displayContext = displayCanvas.getContext("2d");

    let cw = displayCanvas.width;
    let ch = displayCanvas.height;

    let resultImage = new Image();

    resultImage.onload = function () {
        let iw = resultImage.width;
        let ih = resultImage.height;
        let dw = iw;
        let dh = ih;
        if (dw > cw) {
            dw = cw;
            dh = Math.round(dw * ih / iw);
        }
        if (dh > ch) {
            dh = ch;
            dw = Math.round(dh * iw / ih);
        }
        let dx = Math.round((cw - dw) / 2);
        let dy = Math.round((ch - dh) / 2);
        displayContext.drawImage(resultImage, dx, dy, dw, dh);
    };

    resultImage.src = result.image.data;
}

function getImage(dotNetHelper) {
    let canvas = document.getElementById("result-id-img");
    let dataUrl = canvas.toDataURL("image/jpeg");
    dotNetHelper.invokeMethodAsync('ProcessImage', dataUrl);
}

function openFrontCamera(dotNetHelper) {
    console.log('Opening front camera.')
    AcuantPassiveLiveness.startSelfieCapture((image) => {
        onSelfieCaptured(image, dotNetHelper);
    });
}


function onSelfieCaptured(image, dotNetHelper) {
    console.log('Selfie capture callback invoked.');
    console.log('selfie image: ' + image);
    dotNetHelper.invokeMethodAsync('ProcessSelfie', image);
}

function showElementWithClass(id) {
    var elem = document.getElementById(id);
    if (elem != null) {
        elem.classList.remove('hide');
        elem.classList.add('show');
        elem.hidden = false;
    }
}

function hideElementWithClass(id) {
    var elem = document.getElementById(id);
    if (elem != null) {
        elem.classList.remove('show');
        elem.classList.add('hide');
        elem.hidden = true;
    }
}

function setHeaderText(text) {
    var elem = document.getElementById("title-text");
    if (elem != null) {
        elem.textContent = text;
    }
}

function toggleShowHideWithClass(id) {
    var elem = document.getElementById(id);
    if (elem != null) {
        if (elem.classList.contains('show')) hideElementWithClass(id);
        else showElementWithClass(id);
    }
}

function getIpAddress() {
    return fetch('https://jsonip.com/')
        .then((response) => response.json())
        .then((data) => {
            return data.ip
        });
}

function getUserAgent() {
    return navigator.userAgent;
};

function getHost() {
    return window.location.host;
}


