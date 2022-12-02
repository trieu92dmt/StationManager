var defaultReader, readerAutoClosed = false;
var logMsgElement, openButton, closeButton;
var hidden, visibilityChange;
$(document).ready(function () {
    logMsgElement = document.getElementById("logMsg");
    // Check whether the browser supports detection of the web page visibility.
    if (typeof document.webkitHidden !== "undefined") { // Android 4.4 Chrome
        hidden = "webkitHidden";
        visibilityChange = "webkitvisibilitychange";
    }
    else if (typeof document.hidden !== "undefined") { // Standard HTML5 attribute
        hidden = "hidden";
        visibilityChange = "visibilitychange";
    }

    if (hidden && typeof document.addEventListener !== "undefined" &&
        typeof document[hidden] !== "undefined") {
        // Add an event listener for the visibility change of the web page.
        document.addEventListener(visibilityChange, handleVisibilityChange, false);
    }
});
// After BarcodeReader object is created we can configure our symbologies and add our event listener
function onBarcodeReaderComplete(result) {
    if (result.status === 0) {
        // BarcodeReader object was successfully created
        logMsgElement.innerHTML = "<b>Log:</b><br>BarcodeReader object successfully created";
        updateUI(true, true);

        // Configure the symbologies needed. Buffer the settings and commit them at once.
        defaultReader.setBuffered("Symbology", "Code39", "Enable", "true", onSetBufferedComplete);
        defaultReader.setBuffered("Symbology", "Code128", "EnableCode128", "true", onSetBufferedComplete);
        defaultReader.commitBuffer(onCommitComplete);

        // Add an event handler for the barcodedataready event
        defaultReader.addEventListener("barcodedataready", onBarcodeDataReady, false);
        // Add an event handler for the window's beforeunload event
        window.addEventListener("beforeunload", onBeforeUnload);
    }
    else {
        defaultReader = null;
        logMsgElement.innerHTML += "<p style=\"color:red\">Failed to create BarcodeReader, status: " +
            result.status + ", message: " + result.message + "</p>";
        alert('Failed to create BarcodeReader, ' + result.message);
    }
}

// Verify the symbology configuration
function onSetBufferedComplete(result) {
    if (result.status !== 0) {
        logMsgElement.innerHTML += "<p style=\"color:red\">setBuffered failed, status: " +
            result.status + ", message: " + result.message + "</p>";
        logMsgElement.innerHTML += "<p>Family=" + result.family + " Key=" + result.key +
            " Option=" + result.option + "</p>";
    }
}

function onCommitComplete(resultArray) {
    if (resultArray.length > 0) {
        for (var i = 0; i < resultArray.length; i++) {
            var result = resultArray[i];
            if (result.status !== 0) {
                logMsgElement.innerHTML += "<p style=\"color:red\">commitBuffer failed, status: " +
                    result.status + ", message: " + result.message + "</p>";
                if (result.method === "getBuffered" || result.method === "setBuffered") {
                    logMsgElement.innerHTML += "<p>Method=" + result.method +
                        " Family=" + result.family + " Key=" + result.key +
                        " Option=" + result.option + "</p>";
                }
            }
        } //endfor
    }
}


function updateUI(readerOpened, clearData) {
    //openButton.disabled = readerOpened;
    //closeButton.disabled = !readerOpened;
}

/**
 * If the browser supports visibility change event, we can close the
 * BarcodeReader object when the web page is hidden and create a new
 * instance of the BarcodeReader object when the page is visible. This
 * logic is used to re-claim the barcode reader in case another
 * application has claimed it when this page becomes hidden.
 */
function handleVisibilityChange() {
    if (document[hidden]) // The web page is hidden
    {
        closeBarcodeReader(true);
    }
    else // The web page is visible
    {
        if (readerAutoClosed) {
            // Note: If you switch to another tab and quickly switch back
            // to the current tab, the following call may have no effect
            // because the BarcodeReader may not be completely closed yet.
            // Once the BarcodeReader is closed, you may use the Open Reader
            // button to create a new BarcodeReader object.
            openBarcodeReader();
        }
    }
}

function openBarcodeReader() {
    if (!defaultReader) {
        defaultReader = new BarcodeReader(null, onBarcodeReaderComplete);
    }
}

function closeBarcodeReader(isAutoClose) {
    if (defaultReader) {
        readerAutoClosed = isAutoClose;
        defaultReader.close(function (result) {
            if (result.status === 0) {
                logMsgElement.innerHTML += "<p style=\"color:blue\">BarcodeReader successfully closed.</p>";
                defaultReader = null;
                updateUI(false, false);
                window.removeEventListener("beforeunload", onBeforeUnload);
            }
            else {
                logMsgElement.innerHTML += "<p style=\"color:red\">Failed to close BarcodeReader, status: " +
                    result.status + ", message: " + result.message + "</p>";
            }
        });
    }
}

function openButtonClicked() {
    openBarcodeReader();
}

function closeButtonClicked() {
    closeBarcodeReader(false);
}

function onBeforeUnload(e) {
    var message = "Please close BarcodeReader before leaving this page.";
    (e || window.event).returnValue = message;
    return message;
}

function setMessage(div, message) {
    if (Array.isArray(message)) {
        var arr = [];
        $.each(message, function (i, item) {
            //Code cũ
            //arr[i] = { err: item.ErrorMessage }
            if (item.ErrorMessage != undefined && item.ErrorMessage != "") {
                arr[i] = { err: item.ErrorMessage }
            }
            else {
                arr[i] = { err: item }
            }
            $(div + " .alert-message").append("<li>" + arr[i].err + "</li>");
        });
    }
    else {
        $(div + " .alert-message").html(message);
    }
}

function alertPopup(isSuccess, message) {
    if (isSuccess == true) {
        $("#divAlertSuccess .alert-message").html("");
        setMessage("#divAlertSuccess", message);

        $('#divAlertSuccess').show();
        setTimeout(function () {
            $('#divAlertSuccess').hide();
        }, 5000)
    }
    else if (isSuccess == false) {
        $("#divAlertWarning .alert-message").html("");
        setMessage("#divAlertWarning", message);
        $('#divAlertWarning').show();
    }

    $("html, body").animate({ scrollTop: 0 }, "fast");
}

///get value data in qrcode, field = T1
function GetValueFieldInData(data, field) {
    let startField = data.indexOf('<' + field + '>') + 4;
    let endField = data.indexOf('</' + field + '>');

    let value = data.substring(startField, endField);
    return value;
}

//Giải mã UTF-8
function decode_utf8(s) {
    return decodeURIComponent(escape(s));
}

//Mã hoá UTF-8
function encode_utf8(s) {
    return unescape(encodeURIComponent(s));
}

