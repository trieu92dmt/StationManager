//Autocomplete Product
function SearchText_ProductCode() {
    $("#ProductCode").autocomplete({
        source: function (request, response) {
            var FromstockId = $("#FromStockId").val();
            if (FromstockId == "") {
                alertPopup(false, "Vui lòng chọn kho xuất trước!");
                $("#ProductCode").val("");
                $("#ProductCode").prop("disabled", false);
                $("#FromStockId").prop("disabled", false);
                $("#ToStockId").prop("disabled", false);
                $("#SerialOfProduct").val("");
                $("#ProductName").val("");
                $("#ProductQuantinyOnHand").val("");
                $("#ProductStockNote").val("");
                $("#FromStockCode").focus();
            }
            var TostockId = $("#ToStockId").val();
            if (TostockId == "") {
                alertPopup(false, "Vui lòng chọn kho nhập trước!");
                $("#ProductCode").val("");
                $("#ProductCode").prop("disabled", false);
                $("#FromStockId").prop("disabled", false);
                $("#ToStockId").prop("disabled", false);
                $("#SerialOfProduct").val("");
                $("#ProductName").val("");
                $("#ProductQuantinyOnHand").val("");
                $("#ProductStockNote").val("");
                $("#FromStockCode").focus();
            }
            if (FromstockId === TostockId) {
                alertPopup(false, "Không được trùng kho chuyển và kho xuất");
                $("#ProductCode").val("");
                $("#ProductCode").prop("disabled", false);
                $("#FromStockId").prop("disabled", false);
                $("#ToStockId").prop("disabled", false);
                $("#SerialOfProduct").val("");
                $("#ProductName").val("");
                $("#ProductQuantinyOnHand").val("");
                $("#ProductStockNote").val("");
                $("#FromStockCode").focus();
            }
            else {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Sale/Product/SearchProductForFromStock",
                    data: JSON.stringify({ "SearchText": $("#ProductCode").val() }),
                    dataType: 'json',
                    success: function (data) {
                        response($.map(data, function (item) {
                            console.log(item);
                            return { label: item.ProductCode + " | " + item.ProductSearchName + " | " + item.Serial, id: item.ProductId, value: item.ProductCode, code: item.ProductName, serial: item.Serial };
                        })
                        )
                    },
                    error: function (xhr, status, error) {
                        alertPopup(false, xhr.response);
                    }
                });
            }
        },
        autoFocus: true,
        //Ngăn chặn không cho 1 giá trị nào ngoại trừ giá trị trong dropdown của autocomplete
        change: function (item, ui) {
            if (ui.item === null) {
                $("#ProductCode").val("");
                $("#ProductName").val("");
                $("#ProductId").val("");
                $("#ProductQuantinyOld").val("");
                $("#ProductCode").focus();
            }
        },
        select: function (item, ui) {
            $("#FromStockId").prop("disabled", true);
            $("#ToStockId").prop("disabled", true);
   
            $("#ProductCode").val(ui.item.value);
            $("#ProductCode").prop("disabled", true);
            $("#ProductId").val(ui.item.id);
            $("#ProductName").val(ui.item.code);
            $("#SerialOfProduct").val(ui.item.serial);
            $("#ProductQuantity").focus();
            $.ajax({
                type: "POST",
                url:"/Warehouse/ChuyenKho/GetProductTon",
                data: {
                    StockId: $("#FromStockId").val(),
                    ProductId: ui.item.id
                },
                success: function(xhr,status,error) {
                    $("#ProductQuantinyOnHand").val(xhr.Qty);
                },
                error: function (xhr, status, error) {
                    alertPopup(false,xhr.response);
                }
            })
            if (event.keyCode === 9 || event.keyCode === 13) {
                event.preventDefault();
                $("#ProductCode").val(ui.item.value);
            }
        },


    })
}

//Bắt sự kiện nút xóa các val trên form
$(document).on("click", "#btn-del-product", function () {
    $("#ProductCode").val("");
    $("#ProductCode").prop("disabled", false);
    $("#FromStockId").prop("disabled", false);
    $("#ToStockId").prop("disabled", false);
    $("#ProductId").val("");
    $("#ProductName").val("");
    $("#FromStockName").val("");
    $("#ToStockName").val("");
    $("#ProductQuantinyOnHand").val("");
    $("#ProductStockNote").val("");
    $("#SerialOfProduct").val("");
    $("#ProductCode").focus();
})

//clear value in form
function ClearValue() {
    $("#ProductCode").val("");
    $("#ProductCode").prop("disabled", false);
    $("#FromStockId").prop("disabled", false);
    $("#ToStockId").prop("disabled", false);
    $("#ProductId").val("");
    $("#ProductName").val("");
    $("#ProductQuantinyOnHand").val("");
    $("#ProductStockNote").val("");
    $("#SerialOfProduct").val("");
    $("#FromStockCode").focus();
}
//Bắt sự kiện nút thêm vào 
$(document).on("click", "#btn-add-stockReceivingDetail", function () {
    var $btn = $(this);
    $btn.button('loading');
   
    //Không hiểu sao không lấy được dữ liệu
    //var data = $("#frmThem").serialize();
    //console.log(data);

    var ProductId = $("#ProductId").val();
    var ProductCode = $("#ProductCode").val();
    var ProductName = $("#ProductName").val();
    var ProductQuantity = $("#ProductQuantity").val();
    var ProductQuantinyOnHand = $("#ProductQuantinyOnHand").val();
    var ProductPrice = $("#ProductPrice").val();
    var FromStockId = $("#FromStockId").val();
    var ToStockId = $("#ToStockId").val();
    var ProductStockNote = $("#ProductStockNote").val();
    var SerialOfProduct = $("#SerialOfProduct").val();

    if (ProductCode === "") {
        $btn.button('reset');
        alertPopup(false, "Vui lòng chọn sản phẩm trước khi thêm!");
    }
    else {
       
      
        if (ProductQuantity == "" || parseInt(ProductQuantity) < 0) {
            $btn.button('reset');
            alertPopup(false, "Xin vui lòng nhập số lượng sản phẩm lớn hơn 0!");
        }
        else if (parseInt(ProductQuantity) > parseInt(ProductQuantinyOnHand) || ProductQuantinyOnHand == "") {
            $btn.button('reset');
            alertPopup(false, "Xin vui lòng không nhập số lượng sản phẩm cần chuyển lớn hơn số lượng sản phẩm tồn kho!");
        }
        //else if (parseInt(ProductQuantinyOnHand) === 0) {
        //    $btn.button('reset');
        //    alertPopup(false, "Không thể chuyển vì sản phẩm này kho xuất không có tồn");
        //}
        else {
            var frmId = "frmCreate";
            var $frmCreate = $("body #frmCreate");
            var data = $("#" + frmId).serialize();
            data = data + "&ProductId=" + ProductId;
            data = data + "&ProductCode=" + ProductCode;
            data = data + "&ProductName=" + ProductName;
            data = data + "&Serial=" + SerialOfProduct;
            data = data + "&Quantity=" + ProductQuantity;
            data = data + "&QuantinyOnHand=" + ProductQuantinyOnHand;
            data = data + "&Price=" + ProductPrice;
            data = data + "&FromStockId=" + FromStockId;
            data = data + "&ToStockId=" + ToStockId;
            data = data + "&DetailNote=" + ProductStockNote;
            
 
            $.ajax({
                type:"POST",
                url: "/Warehouse/ChuyenKho/InsertProductStock",
                data: data,
                success: function (xhr, status, error) {
                    $btn.button('reset');
                    if (xhr.Message !== "" && xhr.Message !== null && xhr.Message !== undefined) {
                        alertPopup(false, xhr.Message);
                    } else {
                        $("#transferDetailTable tbody").html(xhr);
                        ClearValue();
                    }
                },
                error: function (xhr, status,error) {
                    $btn.button('reset');
                    alertPopup(false, xhr.responseText);
                }
            });
        }
        
    }

})
//$(document).on("click", ".btn-del-proDetail", function () {
//    $(this).parent().parent().remove();
//});
//$("#btn-save-continue").on('click', function () {
//    $("#transferDetailTable tbody#transferDetailList").html("");
//    $("#transferDetailTable tbody#transferDetailList").html(xhr);
//});
//Xóa sản phẩm khỏi lưới
$(document).on("click", ".btn-del-proDetail", function () {

    var frmId = "";
    frmId = "frmCreate";
    var data = $("#" + frmId).serialize();
    var STT = $(this).data("row");
    data = data + "&STT=" + STT;

    $.ajax({
        type: "POST",
        url: "/Warehouse/ChuyenKho/RemoveProductStock",
        data: data,
        success: function (xhr, status, error) {
            if (xhr.Message != "" && xhr.Message != null && xhr.Message != undefined) {
                alertPopup(false, xhr.Message);
            }
            else {
                $("#transferDetailTable tbody#transferDetailList").html("");
                $("#transferDetailTable tbody#transferDetailList").html(xhr);
                //TotalPrice();
            }
        },
        error: function (xhr, status, error) {
            alertPopup(false, xhr.responseText);
        }
    });
});