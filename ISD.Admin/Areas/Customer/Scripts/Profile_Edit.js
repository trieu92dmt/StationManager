
function CustomerTypeCodeChangeInitial() {
    var customerType = $("input[name='CustomerTypeCode']:checked").val();
    var profileType = $("input[name='ProfileTypeCode']").val();
    $("#TypeCode").val(customerType);

    if (profileType == "Account") {
        //Doanh nghiep
        if (customerType == "B") {
            $(".profileB").show();
            $(".profileC").hide();
            $("#divProfileName label").html(Profile_NameBussiness);
            //Add placeholder: cảnh báo nhập sđt + email
            $("#Email").attr("placeholder", EmailBusiness_Hint);
            $("input[name=CompanyNumber]").attr("placeholder", PhoneBusiness_Hint);

            //set width profileName
            setProfileNameWidth("col-md-8");
        } else {
            //Ca nhan
            $(".profileB").hide();
            $(".profileC").show();
            $("#divProfileName label").html(Profile_NameCustomer);
            //Remove placeholder: cảnh báo nhập sđt + email
            $("#Email").attr("placeholder", "");
            $("input[name=Phone]").attr("placeholder", "");

            //set width profileName
            setProfileNameWidth("col-md-7");
        }
    }
}

function setProfileNameWidth(cssClass) {
    //Nếu 7 remove 8
    //if ($("#Profile_General_ProfileName_BC").hasClass("col-md-8") && cssClass == "col-md-7") {
    //    $("#Profile_General_ProfileName_BC").removeClass("col-md-8");
    //    $("#Profile_General_ProfileName_BC").addClass("col-md-7");
    //} else if ($("#Profile_General_ProfileName_BC").hasClass("col-md-7") && cssClass == "col-md-8") {
    //    $("#Profile_General_ProfileName_BC").removeClass("col-md-7");
    //    $("#Profile_General_ProfileName_BC").addClass("col-md-8");
    //}
}

$(document).on("change", "input[name='isForeignCustomer']", function () {
    var isForeignCustomer = $("input[name='isForeignCustomer']:checked").val();
    if (isForeignCustomer == undefined) {
        isForeignCustomer = null;
    }

    var SaleOfficeCode = $("#RequiredSaleOfficeCode").val();
    $.ajax({
        type: "POST",
        url: "/Customer/Profile/GetSaleOfficeBy",
        data: {
            isForeignCustomer: isForeignCustomer
        },
        success: function (jsonData) {
            var arr = [];

            $("#RequiredSaleOfficeCode").html("");
            $("#RequiredSaleOfficeCode").append("<option value=''>-- Vui lòng chọn --</option>");
            $.each(jsonData, function (index, value) {
                arr.push(value.Value);
                $("#RequiredSaleOfficeCode").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
            });

            if (arr.indexOf(SaleOfficeCode) > -1) {
                $("#RequiredSaleOfficeCode").val(SaleOfficeCode);
            }
            $("#RequiredSaleOfficeCode").trigger("change");
        }
    });
    if (isForeignCustomer == "True") {
        $(".address-foreign").hide();
        $("#provinceName label").html("Quốc gia");
    } else {
        $(".address-foreign").show();
        $("#provinceName label").html("Tỉnh/Thành phố");
    }
});

//ProvinceId change
$(document).on("change", "select[name='RequiredProvinceId']", function () {
    var ProvinceId = $(this).val();
    var DistrictId = $("#DistrictId").val();
    $.ajax({
        type: "POST",
        url: "/MasterData/District/GetDistrictByProvince",
        data: {
            ProvinceId: ProvinceId
        },
        success: function (jsonData) {
            $("#DistrictId").html("");
            $("#DistrictId").append("<option value=''>-- Vui lòng chọn --</option>");
            $.each(jsonData, function (index, value) {
                $("#DistrictId").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
            });
            if (DistrictId) {
                $("#DistrictId").val(DistrictId).trigger("change");
            }
            else {
                $("#DistrictId").trigger("change");
            }
        }
    });
});

//change sale office
$(document).on("change", "select[name='RequiredSaleOfficeCode']", function () {
    var SaleOfficeCode = $(this).val();
    var ProvinceId = $("#RequiredProvinceId").val();
    $.ajax({
        type: "POST",
        url: "/Customer/Profile/GetProvinceBy",
        data: {
            SaleOfficeCode: SaleOfficeCode
        },
        success: function (jsonData) {
            var provinceLst = jsonData.provinceList;

            $("#RequiredProvinceId").html("");
            $("#RequiredProvinceId").append("<option value=''>-- Vui lòng chọn --</option>");
            $.each(provinceLst, function (index, value) {
                $("#RequiredProvinceId").append("<option value='" + value.ProvinceId + "'>" + value.ProvinceName + "</option>");
            });
            if (ProvinceId) {
                $("#RequiredProvinceId").val(ProvinceId).trigger("change");
            }
            else {
                $("#RequiredProvinceId").trigger("change");
            }
        }
    });
});

//District change
$(document).on("change", "#frmEdit select[name='DistrictId']", function () {
    var districtId = $(this).val();
    var WardId = $('#WardId').val();
    $.ajax({
        type: "POST",
        url: "/MasterData/Ward/GetWardByDistrict",
        data: {
            DistrictId: districtId
        },
        success: function (jsonData) {
            var $ward = $("#frmEdit #WardId");
            $ward.html("");
            $ward.append("<option value=''>- Vui lòng chọn -</option>");
            $.each(jsonData, function (index, value) {
                $ward.append("<option value='" + value.Value + "'>" + value.Text + "</option>");
            });
            if (WardId) {
                $ward.val(WardId).trigger('change');
            }
        }
    });
});

///add More Phone
$(document).on('click', '#frmEdit .btn-addPhone', function (e) {
    //console.log("Dzoo");
    e.preventDefault();

    var controlForm = $('#frmEdit .phoneControls:first'),
        currentEntry = $(this).parents('#frmEdit .phonenumber:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('input').val('');
    controlForm.find('.phonenumber:not(:first) .btn-addPhone')
        .removeClass('btn-addPhone').addClass('btn-removePhone')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
}).on('click', '#frmEdit .btn-removePhone', function (e) {
    $(this).parents('#frmEdit .phonenumber:last').remove();

    e.preventDefault();
    return false;
});
//end add more phone

var indexperson = 0;
///add More PersonInCharge
$(document).on('click', '.btn-addpersonincharge', function (e) {
    //console.log("Dzoo");
    e.preventDefault();
    indexperson++;
    var controlForm = $('.personInchargeControls:first'),
        currentEntry = $(this).parents('.personincharge:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('select[name="personInCharge[0].SalesEmployeeCode"]').attr('name', 'personInCharge[' + indexperson + '].SalesEmployeeCode');
    newEntry.find('select[name="personInCharge[0].RoleCode"]').attr('name', 'personInCharge[' + indexperson + '].RoleCode');

    newEntry.find('.select2').remove();
    $("select").select2();
    controlForm.find('.personincharge:not(:first) .btn-addpersonincharge')
        .removeClass('btn-addpersonincharge').addClass('btn-removepersonincharge')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
}).on('click', '.btn-removepersonincharge', function (e) {
    $(this).parents('.personincharge:last').remove();

    e.preventDefault();
    return false;
});
//end add more PersonInCharge

var indexrole = 0;
///add More RoleInCharge
$(document).on('click', '.btn-addroleincharge', function (e) {
    //console.log("Dzoo");
    e.preventDefault();
    indexrole++;
    var controlForm = $('.roleInChargeControls:first'),
        currentEntry = $(this).parents('.roleincharge:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);
    newEntry.find('select[name="roleInCharge[0].RolesId"]').attr('name', 'roleInCharge[' + indexrole + '].RolesId');
    newEntry.find('.select2').remove();
    $("select").select2();
    controlForm.find('.roleincharge:not(:first) .btn-addroleincharge')
        .removeClass('btn-addroleincharge').addClass('btn-removeroleincharge')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
}).on('click', '.btn-removeroleincharge', function (e) {
    $(this).parents('.roleincharge:last').remove();

    e.preventDefault();
    return false;
});
//end add more RoleInCharge

var indexRowProfileGroup = 0;
$(document).on('click', '.btn-addProfileGroup', function (e) {
    //console.log("Dzoo");
    e.preventDefault();

    $(".profileGroup").each(function (index, value) {
        indexRowProfileGroup = index;
    });
    indexRowProfileGroup++;

    var controlForm = $('.profileGroupControls:first'),
        currentEntry = $(this).parents('.profileGroup:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('.profilegroupcode').attr('name', 'profileGroupList[' + indexRowProfileGroup + '].ProfileGroupCode').val('');
    newEntry.find('.profilegroupcode').data('row', indexRowProfileGroup);

    newEntry.find('.select2').remove();
    $("select").select2();

    controlForm.find('.profileGroup:not(:first) .btn-addProfileGroup')
        .removeClass('btn-addProfileGroup').addClass('btn-removeProfileGroup')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
});

$(document).on('click', '.btn-removeProfileGroup', function (e) {
    $(this).parents('.profileGroup:last').remove();

    $(".profileGroup").each(function (index, value) {
        $(this).find('.profilegroupcode').attr("name", "profileGroupList[" + index + "].ProfileGroupCode");
    });
    e.preventDefault();
    return false;
});

//Xem chi tiết catalog
$(document).on("click", ".btn-showStockDelivery", function () {
    var DeliveryId = $(this).data("id");
    $("#contentCatalogue .dropdown-menu").addClass("hidden");

    $.ajax({
        type: "POST",
        url: "/Warehouse/StockDelivery/GetProductDetails",
        data: {
            DeliveryId: DeliveryId
        },
        success: function (jsonData) {
            $("#contentCatalogue .dropdown-menu").html("");
            $.each(jsonData, function (index, value) {
                $("#contentCatalogue .dropdown-menu").append("<li>" + value.ProductName + "</li>");
            });
            $("#contentCatalogue .dropdown-menu").removeClass("hidden");
        },
        error: function (xhr, status, error) {
            alertPopup(false, xhr.responseText);
        }
    });
});

var indexRowPerson = 0;
$(document).on('click', '.btn-addPersonCharge', function (e) {
    //console.log("Dzoo");
    e.preventDefault();

    $(".personCharge").each(function (index, value) {
        indexRowPerson = index;
    });
    indexRowPerson++;

    var controlForm = $('.personInChargeControls:first'),
        currentEntry = $(this).parents('.personCharge:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);


    newEntry.find('.personemployeecode').attr('name', 'personInChargeList[' + indexRowPerson + '].SalesEmployeeCode').val('');
    newEntry.find('.personemployeecode').data('row', indexRowPerson);
    newEntry.find('.personrolecode').attr('name', 'personInChargeList[' + indexRowPerson + '].RoleCode').val('');
    newEntry.find('.roleName').removeClass('roleName_0');
    newEntry.find('.roleName').addClass('roleName_' + indexRowPerson);

    newEntry.find('.select2').remove();
    newEntry.find('.roleName').html('');
    $("select").select2();

    controlForm.find('.personCharge:not(:first) .btn-addPersonCharge')
        .removeClass('btn-addPersonCharge').addClass('btn-removePersonCharge')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');

    displayTitle();

});

$(document).on("change", ".personemployeecode", function () {
    var SalesEmployeeCode = $(this).val();
    var row = $(this).data('row');

    $.ajax({
        type: "POST",
        url: "/Customer/Profile/GetRoleBySaleEmployee",
        data: {
            SalesEmployeeCode: SalesEmployeeCode
        },
        success: function (jsonData) {
            $(".roleName_" + row).html("");
            $(".roleName_" + row).html(jsonData);
        }
    });
});

$(document).on('click', '.btn-addPersonCharge2', function (e) {
    //console.log("Dzoo");
    e.preventDefault();
    var indexRowPerson2 = 0;
    $(".personCharge2").each(function (index, value) {
        indexRowPerson2 = index;
    });
    indexRowPerson2++;

    var controlForm = $('.personInChargeControls2:first'),
        currentEntry = $(this).parents('.personCharge2:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('.personemployeecode2').attr('name', 'personInCharge2List[' + indexRowPerson2 + '].SalesEmployeeCode').val('');
    newEntry.find('.personemployeecode2').data('row', indexRowPerson2);
    newEntry.find('.personrolecode2').attr('name', 'personInCharge2List[' + indexRowPerson2 + '].RoleCode').val('');
    newEntry.find('.roleName2').removeClass('roleName2_0');
    newEntry.find('.roleName2').addClass('roleName2_' + indexRowPerson2);

    newEntry.find('.select2').remove();
    newEntry.find('.roleName2').html('');
    $("select").select2();

    controlForm.find('.personCharge2:not(:first) .btn-addPersonCharge2')
        .removeClass('btn-addPersonCharge2').addClass('btn-removePersonCharge2')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
});

$(document).on('click', '.btn-removePersonCharge2', function (e) {
    $(this).parents('.personCharge2:last').remove();

    $(".personCharge2").each(function (index, value) {
        $(this).find('.personemployeecode2').attr("name", "personInCharge2List[" + index + "].SalesEmployeeCode");
        $(this).find('.personrolecode2').attr("name", "personInCharge2List[" + index + "].RoleCode");
    });
    e.preventDefault();
    return false;
});

$(document).on("change", ".personemployeecode2", function () {
    var SalesEmployeeCode = $(this).val();
    var row = $(this).data('row');

    $.ajax({
        type: "POST",
        url: "/Customer/Profile/GetRoleBySaleEmployee",
        data: {
            SalesEmployeeCode: SalesEmployeeCode
        },
        success: function (jsonData) {
            $(".roleName2_" + row).html("");
            $(".roleName2_" + row).html(jsonData);
        }
    });
});

function displayTitle() {
    var customerType = $("input[name='CustomerTypeCode']:checked").val();
    if (customerType == "B") {
        $("#divProfileB").show();
        $("#divProfileC").hide();
    }
    else {
        $("#divProfileB").hide();
        $("#divProfileC").show();
    }
}

$(document).on('click', '.btn-removePersonCharge', function (e) {
    $(this).parents('.personCharge:last').remove();

    $(".personCharge").each(function (index, value) {
        $(this).find('.personemployeecode').attr("name", "personInChargeList[" + index + "].SalesEmployeeCode");
        $(this).find('.personrolecode').attr("name", "personInChargeList[" + index + "].RoleCode");
    });
    e.preventDefault();
    return false;
});

$(document).on("change", "input[name='ContractValue']", function () {
    var value = $(this).val();
    $(".numberContractValue").html("");
    var formatCurrencyContractValue = formatCurrency(value);
    if (value != "" && value > 0) {
        $(".numberContractValue").removeClass("hidden");
        $(".numberContractValue").html(formatCurrencyContractValue);
    }
    else {
        $(".numberContractValue").addClass("hidden");
    }
});

$(document).on("change", "input[name='IsAnCuongAccessory']", function () {
    var IsAnCuongAccessory = $("input[name='IsAnCuongAccessory']:checked").val();
    if (IsAnCuongAccessory == true || IsAnCuongAccessory == "True" || IsAnCuongAccessory == "true") {
        $(".hidden_IsAnCuongAccessory").removeClass("hidden");
    }
    else {
        $(".hidden_IsAnCuongAccessory").addClass("hidden");
    }
});

//Lấy danh sách đơn hàng trên SAP
function GetCustomerSaleOrder() {
    var ProfileForeignCode = $("#ProfileForeignCode").val();
    if (ProfileForeignCode) {
        $.ajax({
            type: "POST",
            url: "/Customer/CustomerSaleOrder/_List",
            data: {
                ProfileForeignCode: ProfileForeignCode,
                isLoadContent: true
            },
            success: function (jsonData) {
                if (!jsonData.Code) {
                    $("#tab-don-hang").html(jsonData);
                }
            }
        });
    }
}

//Xem doanh số theo năm
$(document).on("click", "#btn-view-revenue", function () {
    var $btn = $(this);
    $btn.button('loading');
    var ProfileId = $("#ProfileId").val();
    var Year = $("#Year").val();

    $.ajax({
        type: "POST",
        url: "/Customer/Revenue/_ProfileRevenue",
        data: {
            id: ProfileId,
            Year: Year
        },
        success: function (jsonData) {
            if (!jsonData.Code) {
                $("#tab-revenue").html(jsonData);
            }
            $btn.button('reset');
        },
        error: function (xhr, status, error) {
            alertPopup(false, xhr.responseText);
            $btn.button('reset');
        }
    });
});

//dự án - thi công 
var indexRowInternal = 0;
$(document).on('click', '.btn-addInternal', function (e) {
    //console.log("Dzoo");
    e.preventDefault();

    $(".internal").each(function (index, value) {
        indexRowInternal = index;
    });
    indexRowInternal++;

    var controlForm = $('.internalControls:first'),
        currentEntry = $(this).parents('.internal:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);


    newEntry.find('.constructionname').attr('name', 'internalList[' + indexRowInternal + '].ConstructionName').val('');
    newEntry.find('.constructionname').attr('id', 'internalList_' + indexRowInternal + '__ConstructionName').val('');
    newEntry.find('.constructionid').attr('name', 'internalList[' + indexRowInternal + '].ConstructionId').val('');
    newEntry.find('.constructionid').attr('id', 'internalList_' + indexRowInternal + '__ConstructionId').val('');
    newEntry.find('.btn-get-profile').data('row', indexRowInternal);
    newEntry.find('.btn-del-profile').data('row', indexRowInternal);

    newEntry.find('.select2').remove();
    $("select").select2();

    controlForm.find('.internal:not(:first) .btn-addInternal')
        .removeClass('btn-addInternal').addClass('btn-removeInternal')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
});

$(document).on('click', '.btn-removeInternal', function (e) {
    $(this).parents('.internal:last').remove();

    $(".internal").each(function (index, value) {
        $(this).find('.constructionname').attr("name", "internalList[" + index + "].ConstructionName");
        $(this).find('.constructionname').attr('id', 'internalList_' + index + '__ConstructionName');
        $(this).find('.constructionid').attr("name", "internalList[" + index + "].ConstructionId");
        $(this).find('.constructionid').attr('id', 'internalList_' + index + '__ConstructionId');
        $(this).find('.btn-get-profile').data('row', index);
        $(this).find('.btn-del-profile').data('row', index);
    });
    e.preventDefault();
    return false;
});

var indexRowCompetitor = 0;
$(document).on('click', '.btn-addCompetitor', function (e) {
    //console.log("Dzoo");
    e.preventDefault();

    $(".competitor").each(function (index, value) {
        indexRowCompetitor = index;
    });
    indexRowCompetitor++;

    var controlForm = $('.competitorControls:first'),
        currentEntry = $(this).parents('.competitor:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);


    newEntry.find('.constructionname').attr('name', 'competitorList[' + indexRowCompetitor + '].ConstructionName').val('');
    newEntry.find('.constructionname').attr('id', 'competitorList_' + indexRowCompetitor + '__ConstructionName').val('');
    newEntry.find('.constructionid').attr('name', 'competitorList[' + indexRowCompetitor + '].ConstructionId').val('');
    newEntry.find('.constructionid').attr('id', 'competitorList_' + indexRowCompetitor + '__ConstructionId').val('');
    newEntry.find('.btn-get-profile').data('row', indexRowCompetitor);
    newEntry.find('.btn-del-profile').data('row', indexRowCompetitor);

    newEntry.find('.select2').remove();
    $("select").select2();

    controlForm.find('.competitor:not(:first) .btn-addCompetitor')
        .removeClass('btn-addCompetitor').addClass('btn-removeCompetitor')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
});

$(document).on('click', '.btn-removeCompetitor', function (e) {
    $(this).parents('.competitor:last').remove();

    $(".competitor").each(function (index, value) {
        $(this).find('.constructionname').attr("name", "competitorList[" + index + "].ConstructionName");
        $(this).find('.constructionname').attr('id', 'competitorList' + index + '__ConstructionName');
        $(this).find('.constructionid').attr("name", "competitorList[" + index + "].ConstructionId");
        $(this).find('.constructionid').attr('id', 'competitorList_' + index + '__ConstructionId');
        $(this).find('.btn-get-profile').data('row', index);
        $(this).find('.btn-del-profile').data('row', index);
    });
    e.preventDefault();
    return false;
});

//add More Email
$(document).on('click', '#frmEdit .btn-addEmail', function (e) {
    e.preventDefault();

    var controlForm = $('#frmEdit .emailControls:first'),
        currentEntry = $(this).parents('#frmEdit .email:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('input').val('');
    controlForm.find('.email:not(:first) .btn-addEmail')
        .removeClass('btn-addEmail').addClass('btn-removeEmail')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
}).on('click', '#frmEdit .btn-removeEmail', function (e) {
    $(this).parents('#frmEdit .email:last').remove();

    e.preventDefault();
    return false;
});

$(document).on('click', '#frmProfileContact .btn-addEmail', function (e) {
    e.preventDefault();

    var controlForm = $('#frmProfileContact .emailControls:first'),
        currentEntry = $(this).parents('#frmProfileContact .email:last'),
        newEntry = $(currentEntry.clone()).appendTo(controlForm);

    newEntry.find('input').val('');
    controlForm.find('.email:not(:first) .btn-addEmail')
        .removeClass('btn-addEmail').addClass('btn-removeEmail')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="glyphicon glyphicon-minus"></span>');
}).on('click', '#frmProfileContact .btn-removeEmail', function (e) {
    $(this).parents('#frmProfileContact .email:last').remove();

    e.preventDefault();
    return false;
});
            //end add more email