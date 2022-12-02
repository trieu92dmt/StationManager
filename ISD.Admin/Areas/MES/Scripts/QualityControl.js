$(document).on("click", ".btn-qc", function () {
    $btn = $(this);
    $.ajax({
        type: "GET",
        url: "/MES/QualityControl/Edit/" + $btn.data("id"),
        success: function (jsonData) {
            $btn.button('reset');
            if (jsonData.Success === false) {
                //lỗi => Báo lỗi
                alertPopup(false, jsonData.Data);
            } else {
                $("#divFunctionModal").html(jsonData);
                $("#divFunctionModal").modal('show');
                $("select[id='SamplingLevel']").trigger("change");
            }
       
        }, error: function (err) {
            console.log(err.statusText);
            $btn.button('reset');
        }
    });
})

$(document).on("click", ".btn-show-image", function () {
    $btn = $(this);
    var type = $btn.data("type");
    $.ajax({
        type: "GET",
        url: "/MES/QualityControl/_ImageInfo",
        data: {
            id: $btn.data("id"),
            type: type
        },
        success: function (jsonData) {
            if (jsonData.Success === false) {
                //lỗi => Báo lỗi
                alertPopup(false, jsonData.Data);
            } else {
                $("#divImageModal").html(jsonData);
                $("#divImageModal").modal('show');
            }
            $btn.button('reset');
        }, error: function (err) {
            console.log(err.statusText);
            $btn.button('reset');
        }
    });
})

$(document).on("change", "#SamplingLevel", function () {
    $btn = $(this);
    var value = $btn.val();
    if (value === 'OTHER') {
        $(".hidden-samplinglevel").removeClass("hidden");
        $(".hidden-samplinglevel input").removeAttr("disabled");
    } else {
        if (!($(".hidden-samplinglevel").hasClass("hidden"))) {
            $(".hidden-samplinglevel").addClass("hidden");
            $(".hidden-samplinglevel input").Attr("disable", "disabled");
        }

    }
})

//Xoá thông tin kiểm tra
$(document).on("click", ".btn-remove-info", function () {
    $btn = $(this);
    if ($btn.data("id") == null || $btn.data("id") == '' || $btn.data("id") == undefined) {
        $(".row-info-" + $btn.data("row")).remove();
    } else {
        if (confirm('Bạn có chắc chắn muốn xoá thông tin kiểm tra này?')) {
            $.ajax({
                type: "POST",
                url: "/MES/QualityControl/DeleteInfo/" + $btn.data("id"),
                contentType: 'application/json',
                success: function (jsonData) {
                    if (jsonData.Success === true) {
                        $(".row-info-" + $btn.data("row")).remove();
                        alertModalPopup(true, jsonData.Data);
                    } else {
                        alertModalPopup(false, jsonData.Data);
                    }
                }, error: function (err) {
                    alertModalPopup(false, err.statusText);
                }
            });
        }

    }

})

//Thêm thông tin kiểm tra
$(document).on("click", ".btn-add-row-info", function () {
    $btn = $(this);
    var frm = $("#frmEdit"),
        formData = new FormData(),
        formParams = frm.serializeArray();

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });
    formData.append("WorkCenterCode", $("#divFunctionModal #WorkCenterCode").val());
    $.ajax({
        type: "POST",
        url: "/MES/QualityControl/AddRowInfo/",
        data: formData,
        processData: false,
        contentType: false,
        success: function (jsonData) {
            if (jsonData.Success === false) {
                alertModalPopup(false, jsonData.Data);
            } else {
                $("#form-info tbody").append(jsonData)
            }
        }, error: function (err) {
            alertModalPopup(false, err.statusText);
        }
    });
})

//Xoá lỗi
$(document).on("click", ".btn-remove-error", function () {
    $btn = $(this);
    if ($btn.data("id") == null || $btn.data("id") == '' || $btn.data("id") == undefined) {
        $(".row-error-" + $btn.data("row")).remove();
    } else {
        if (confirm('Bạn có chắc chắn muốn xoá lỗi này?')) {
            $.ajax({
                type: "POST",
                url: "/MES/QualityControl/DeleteError/" + $btn.data("id"),
                contentType: 'application/json',
                success: function (jsonData) {
                    if (jsonData.Success === true) {
                        $(".row-error-" + $btn.data("row")).remove();
                        alertModalPopup(true, jsonData.Data);
                    } else {
                        alertModalPopup(false, jsonData.Data);
                    }
                }, error: function (err) {
                    alertModalPopup(false, err.statusText);
                }
            });
        }

    }

})

//Thêm lỗi
$(document).on("click", ".btn-add-row-error", function () {
    $btn = $(this);
    var frm = $("#frmEdit"),
        formData = new FormData(),
        formParams = frm.serializeArray();

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });
    $.ajax({
        type: "POST",
        url: "/MES/QualityControl/AddRowError/",
        data: formData,
        processData: false,
        contentType: false,
        success: function (jsonData) {
            if (jsonData.Success === false) {
                alertModalPopup(false, jsonData.Data);
            } else {
                $("#form-error tbody").append(jsonData)
            }
        }, error: function (err) {
            alertModalPopup(false, err.statusText);
        }
    });
})

function alertModalPopup(type, message) {
    if (type == true) {
        setModalMessage("#divModalAlertSuccess", message);
        setTimeout(function () {
            $('#divModalAlertSuccess').show();
        }, 500)
        setTimeout(function () {
            $('#divModalAlertSuccess').hide();
        }, 3000)
    }
    else if (type == false) {
        setModalMessage("#divModalAlertWarning", message);
        setTimeout(function () {
            $('#divModalAlertWarning').show();
        }, 500)
    }
}
function setModalMessage(div, message) {
    if (Array.isArray(message)) {
        var arr = [];
        $.each(message, function (i, item) {
            arr[i] = { err: item }
            $(div + " .modal-alert-message").append("<li>" + arr[i].err + "</li>");
        });
    }
    else {
        $(div + " .modal-alert-message").html(message);
    }
}