//Thêm mới thi công
$(document).on("click", "#btn-create-internal", function () {
    var $btn = $(this);
    $btn.button('loading');
    $.ajax({
        type: "POST",
        url: '/Work/Task/_ProfileSearch',
        data: {
            hasNoContact: true,
            ProfileType: 'Account'
        },
        success: function (html) {
            $("#divConstructionPopup").html("");

            $("#divConstructionPopup").html(html);
            $("#divConstructionPopup input[id='SearchProfileId']").val("");
            $(".with-search").select2();
            $("#divConstructionPopup #divProfileSearch").attr("id", 'divProfileSearch-internal');
            $("#divConstructionPopup #frmProfileSearchPopup").attr("id", 'frmProfileSearchPopup-internal');
            $("#divConstructionPopup #divProfileSearch-internal").modal("show");

            $btn.button('reset');
        }
    });
});

$(document).on("click", "#divProfileSearch-internal .btn-profile-choose", function () {
    var profileId = $('#ProfileId').val();
    var partnerId = $(this).data('id');
    $.ajax({
        type: "POST",
        url: '/Customer/Construction/SaveInternal',
        data: {
            ProfileId: profileId,
            PartnerId: partnerId
        },
        success: function (jsonData) {
            if (jsonData.Success === true) {
                $("#divConstructionPopup #divProfileSearch-internal").modal("hide");
                //window.location.href = jsonData.RedirectUrl + "&message=" + jsonData.Data;
                ReloadConstructionList();
            }
            else {
                setModalMessage("#divProfileSearch-internal .modalAlert", jsonData.Data);
            }
        }
    });
});

$(document).on("click", "#btn-create-competitor", function () {
    var $btn = $(this);
    $btn.button('loading');
    $.ajax({
        type: "POST",
        url: '/Work/Task/_ProfileSearch',
        data: {
            hasNoContact: true,
            ProfileType: 'Account'
        },
        success: function (html) {
            $("#divConstructionPopup").html("");

            $("#divConstructionPopup").html(html);
            $("#divConstructionPopup input[id='SearchProfileId']").val("");
            $(".with-search").select2();
            $("#divConstructionPopup #divProfileSearch").attr("id", 'divProfileSearch-competitor');
            $("#divConstructionPopup #frmProfileSearchPopup").attr("id", 'frmProfileSearchPopup-competitor');
            $("#divConstructionPopup #divProfileSearch-competitor").modal("show");

            $btn.button('reset');
        }
    });
});

$(document).on("click", "#divProfileSearch-competitor .btn-profile-choose", function () {
    var profileId = $('#ProfileId').val();
    var partnerId = $(this).data('id');
    $.ajax({
        type: "POST",
        url: '/Customer/Construction/SaveCompetitor',
        data: {
            ProfileId: profileId,
            PartnerId: partnerId
        },
        success: function (jsonData) {
            if (jsonData.Success === true) {
                $("#divConstructionPopup #divProfileSearch-competitor").modal("hide");
                //window.location.href = jsonData.RedirectUrl + "&message=" + jsonData.Data;
                ReloadConstructionList();
            }
            else {
                setModalMessage("#divProfileSearch-competitor .modalAlert", jsonData.Data);
            }
        }
    });
});

function setModalMessage(div, message) {
    if (Array.isArray(message)) {
        var arr = [];
        $.each(message, function (i, item) {
            arr[i] = { err: item };
            $(div + " .modal-alert-message").append("<li>" + arr[i].err + "</li>");
        });
    }
    else {
        $(div + " .modal-alert-message").html(message);
    }
}

$(document).on("click", "#contentInternal .btn-main", function () {
    var OpportunityPartnerId = $(this).data('id');
    $.ajax({
        type: "POST",
        url: '/Customer/Construction/SetMainInternal',
        data: {
            OpportunityPartnerId: OpportunityPartnerId
        },
        success: function (jsonData) {
            if (jsonData.Success === true) {
                //window.location.href = jsonData.RedirectUrl + "&message=" + jsonData.Data;
                ReloadConstructionList();
            }
            else {
                alertPopup(false, jsonData.Data);
            }
        }
    });
});


$(document).on("click", "#contentCompetitor .btn-main", function () {
    var OpportunityPartnerId = $(this).data('id');
    $.ajax({
        type: "POST",
        url: '/Customer/Construction/SetMainCompetitor',
        data: {
            OpportunityPartnerId: OpportunityPartnerId
        },
        success: function (jsonData) {
            if (jsonData.Success === true) {
                //window.location.href = jsonData.RedirectUrl + "&message=" + jsonData.Data;
                ReloadConstructionList();
            }
            else {
                alertPopup(false, jsonData.Data);
            }
        }
    });
});

ReloadConstructionList = function () {
    var profileId = $("input[name='ProfileId']").val();
    var requestUrl = "/Customer/Construction/_ListInternal/" + profileId + "?isLoadContent=true";
    $("#contentInternal table tbody").load(requestUrl);
    var requestUrl2 = "/Customer/Construction/_ListCompetitor/" + profileId + "?isLoadContent=true";
    $("#contentCompetitor table tbody").load(requestUrl2);
};