//-- BEGIN event tách lệnh sản xuất--
$('body').on('click', '.btn-DivionOfTask', function () {   
    var id = $btn.data("id");
    $.ajax({
        data: {
            Id: id
        },
        url: 'ProductionOrder/_DivisionOfTask'
    }).done(function (html) {
        $('.modal-content').html(html);
        $('#popupProductionOrder').modal('show');


    })
})
   
$('#popupProductionOrder').on('hidden.bs.modal', function () {
    $('#btn-Search').click();
});
//  begin Add row
//$('body').on('click', '.btn-add-row', function (event) {
//    var $btn = $(".btn-add-row");
//    $btn.button('loading');
//    event.preventDefault()
//    var $form = $('.frm-task-product').serializeArray()
//    $.ajax({
//        url: 'ProductionOrder/_AddRow',
//        data: $form,
//        type: 'GET',
//    }).done(function (html) {
//        if (html != null) {
//            $('.detail-production-order').html(html);
//            $('input.work-flow-id').val($('input[name="createSubTaskViewModels[0].WorkFlowId"]').val());

//        } else {
//            //Báo lỗi trên popup
//            ISD.setMessage("#divAlertWarningTask", "Lỗi chia đợt");
//            $('#popupProductionOrder #divAlertWarningTask').show();
//        }


//        $btn.button('reset');
//    })

//})
// end show modal add row

//  begin show modal phân bổ chi tiết
$('body').on('click', '.btn-PhanBo', function (event) {
    var $btn = $(".btn-PhanBo");
    $btn.button('loading');
    event.preventDefault()
    var qty = $('.txt-sl-phan-bo').val()

    $.ajax({
        data: {
            WorkFlowId: $('#WorkFlowId').val(),
            qty: qty
        },
        url: 'ProductionOrder/_DetailProductionOrder'
    }).done(function (html) {
        if (html!=null) {
            //thành công
            var SLLSanPham = $('input[name=Qty]#Qty').val();// số lương sản phầm cần sản xuất
            var SLSanXuat = $('.txt-sl-phan-bo').val();// số lần sx
            var SLMoiLanPB = Math.floor(SLLSanPham / SLSanXuat); // số lượng mỗi lần phân bổ
            var SoSPDu = SLLSanPham % SLSanXuat; // số lượng còn dư

            $('.detail-production-order').html(html);
            var row = $('.detail-production-order tr.detail-product-order'); // lấy tất cả nhữn dòng là chi tiết phân bổ
            $('#start-date-0').val($('input[name=StartDate]#StartDate').val())
            $('#estimate-end-date-' + (SLSanXuat-1)).val($('input[name=EstimateEndDate]#EstimateEndDate').val())
            row.each(function (i, item) { // lặp từng dòng
                $('input#summary-' + i).val("Đợt " + (i+1));
                //$('.SubTaskActions_' + i).append("<a href='#' class='btn btn-danger' > Xoá</a> <a href='#' class='btn btn-success' > Cập nhật</a>");


                if (row.length == i) { // nếu là lần sản xuất cuối cùng thì công thêm  số lương  dư
                    $('input#ct-phan-bo-' + i).val(SLMoiLanPB + SoSPDu);
                }
                else {
                    $('input#ct-phan-bo-' + i).val(SLMoiLanPB)
                }
                i++;
            })
        } else {
            //Báo lỗi trên popup
            ISD.setMessage("#divAlertWarningTask", "Lỗi chia đợt");
            $('#popupProductionOrder #divAlertWarningTask').show();
        }
        

        $btn.button('reset');   
    })

})
// end show modal phân bổ lệnh sản xuất

// begin event thay đổi số lượng mỗi lần sản xuất
$('body').on('change', '.sl-phan-bo', function (event) {
    event.preventDefault()
    arrSLSanXuat = $('.sl-phan-bo');
    var SLThucThe = 0;
    var SLSPSXThucThe = $('input[name=Qty]').val();// số lương sản phẩn cần sản xuất
    $.each($('.sl-phan-bo'), function (i, item) {
        SLThucThe = SLThucThe + parseInt(item.value);
    })
    if (SLThucThe > SLSPSXThucThe) {
        $('.alert-message').html('Số lượng không hợp lệ ');
        $('#divAlertWarningTask').show();
    }
    else {
        $('#divAlertWarningTask').hide();
    }
})
// end event thay đổi số lượng mỗi lần sản xuất
function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
// begin event lưu phân bổ lệnh sản xuất
$('body').on('click', '.btn-save-subtask', function (e) {
    var $btn = $(".btn-save-subtask");
    $btn.button('loading');
    e.preventDefault();
    var $form = $('.frm-task-product').serialize()
    $.ajax({
        url: '/Work/ProductionOrder/Create',
        data: $form,
        type: 'POST',
        dataType: "json",
        success: function (xhr, status, error) {
            $btn.button('reset');

            if (xhr.Success === true) {
                //Báo thành công + tắt popup
                ISD.setMessage("#divAlertSuccess", xhr.Data);
                $('#divAlertSuccess').show();
                //tat popup
                $('#popupProductionOrder').modal('hide');
            } else {
                //Báo lỗi trên popup
                ISD.setMessage("#divAlertWarningTask", xhr.Data);
                $('#popupProductionOrder #divAlertWarningTask').show();
            }
        },
        error: function () {
            $btn.button('reset');

            $('.alert-message').html(json.Data);
            $('#divAlertWarningTask').show();
        }
    })
    
})
    // end vent lưu phân bổ lệnh sản xuất
    //--END event tách lệnh sản xuất --