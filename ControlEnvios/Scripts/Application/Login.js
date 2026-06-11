$(function () {
    $("#fromLogin").submit(function () {
        if ($(this).valid()) {
            var isTemsConditionAccept = $("#isTemsConditionAccept").val();
            if (isTemsConditionAccept === "false") {
                AlertModal(GetResouceMessage("Warning"), GetResouceMessage("AcceptTAC"))
                return false;
            }
        }
    });
});