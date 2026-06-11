//var URLBASE = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port;
var URL = BaseURL + /Gestor/;
var IndexURL = URL + "Index";

var CancelaEnvioId;



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

    $('#content').css('background-color', '#ffffff');

    //function loadView(pageInfo, navigationDirection) {
       
    //    if (typeof (pageInfo) === 'undefined') pageInfo = "";
    //    if (typeof (navigationDirection) === 'undefined') navigationDirection = "";
    //    $("#itemList").load(URL + "RecuperaFiltroEnvios?Valor1=" + pageInfo + "&Valor2=" + navigationDirection + "", function () {
    //        //  
    //    });
    //}

    console.log(cargalista)
  
    function loadView(pageInfo, navigationDirection) {

        if (typeof (pageInfo) === 'undefined') pageInfo = "";
        if (typeof (navigationDirection) === 'undefined') navigationDirection = "";
        if (cargalista == '0')
        {
            $("#itemList").load(URL + "NavegaConsultaEnvios", function () {
                //  
            });
        }
        if(cargalista == '1')
        {
            $("#itemList").load(URL + "RecuperaFiltroEnvios?Valor1=" + pageInfo + "&Valor2=" + navigationDirection + "", function () {
                //  
            });
        }
        
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
      

    $(document).on("click", ".btnEmail", function (e) {
        e.preventDefault();
        ModalHide();
        var id = $(this).attr('id');
        $.ajax({
            type: "GET",
            url: URL + "SendEmailProveedor?id=" + id + "",
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

    $(document).on("click", "#btnCerrarModalWindows", function (e) {
        $('#ModalWindows').modal('hide');
    });



    $(document).on("click", ".btnSendCancel", function (e) {
        debugger
        e.preventDefault();
        CancelaEnvioId = $(this).attr('id');
        //Modaldialog(GetResouceMessage("CancelSendEnvio"), "btnCancelSendEnvio", id, GetResouceMessage("btnDialogConfirmSendEnvio"), GetResouceMessage("titlConfirmationSendEnvio"));
        $('#ModalWindows').modal('show');
        return false;
    });

    $(document).on("click", "#btnSiCancelarEnvio", function () {
        // code
        //var id = $("#btnDelete").attr('id');
        debugger
        //ModalHide();
        $('#ModalWindows').modal('hide');
        $.ajax({
            type: "GET",
            url: URL + "CancelSendEnvio?id=" + CancelaEnvioId + "",
            success: function (response) {
                hideLoading();
                //showTostMessage(response.success, response.message);
                reloadView();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("some error");
            }
        });

        
    });

});

