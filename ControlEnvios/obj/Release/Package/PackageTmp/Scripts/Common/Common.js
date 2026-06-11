var BaseURL = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port;
var LoginUrl = BaseURL + "/Account/Login";
var datepikerOptions = {
    autoclose: true
    , format: getLocaleDateString(),
}

var multiselectOptions = {
    includeSelectAllOption: true,
    enableFiltering: true
}

function GetResouceMessage(key) {
    var currentLanguage = typeof (localStorage.getItem("CurrentCulture")) == "undefined" ? 'en_US' : localStorage.getItem("CurrentCulture").toString().toLowerCase().replace('-', '_');
    if (Languages[currentLanguage] != undefined && Languages[currentLanguage][key] != undefined)
        return Languages[currentLanguage][key];
    else
        return Languages['en_US'][key];
}

//function to show success or error message after getting response
function showTostMessage(isSuccess, message) {
    //toastr.options.onHidden = callback;
    if (isSuccess) {
        toastr.success(message, GetResouceMessage("Success"), { timeOut: 3000 })
    } else {
        toastr.error(message, GetResouceMessage("Error"), { timeOut: 3000 })
    }
}
jQuery.ajaxSetup({ cache: false });

jQuery(document).ajaxStart(function () {
    showLoading();
}).ajaxStop(function () {
    $('#content').animate({ scrollTop: "0px" }, 500);
    hideLoading();
});

$(document).ajaxError(function (e, jqxhr, settings, exception) {
    e.stopPropagation();
    if (jqxhr != null)
        alert(jqxhr.responseText);
});

jQuery(document).ready(function () {

    //set culture value

    //alert($("#CurrentCulture").val());
    KeyPressValidation();//assign key press validation to input and textarea    

    jQuery(".local-datetime").each(function () {
        jQuery(this).localDatetime();
    });
    jQuery(".local-date").each(function () {
        jQuery(this).localDate();
    });

    jQuery("#Language").change(function () {
        var culture = this.value;
        window.location.href = WebBaseUri + culture + "/Login/Login";
    });
    //if (UICULTURE != "") {
    //    jQuery("#Language").val(UICULTURE);
    //}
    //GetClientResoruceValue();
});

function GetClientResoruceValue() {
    var formData = {
        cultureCode: UICULTURE
    };
    AjaxCall({ url: 'Login/GetResourcevalues', postData: formData, httpMethod: 'POST', dataType: 'JSON', successCallBackFunction: 'OnResourceSuccess' });
}
function OnResourceSuccess(response) {
    localStorage.setItem('ValidationMessages', response);
}

function AjaxCall(options) {
    jQuery.ajax({
        type: options.httpMethod,
        url: options.url,
        data: options.postData,
        cache: options.cache == undefined ? true : options.cache,
        global: options.showLoading == undefined ? true : options.showLoading,
        dataType: options.dataType,
        contentType: options.contentType == undefined ? "application/x-www-form-urlencoded;charset=UTF-8" : options.contentType,
        async: options.isAsync == undefined ? true : options.isAsync,
        success: function (data) {
            alert(data);
            if (data.Status != undefined && data.Status == "VALIDATION_ERROR") { // handle server side errors
                ShowServerErrors(data.Data);
            }
            else if (data.Status != undefined && data.Status == "SESSION_TIMEOUT" || data == "{\"Status\":\"SESSION_TIMEOUT\"}") { // handle server side errors
                window.location = LoginUrl;
            }
            else if (data.Status != undefined && data.Status == "EXCEPTION") { // handle service broker or service side errors
                AlertModalApplicationError(GetMessage("Error"), GetMessage("ErrorMessage"));
            }
            else {
                if (options.successCallBackFunction != '') {
                    if (options.params != undefined) {
                        eval("new function () {" + options.successCallBackFunction + "(data," + options.params + ")}");
                    }
                    else {
                        eval(options.successCallBackFunction + '(data)');
                    }
                }
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            try {
                var reqJson = jQuery.parseJSON(xhr.responseText);
                if (!UserAborted(xhr)) {
                    if (xhr.status == 403) { // invalid user - redirect to login
                        window.location = LoginUrl;
                    }
                    else {
                        AlertModalApplicationError(GetMessage("Error"), reqJson.ErrorMessage);
                    }
                }
            }
            catch (e) {
                AlertModalApplicationError(GetMessage("Error"), GetMessage("ErrorMessage"));
            }
        }
    });
}
// invalid user exception checking
function UserAborted(xhr) {
    return !xhr.getAllResponseHeaders();
}

function AlertModalApplicationError(title, message, callback) {

    alert(message);

    //var alert = jQuery('<div title="' + title + '"><i class="glyphicon glyphicon-remove-circle redicon alertIconSize"></i><span>' + message + '</span></div>');
    //alert.dialog({
    //    width: 450,
    //    bgiframe: true,
    //    resizable: false,
    //    draggable: true,
    //    dialogClass: 'alertModalPadding',
    //    beforeClose: function () {
    //        alert.remove();
    //        if (callback) {
    //            callback();
    //        }
    //    },
    //    modal: true,
    //    buttons: {
    //        //AddTicket: {
    //        //    text: GetMessage("AddTicket"),
    //        //    'class': 'btn btn-primary',
    //        //    click: function () {
    //        //        jQuery(".ui-dialog-content").dialog("close");
    //        //        var GetTicketsByIdUrlCommon = BASEPATHURL + "/TS/GetTicketsById";
    //        //        OpenDialog({ url: GetTicketsByIdUrlCommon, title: 'AddTicket', saveCallback: 'SaveTicketsCommon', width: 900, height: 600 });
    //        //    }
    //        //},
    //        OK: {
    //            text: GetMessage("OK"),
    //            'class': 'btn btn-primary',
    //            click: function () {
    //                jQuery(this).dialog('close');
    //            }
    //        }
    //    }
    //});
    //alert.attr('tabIndex', -1).css('outline', 0).focus().keydown(function (ev) {
    //    if (ev.keyCode && ev.keyCode == jQuery.ui.keyCode.ENTER) {
    //        alert.dialog('close');
    //    }
    //});
}

function GetMessage(key) {
    var ValidationMessages = JSON.parse(localStorage.getItem("ValidationMessages"));
    return ValidationMessages[key];
}

function showLoading() {
    jQuery("#loadingDiv").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function hideLoading() {
    jQuery("#loadingDiv").modal("hide");
}

function BindSelectOptions(selectElement, data) {

    jQuery(selectElement).find('option')
                    .remove()
                    .end()
                    .append('<option value="0">' + GetMessage("Select") + '</option>')
                    .val('0');

    $.each(data.Data, function (Value, Name) {
        jQuery(selectElement)
            .append(jQuery("<option></option>")
                       .attr("value", Name.Value)
                       .text(Name.Name));
    });
}

function KeyPressValidation() {
    jQuery('[data="integer"]').keypress(function (event) {
        return Integer(this, event);
    }).bind('paste', function (e) {
        return false;
    });
    jQuery('[data="digit"]').keypress(function (event) {
        return Digit(this, event);
    }).bind('paste', function (e) {
        return false;
    });
    jQuery('[data="numeric"]').keypress(function (event) {
        return Numeric(this, event);
    }).bind('paste', function (e) {
        return false;
    });
    jQuery('[data="positiveNumeric"]').keypress(function (event) {
        return PositiveNumeric(this, event);
    }).bind('paste', function (e) {
        return false;
    });
    jQuery('[data="alphaNumeric"]').keypress(function (event) {
        return AlphaNumeric(this, event);
    }).bind('paste', function (e) {
        return false;
    });
    jQuery('[data="alphaNumericWithSpace"]').keypress(function (event) {
        return AlphaNumericWithSpace(this, event);
    }).bind('paste', function (e) {
        return false;
    });
}

function Digit(control, event) {
    var keyCode = (event.which) ? event.which : (window.event) ? window.event.keyCode : -1;
    if (keyCode >= 48 && keyCode <= 57 || keyCode == 8 || keyCode == 9 || keyCode == 13 || keyCode == 27 || keyCode == -1) {
        return true;
    }
    else {
        return false;
    }
}

function Integer(control, event) {
    var keyCode = (event.which) ? event.which : (window.event) ? window.event.keyCode : -1;
    if (keyCode >= 48 && keyCode <= 57 || keyCode == 45 || keyCode == 8 || keyCode == 9 || keyCode == 13 || keyCode == 27 || keyCode == -1) {
        if (keyCode == 45) {
            if (control.value.indexOf("-") == -1) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    }
    else {
        return false;
    }
}

function Numeric(control, event) {
    var keyCode = (event.which) ? event.which : (window.event) ? window.event.keyCode : -1;
    if (keyCode >= 48 && keyCode <= 57 || keyCode == 39 || keyCode == 183 || keyCode == 46 || keyCode == 45 || keyCode == 44 || keyCode == 8 || keyCode == 9 || keyCode == 13 || keyCode == 27 || keyCode == -1) {
        if (keyCode == 45) {
            if (control.value.indexOf("-") == -1) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    }
    else {
        return false;
    }
}

function AlphaNumeric(control, event) {
    if (event.charCode != 0) {
        var regex = new RegExp("^[a-zA-Z0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
    var key = event.which || event.keyCode;
}

function AlphaNumericWithSpace(control, event) {
    if (event.charCode != 0) {
        var regex = new RegExp("^[a-zA-Z0-9\\s]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
    var key = event.which || event.keyCode;
}

function PositiveNumeric(control, event) {
    var keyCode = (event.which) ? event.which : (window.event) ? window.event.keyCode : -1;
    if (keyCode >= 48 && keyCode <= 57 || keyCode == 39 || keyCode == 46 || keyCode == 44 || keyCode == 8 || keyCode == 9 || keyCode == 13 || keyCode == 27 || keyCode == -1) {
        return true;
    }
    else {
        return false;
    }
}

function AlertModal(title, messages) {
    jQuery('#alertDiv').find("#alertModalTitle").text(title);
    jQuery('#alertDiv').find("#alertModalbody").text(messages);
    jQuery('#alertDiv').find("#alertModalCloseButton").text(GetResouceMessage("Close"));

    jQuery('#alertDiv').modal({
        keyboard: true
    })

    jQuery('#alertDiv').modal("show");
}

function ConfirmModel(title, message, okFn, cacnelFn, paramsOk) {
    jQuery('#confirmDelete').find('.modal-title').text(title);
    jQuery('#confirmDelete').find('.modal-body p').text(message);
    jQuery('#confirmDelete').modal('show');
    jQuery("#btnOk").off('click').click(function () {
        jQuery('#confirmDelete').modal("hide");
        okFn(paramsOk);

    });
    jQuery("#btnCancel").off('click').click(function () {
        jQuery('#confirmDelete').modal("hide");
        cacnelFn();
    });
}

function ShowServerErrors(modalStateErrors) {
    jQuery('input, textarea, select').parent().removeClass("has-error");
    var messages = "";
    jQuery(modalStateErrors).each(function (i, e) {
        jQuery('[name="' + e.Key + '"]').parent().addClass("has-error");
        messages += "<li>" + e.Value[0] + "</li>";
    });
    //messages = "<div><h5><i class='glyphicon glyphicon-remove-circle redicon alertIconSize'></i> " + GetMessage("ErrorTitle") + "</h5><ul>" + messages + "</ul></div>";
    messages = "<div><ul>" + messages + "</ul></div>";
    //AlertModal(GetMessage("Error"), messages, function () { }, "NoIcon")
    AlertModal(GetMessage("Error"), messages)
}

function ShowClientErrors(title, messageList) {
    debugger;
    jQuery('input, textarea, select').parent().removeClass("has-error");

    var messages = "";
    for (var i = 0; i < messageList.length; i++) {
        jQuery('[name="' + messageList[i].Field + '"]').parent().addClass("has-error");
        messages += "<li>" + messageList[i].Message + "</li>";
    }

    //messages = "<div><h5><i class='glyphicon glyphicon-remove-circle redicon alertIconSize'></i> " + GetMessage("ErrorTitle") + "</h5><ul>" + messages + "</ul></div>";
    messages = "<div><ul>" + messages + "</ul></div>";
    //AlertModal(GetMessage("Error"), messages, function () { }, "NoIcon")
    AlertModal(title, messages)
}

(function () {
    (function ($) {
        return $.fn.localDatetime = function () {
            var date = new Date(jQuery(this).html() + ' UTC');
            jQuery(this).html(date.toLocaleString(UICULTURE, { hour12: true }));
        };
    })(jQuery);
    (function ($) {
        return $.fn.localDate = function () {
            var date = new Date(jQuery(this).html() + ' UTC');
            jQuery(this).html(date.toLocaleDateString(UICULTURE));
        };
    })(jQuery);
}).call(this);

function GetDateFromJSON(dateString) {
    return new Date(parseInt(dateString.substr(6)));
}

function getLocaleDateString() {

    var formats = {
        "ar-SA": "dd/mm/yy",
        "bg-BG": "dd.mm.yyyy",
        "ca-ES": "dd/mm/yyyy",
        "zh-TW": "yyyy/m/d",
        "cs-CZ": "d.m.yyyy",
        "da-DK": "dd-mm-yyyy",
        "de-DE": "dd.mm.yyyy",
        "el-GR": "d/m/yyyy",
        "en-US": "mm/d/yyyy",
        "fi-FI": "d.m.yyyy",
        "fr-FR": "dd/mm/yyyy",
        "he-IL": "dd/mm/yyyy",
        "hu-HU": "yyyy. mm. dd.",
        "is-IS": "d.m.yyyy",
        "it-IT": "dd/mm/yyyy",
        "ja-JP": "yyyy/mm/dd",
        "ko-KR": "yyyy-mm-dd",
        "nl-NL": "d-m-yyyy",
        "nb-NO": "dd.mm.yyyy",
        "pl-PL": "yyyy-mm-dd",
        "pt-BR": "d/m/yyyy",
        "ro-RO": "dd.mm.yyyy",
        "ru-RU": "dd.mm.yyyy",
        "hr-HR": "d.m.yyyy",
        "sk-SK": "d. m. yyyy",
        "sq-AL": "yyyy-mm-dd",
        "sv-SE": "yyyy-mm-dd",
        "th-TH": "d/m/yyyy",
        "tr-TR": "dd.mm.yyyy",
        "ur-PK": "dd/mm/yyyy",
        "id-ID": "dd/mm/yyyy",
        "uk-UA": "dd.mm.yyyy",
        "be-BY": "dd.mm.yyyy",
        "sl-SI": "d.m.yyyy",
        "et-EE": "d.mm.yyyy",
        "lv-LV": "yyyy.mm.dd.",
        "lt-LT": "yyyy.mm.dd",
        "fa-IR": "mm/dd/yyyy",
        "vi-VN": "dd/mm/yyyy",
        "hy-AM": "dd.mm.yyyy",
        "az-Latn-AZ": "dd.mm.yyyy",
        "eu-ES": "yyyy/mm/dd",
        "mk-MK": "dd.mm.yyyy",
        "af-ZA": "yyyy/mm/dd",
        "ka-GE": "dd.mm.yyyy",
        "fo-FO": "dd-mm-yyyy",
        "hi-IN": "dd-mm-yyyy",
        "ms-MY": "dd/mm/yyyy",
        "kk-KZ": "dd.mm.yyyy",
        "ky-KG": "dd.mm.yy",
        "sw-KE": "m/d/yyyy",
        "uz-Latn-UZ": "dd/mm yyyy",
        "tt-RU": "dd.mm.yyyy",
        "pa-IN": "dd-mm-yy",
        "gu-IN": "dd-mm-yy",
        "ta-IN": "dd-mm-yyyy",
        "te-IN": "dd-mm-yy",
        "kn-IN": "dd-mm-yy",
        "mr-IN": "dd-mm-yyyy",
        "sa-IN": "dd-mm-yyyy",
        "mn-MN": "yy.mm.dd",
        "gl-ES": "dd/mm/yy",
        "kok-IN": "dd-mm-yyyy",
        "syr-SY": "dd/mm/yyyy",
        "dv-MV": "dd/mm/yy",
        "ar-IQ": "dd/mm/yyyy",
        "zh-CN": "yyyy/m/d",
        "de-CH": "dd.mm.yyyy",
        "en-GB": "dd/mm/yyyy",
        "es-MX": "dd/mm/yyyy",
        "fr-BE": "d/mm/yyyy",
        "it-CH": "dd.mm.yyyy",
        "nl-BE": "d/mm/yyyy",
        "nn-NO": "dd.mm.yyyy",
        "pt-PT": "dd-mm-yyyy",
        "sr-Latn-CS": "d.m.yyyy",
        "sv-FI": "d.m.yyyy",
        "az-Cyrl-AZ": "dd.mm.yyyy",
        "ms-BN": "dd/mm/yyyy",
        "uz-Cyrl-UZ": "dd.mm.yyyy",
        "ar-EG": "dd/mm/yyyy",
        "zh-HK": "d/m/yyyy",
        "de-AT": "dd.mm.yyyy",
        "en-AU": "d/mm/yyyy",
        "es-ES": "dd/mm/yyyy",
        "fr-CA": "yyyy-mm-dd",
        "sr-Cyrl-CS": "d.m.yyyy",
        "ar-LY": "dd/mm/yyyy",
        "zh-SG": "d/m/yyyy",
        "de-LU": "dd.mm.yyyy",
        "en-CA": "dd/mm/yyyy",
        "es-GT": "dd/mm/yyyy",
        "fr-CH": "dd.mm.yyyy",
        "ar-DZ": "dd-mm-yyyy",
        "zh-MO": "d/m/yyyy",
        "de-LI": "dd.mm.yyyy",
        "en-NZ": "d/mm/yyyy",
        "es-CR": "dd/mm/yyyy",
        "fr-LU": "dd/mm/yyyy",
        "ar-MA": "dd-mm-yyyy",
        "en-IE": "dd/mm/yyyy",
        "es-PA": "mm/dd/yyyy",
        "fr-MC": "dd/mm/yyyy",
        "ar-TN": "dd-mm-yyyy",
        "en-ZA": "yyyy/mm/dd",
        "es-DO": "dd/mm/yyyy",
        "ar-OM": "dd/mm/yyyy",
        "en-JM": "dd/mm/yyyy",
        "es-VE": "dd/mm/yyyy",
        "ar-YE": "dd/mm/yyyy",
        "en-029": "mm/dd/yyyy",
        "es-CO": "dd/mm/yyyy",
        "ar-SY": "dd/mm/yyyy",
        "en-BZ": "dd/mm/yyyy",
        "es-PE": "dd/mm/yyyy",
        "ar-JO": "dd/mm/yyyy",
        "en-TT": "dd/mm/yyyy",
        "es-AR": "dd/mm/yyyy",
        "ar-LB": "dd/mm/yyyy",
        "en-ZW": "m/d/yyyy",
        "es-EC": "dd/mm/yyyy",
        "ar-KW": "dd/mm/yyyy",
        "en-PH": "m/d/yyyy",
        "es-CL": "dd-mm-yyyy",
        "ar-AE": "dd/mm/yyyy",
        "es-UY": "dd/mm/yyyy",
        "ar-BH": "dd/mm/yyyy",
        "es-PY": "dd/mm/yyyy",
        "ar-QA": "dd/mm/yyyy",
        "es-BO": "dd/mm/yyyy",
        "es-SV": "dd/mm/yyyy",
        "es-HN": "dd/mm/yyyy",
        "es-NI": "dd/mm/yyyy",
        "es-PR": "dd/mm/yyyy",
        "am-ET": "d/m/yyyy",
        "tzm-Latn-DZ": "dd-mm-yyyy",
        "iu-Latn-CA": "d/mm/yyyy",
        "sma-NO": "dd.mm.yyyy",
        "mn-Mong-CN": "yyyy/m/d",
        "gd-GB": "dd/mm/yyyy",
        "en-MY": "d/m/yyyy",
        "prs-AF": "dd/mm/yy",
        "bn-BD": "dd-mm-yy",
        "wo-SN": "dd/mm/yyyy",
        "rw-RW": "m/d/yyyy",
        "qut-GT": "dd/mm/yyyy",
        "sah-RU": "mm.dd.yyyy",
        "gsw-FR": "dd/mm/yyyy",
        "co-FR": "dd/mm/yyyy",
        "oc-FR": "dd/mm/yyyy",
        "mi-NZ": "dd/mm/yyyy",
        "ga-IE": "dd/mm/yyyy",
        "se-SE": "yyyy-mm-dd",
        "br-FR": "dd/mm/yyyy",
        "smn-FI": "d.m.yyyy",
        "moh-CA": "m/d/yyyy",
        "arn-CL": "dd-mm-yyyy",
        "ii-CN": "yyyy/m/d",
        "dsb-DE": "d. m. yyyy",
        "ig-NG": "d/m/yyyy",
        "kl-GL": "dd-mm-yyyy",
        "lb-LU": "dd/mm/yyyy",
        "ba-RU": "dd.mm.yy",
        "nso-ZA": "yyyy/mm/dd",
        "quz-BO": "dd/mm/yyyy",
        "yo-NG": "d/m/yyyy",
        "ha-Latn-NG": "d/m/yyyy",
        "fil-PH": "m/d/yyyy",
        "ps-AF": "dd/mm/yy",
        "fy-NL": "d-m-yyyy",
        "ne-NP": "m/d/yyyy",
        "se-NO": "dd.mm.yyyy",
        "iu-Cans-CA": "d/m/yyyy",
        "sr-Latn-RS": "d.m.yyyy",
        "si-LK": "yyyy-mm-dd",
        "sr-Cyrl-RS": "d.m.yyyy",
        "lo-LA": "dd/mm/yyyy",
        "km-KH": "yyyy-mm-dd",
        "cy-GB": "dd/mm/yyyy",
        "bo-CN": "yyyy/m/d",
        "sms-FI": "d.m.yyyy",
        "as-IN": "dd-mm-yyyy",
        "ml-IN": "dd-mm-yy",
        "en-IN": "dd-mm-yyyy",
        "or-IN": "dd-mm-yy",
        "bn-IN": "dd-mm-yy",
        "tk-TM": "dd.mm.yy",
        "bs-Latn-BA": "d.m.yyyy",
        "mt-MT": "dd/mm/yyyy",
        "sr-Cyrl-ME": "d.m.yyyy",
        "se-FI": "d.m.yyyy",
        "zu-ZA": "yyyy/mm/dd",
        "xh-ZA": "yyyy/mm/dd",
        "tn-ZA": "yyyy/mm/dd",
        "hsb-DE": "d. m. yyyy",
        "bs-Cyrl-BA": "d.m.yyyy",
        "tg-Cyrl-TJ": "dd.mm.yy",
        "sr-Latn-BA": "d.m.yyyy",
        "smj-NO": "dd.mm.yyyy",
        "rm-CH": "dd/mm/yyyy",
        "smj-SE": "yyyy-mm-dd",
        "quz-EC": "dd/mm/yyyy",
        "quz-PE": "dd/mm/yyyy",
        "hr-BA": "d.m.yyyy.",
        "sr-Latn-ME": "d.m.yyyy",
        "sma-SE": "yyyy-mm-dd",
        "en-SG": "d/m/yyyy",
        "ug-CN": "yyyy-m-d",
        "sr-Cyrl-BA": "d.m.yyyy",
        "es-US": "m/d/yyyy"
    };

    return formats[navigator.language] || 'dd/mm/yyyy';
}

//function GetWebURLWithCultureCode() {

//    if (UICULTURE == "") {
//        UICULTURE = "en-US";
//    }

//    return WebBaseUri + UICULTURE + "/";
//}

// Get URL Query string param by key
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}


function Modaldialog(messages, buttonId, value, btnText, title) {
    
    jQuery('#CommonDialog').find("#CommonModelBody").text(messages);
    jQuery('#CommonDialog').find("#btnConfirm").attr('id', buttonId);
    jQuery('#CommonDialog').find("#" + buttonId + "").text(btnText);
    jQuery('#CommonDialog').find("#commonDialogTitle").text(title);
    //set value in hidden text box.
    jQuery('#CommonDialog').find("#txtId").val(value);
    jQuery('#CommonDialog').modal({
        keyboard: true
    })
    jQuery('#CommonDialog').modal("show");
}

function ModalHide() {
    jQuery('#CommonDialog').modal("hide");
}

