//Chuyển công đoạn
$(document).on("click", "#btn-save-switching-stages", function (e) {
    switchingStages($(this));
});
//Ghi nhận sản lượng
$(document).on("click", "#btn-save-record-production", function (e) {
    var $btn = $(this);
    loading2();
    $btn.button("loading");
    var frm = $('#frm-record-production')
        , formData = new FormData()
        , formParams = frm.serializeArray()

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });

    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_RecordProduction",
        data: formData,
        dataType: "json",
        processData: false,
        contentType: false,
        success: function (xhr, status, error) {
            if (xhr.Success === true) {
                //Báo thành công + tắt popup
                ISD.setMessage("#divAlertSuccess", xhr.Data);
                $('#divAlertSuccess').show();
                //tat popup
                $('#divFunctionModal').modal('hide');
                //gọi lại nút search sau khi cập nhật thành công
                if ($('#btn-search').length) {
                    $('#btn-search').trigger('click');
                }
            } else {
                //Báo lỗi trên popup
                ISD.setMessage("#divAlertWarningTask", xhr.Data);
                $('#divFunctionModal #divAlertWarningTask').show();
            }
            $btn.button('reset');
        },
        error: function (xhr, status, error) {
            $('.alert-message').html(xhr.Data);
            $('#divFunctionModal #divAlertWarningTask').show();
            $btn.button('reset');
        }
      
    });
});
//Xác nhận hoàn tất công đoạn lớn
$(document).on("click", "#btn-save-confirm-cdl", function (e) {
    var $btn = $(this);
    loading2();
    $btn.button("loading");
    var frm = $('#frm-confirm-cdl')
        , formData = new FormData()
        , formParams = frm.serializeArray()

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });

    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_ConfirmWorkCenter",
        data: formData,
        dataType: "json",
        processData: false,
        contentType: false,
        success: function (xhr, status, error) {
            if (xhr.Success === true) {
                //Báo thành công + tắt popup
                ISD.setMessage("#divAlertSuccess", xhr.Data);
                $('#divAlertSuccess').show();
                //tat popup
                $('#divFunctionModal').modal('hide');
                //gọi lại nút search sau khi cập nhật thành công
                if ($('#btn-search').length) {
                    $('#btn-search').trigger('click');
                }
            } else {
                //Báo lỗi trên popup
                ISD.setMessage("#divAlertWarningTask", xhr.Data);
                $('#divFunctionModal #divAlertWarningTask').show();
            }
            $btn.button('reset');
        },
        error: function (xhr, status, error) {
            $('.alert-message').html(xhr.Data);
            $('#divFunctionModal #divAlertWarningTask').show();
            $btn.button('reset');
        }

    });
});
//Load dữ liệu đã có trong DB vào bảng ghi nhận sản lượng
function loadingRouting() {
    loading2();
    $form = $('#frmRoutingSearchPopup').serializeArray();
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_RoutingSearchResult",
        data: $form
    }).done(function (html) {
        $('#frmRoutingSearchResultPopup #table-routing').html("");
        $('#frmRoutingSearchResultPopup #table-routing').html(html);
    });;
}
//Load danh sách routing vào popup _RecordProduction
//function loadTableSearchRoutingTable() {
//    var id = $('#TaskId').val();
//    $.ajax({
//        type: "Get",
//        url: "/MES/ProductionManagement/_RecordProductionDetail",
//        data: {
//            TaskId: id
//        }
//    }).done(function (html) {
//        $('.div-product-details-history').html(html);
//    });;
//};

//Load dữ liệu từ routing để người dùng lựa chọn
$(document).on("click", ".load-Routing", function () {
    $btn = $(this);
    $btn.button('loadding')
    loading2();
    var $form = $('#frm-record-production').serializeArray();
    $.ajax({
        type: "POST",
        data: $form,
        url: '/MES/ProductionManagement/_RoutingSearch'
    }).done(function (html) {
        $("#divRoutingSearch").html("");
        $("#divRoutingSearch").html(html);
        $('#divRoutingSearch').modal('show');
        $btn.button('reset')
    })
});

//submit dữ liệu được chọn
$(document).on("click", "#btn-Routing-submit-Result", function (e) {
    loading2();
    e.preventDefault();
    var $btn = $(this);
    $btn.button('loadding')
    //var frmRoutingSearchResultPopup = $('#frmRoutingSearchResultPopup').serializeAnything();
    //var frmRecordProduction = $('#frm-record-production').serializeAnything();
    //const target = {};
    //$.extend(target, frmRoutingSearchResultPopup, frmRecordProduction);
    var frm = $('#frmRoutingSearchResultPopup')
        ,frm2 = $('#frm-record-production')
        ,formData = new FormData()
        ,formParams = frm.serializeArray()
        ,formParams2 = frm2.serializeArray();

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });

    $.each(formParams2, function (i, val) {
        formData.append(val.name, val.value);
    });
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_RoutingResult",
        data: formData,
        processData: false,
        contentType: false,
    }).done(function (html) {
        loading2();
        $('#divRoutingSearch').modal('hide');
        $("#divRoutingSearch").html("");
        $('.div-routing-result').html(html);
        
        $.ajax({
            type: "POST",
            url: "/MES/ProductionManagement/_UsageQuantity",
            data: formData,
            processData: false,
            contentType: false,
        }).done(function (html) {
            $('.div-usage-quantity').html(html);
        });
        $btn.button('reset')
    });
});

//Xóa row routing
$(document).on("click", ".btn-delete-row", function (e) {
    loading2();
    e.preventDefault();
    var $btn = $(this);
    $btn.button('loadding');

    var Id = $btn.data('id');
    var frm = $('#frm-record-production'),
        formData = new FormData(),
        formParams = frm.serializeArray()
    formData["ITMNO"] = Id;
    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });
    formData.append("ITMNO",Id)
 
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_DeleteRowRouting",
        data: formData,
        processData: false,
        contentType: false,
    }).done(function (html) {
        $('#divRoutingSearch').modal('hide');
        $("#divRoutingSearch").html("");
        $('.div-routing-result').html(html);
        $('.rows-table-' + $btn.data("row")).remove()

        $btn.button('reset');
    });
});


//Detail
$(document).on("click", ".btn-showhistory", function () {
    loading2();
    var $btn = $(this);
    $btn.button("loading")
    var TTLSX = $(this).data("ttlsxid");
    var fromTime = $(this).data("fromtime");
    var toTime = $(this).data("totime");
    var itmno = $(this).data("itmno");
    var StepCode = $(this).data("stepcode");
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_ProductionRecordhistory",
        data: {
            TTLSX:TTLSX,
            fromTime: fromTime,
            toTime: toTime,
            itmno: itmno,
            StepCode: StepCode
        },
        success: function (html) {
            $('#divProductionRecordhistory').html(html);
            $('#divProductionRecordhistory').modal("show");
            $btn.button("reset")

        },
        error: function (xhr, status, error) {
            alertPopup(false, xhr.responseText);
            $btn.button("reset")
        }
    });
});

$(document).on("click", ".btn-showDepartment", function () {
    loading2();
    var $btn = $(this);
    $btn.button("loading")
    var TTLSX = $(this).data("ttlsxid");
    var fromTime = $(this).data("fromtime");
    var toTime = $(this).data("totime");
    var itmno = $(this).data("itmno");
    $(".tableDetailItem .dropdown-menu").addClass("hidden");

    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_ShowDepartment",
        data: {
            TTLSX: TTLSX,
            fromTime: fromTime,
            toTime: toTime,
            itmno: itmno
        },
        success: function (jsonData) {
            $(".DepartmentItem").html("");
            $(".DepartmentItem").append("<p>Tổ: " + jsonData.Department + "</p><p>Phân xưởng: " + jsonData.Workshop +"</p>");
            $(".DepartmentItem").removeClass("hidden");
            $btn.button("reset")
        },
        error: function (xhr, status, error) {
            alertPopup(false, xhr.responseText);
            $btn.button("reset")
        }
    });
});


$('#divRoutingSearch').on('hidden.bs.modal', function () {
    $('#divRoutingSearch').html("");
});
$('#divProductionRecordhistory').on('hidden.bs.modal', function () {
    $('#divProductionRecordhistory').html("");
});

//Xử lý check có routing thì mới hiển thị nút chuyển và ghi nhận
$(document).on('change', '#frm-switching-stages select[name="ToStepCode"]', function () {
    var ProductCode = $('#frm-switching-stages input[name="ProductCode"]').val();
    var ProductAttributes = $('#frm-switching-stages input[name="ProductAttributes"]').val();
    var ToStockCode = $(this).val();
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/CheckIsHasRouting",
        data: {
            ProductCode: ProductCode,
            ProductAttributes: ProductAttributes,
            ToStockCode: ToStockCode,
        },
        success: function (jsonData) {
            if (jsonData) {
                if (jsonData.IsSuccess) {
                    if (jsonData.Data === true) {
                        $('#btn-save-switching-stages-and-continue').show();
                    }
                    else {
                        $('#btn-save-switching-stages-and-continue').hide();
                    }
                }
                else {
                    $('#btn-save-switching-stages-and-continue').hide();
                }
            }
        },
        error: function (xhr, status, error) {
            //alertPopup(false, xhr.responseText);
            $btn.button("reset")
        }
    });
});

//Xử lý nút Chuyển và Ghi nhận
$(document).on('click', '#btn-save-switching-stages-and-continue', function () {
    var url = window.location.href;
    var id = $(this).data('id');
    //Màn hình quét chuyển công đoạn => sau khi chuyển công đoạn thành công thì chuyển qua trang ghi nhận sản lượng
    if (url.indexOf('/MES/ProductionManagement?Type=CCD') > -1) {
        window.location.href = '/MES/ProductionManagement?Type=GNSL&Barcode=' + id;
    }
    //Màn hình danh sách ghi nhận
    else if (url.indexOf('/MES/ProductionRecording') > -1){
        switchingStages($(this), function() {
            $('.btn-record-production[data-id="' + id + '"]').trigger('click');
        });
    }
    
});

function switchingStages(element, callback) {
    var $btn = $(element);
    loading2();
    $btn.button("loading");
    var frm = $('#frm-switching-stages')
        , formData = new FormData()
        , formParams = frm.serializeArray()

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });

    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/_SwitchingStages",
        data: formData,
        processData: false,
        contentType: false,
        success: function (xhr, status, error) {
            if (xhr.Success === true) {
                //Báo thành công + tắt popup
                ISD.setMessage("#divAlertSuccess", xhr.Data);
                $('#divAlertSuccess').show();
                //tat popup
                $('#divFunctionModal').modal('hide');
                //location.reload();
                if (typeof callback === 'function') {
                    callback();
                }
                else {
                    //gọi lại nút search sau khi cập nhật thành công
                    if ($('#btn-search').length) {
                        $('#btn-search').trigger('click');
                    }
                }
            } else {
                //Báo lỗi trên popup
                ISD.setMessage("#divAlertWarningTask", xhr.Data);
                $('#divFunctionModal #divAlertWarningTask').show();
            }
            $btn.button('reset');
        },
        error: function (xhr, status, error) {
            $('#divAlertWarningTask .alert-message').html(xhr.Data);
            $('#divFunctionModal #divAlertWarningTask').show();
            $btn.button('reset');
        }
    });
}
//Xử lý kiểm tra tồn tại routing mới cho phép ghi nhận
$(document).on('change', '#frm-record-production select[name="StepCode"]', function () {
    var ProductCode = $('#frm-record-production input[name="ProductCode"]').val();
    var ProductAttributes = $('#frm-record-production input[name="ProductAttributes"]').val();
    var ToStockCode = $(this).val();
    $.ajax({
        type: "POST",
        url: "/MES/ProductionManagement/CheckIsHasRouting",
        data: {
            ProductCode: ProductCode,
            ProductAttributes: ProductAttributes,
            ToStockCode: ToStockCode,
        },
        success: function (jsonData) {
            if (jsonData) {
                if (jsonData.IsSuccess) {
                    if (jsonData.Data === true) {
                        $('#divAlertWarningRouting').hide();
                        $('#frm-record-production #btn-save-record-production').show();
                    }
                    else {
                        $('#divAlertWarningRouting').show();
                        $('#frm-record-production #btn-save-record-production').hide();
                    }
                }
                else {
                    $('#divAlertWarningRouting').show();
                    $('#frm-record-production #btn-save-record-production').hide();
                }
            }
        },
        error: function (xhr, status, error) {
            $('#divAlertWarningTask .alert-message').html(xhr.Data);
            $('#divFunctionModal #divAlertWarningTask').show();
        }
    });
});