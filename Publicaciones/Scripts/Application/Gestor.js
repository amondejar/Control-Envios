//var URLBASE = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port;
var URL = BaseURL + /Gestor/;
var IndexURL = URL + "Index";

function toDate(date) {
    //console.log(date)
    var all = date.split("/");
    var year = all[2].split(" ");
    var hours = year[1].split(":");
    console.log("Date ", year[0], all[0] - 1, all[1], hours[0], hours[1]);
    return f = new Date(year[0], all[0] - 1, all[1], hours[0], hours[1]);
}

function dateValidation() {

    if (toDate($("#DtFechaFin").val()) >= toDate($("#DtFechaInicio").val())) {
        $('span[data-valmsg-for="DtFechaInicio"]').text('');
        $('span[data-valmsg-for="DtFechaFin"]').text('');
        return true;
    }
    else {
        $('span[data-valmsg-for="DtFechaInicio"]').text(GetResouceMessage("ValidateStatDate"));
        $('span[data-valmsg-for="DtFechaFin"]').text(GetResouceMessage("ValidateStatDate"));
        return false;
    }
}



$(document).ready(function () {

    $('#content').css('background-color', '#C5D02B');


    function loadView(pageInfo, navigationDirection) {
       
        if (typeof (pageInfo) === 'undefined') pageInfo = "";
        if (typeof (navigationDirection) === 'undefined') navigationDirection = "";
        $("#itemList").load(URL + "RecuperaFiltroEnvios?Valor1=" + pageInfo + "&Valor2=" + navigationDirection + "", function () {
            //  
        });
    }


    loadView();

    function reloadView(isError, message) {
        setTimeout(function () {
            loadView();
        }, 2000);
    }

    $(document).on("click", ".btnNavigaton", function (e) {
        e.preventDefault();
        var pageInfo = getParameterByName("pageInfo", this.href);
        var navigationDirection = getParameterByName("navigationDirection", this.href);
        loadView(pageInfo, navigationDirection);
        return false;
    });

    $(document).on("click", "#btnAdd", function (e) {
        e.preventDefault();
        $("#itemList").load(URL + "Create", function () {
            //  
        });
    });
    //Save Expense request.
    $(document).on("click", "#btnSave", function (e) {
        e.preventDefault();
        if ($("#formGestor").valid()) {
            $.ajax({
                type: "POST",
                url: $("#formGestor").attr("action"),
                data: $('#formGestor').serialize(),
                success: function (response) {
                    hideLoading();
                    showTostMessage(response.success, response.message);
                    //reloadView();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("some error");
                }
            });

        }
        return false;
    });
   



    //Redirect on index view.
    $(document).on("click", ".btnBack", function (e) {
        e.preventDefault();
        loadView();
    });
      
    //Get details of certificate request
    $(document).on("click", ".btnView", function (e) {
        e.preventDefault();
        //var id = $(this).attr("id");
        var id = $(this).attr("id");
        $("#itemList").load(URL + "Detail?id=" + id + "", function () {
            //  
        });
    });

    //Get certificate details for edit
    $(document).on("click", ".btnEdit", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        $("#itemList").load(URL + "Edit?id=" + id + "", function () {
            //  
        });
    });

    //Delete Certificate request.
    $(document).on("click", ".btnDelete", function (e) {
        e.preventDefault();
        var id = $(this).attr('id');
        Modaldialog(GetResouceMessage("CancelRequest"), "btnCancelCertificateRequest", id, GetResouceMessage("btnDialogConfirm"), GetResouceMessage("titlConfirmation"));
        return false;
    });

    $("#CommonDialog").on("click", "#btnCancelCertificateRequest", function () {
        // code
        var id = $("#txtId").val();
        ModalHide();
        $.ajax({
            type: "GET",
            url: URL + "Delete?id=" + id + "",
            success: function (response) {
                hideLoading();
                showTostMessage(response.success, response.message);
                reloadView();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("some error");
            }
        });
    });

});

