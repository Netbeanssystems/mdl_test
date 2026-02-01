try {
    $('#ManageMenuModal').on('show.bs.modal',
        function(event) {
            var id = $(event.relatedTarget).data('id');
            var pid = $(event.relatedTarget).data('pid');
            var url = "?handler=Model";
            if (typeof id != "undefined") {
                $.ajax({
                    type: "GET",
                    url: url,
                    data: { id: id },
                    success: function(data) {
                        if (data != null) {
                            $('#Id').val(data.id);
                            $('#ParentId').val(data.parentId);
                            $('#Sequence').val(data.sequence);
                            $('#MenuText').val(data.menuText);
                            $('#IconClass').val(data.iconClass);
                            $('#faPreview').removeClass().addClass(data.iconClass);
                            $('#PageUrl').val(data.pageUrl);
                            $('#CreatedDate').val(data.createdDate);
                            $('#CreatedBy').val(data.createdBy);
                            $('#IsActive').attr('checked', data.isActive);
                        } else {
                            $("#divClientAlert").addClass("alert-danger");
                            $("#divClientAlert > p.m-0").text("Not found");
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                        }
                    }
                });
            } else {
                $('#Id').val("0");
                $('#ParentId').val(pid);
                $('#Sequence').val('');
                $('#MenuText').val('');
                $('#IconClass').val('');
                $('#faPreview').removeClass().addClass('');
                $('#PageUrl').val('');
                $("#CreatedDate").val((new Date()).toISOString().split('T')[0]);
                $('#IsActive').attr('checked', true);
            }
        });
        $('#IconClass').change(function () {
            var faclass = $('#IconClass').val();
            $('#faPreview').removeClass().addClass(faclass);
        });
        function Delete(id) {
            getConfirm("Are you sure you want to delete?", function (result) {
                if (result) {
                    $.ajax({
                        type: "POST",
                        url: "?handler=Delete",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        dataType: "json",
                        data: { id: id },
                        success: function (data) {
                            if (data != null) {
                                if (data === "unauthorized") {
                                    $("#divClientAlert").addClass("alert-warning");
                                    $("#divClientAlert > p.m-0").text("Please login");
                                    $("#divClientAlert").show();
                                    SetTimeOut($("#divClientAlert"));
                                    window.location.href = "/Account/Login";
                                } else if (data === "success"){
                                    $("#divClientAlert").addClass("alert-success");
                                    $("#divClientAlert > p.m-0").text("Deleted successfully");
                                    $("#divClientAlert").show();
                                    SetTimeOut($("#divClientAlert"));
                                    window.location.reload(true);
                                } else if (data === "fail"){
                                    $("#divClientAlert").addClass("alert-danger");
                                    $("#divClientAlert > p.m-0").text("Delete failed. There might be active child records.");
                                    $("#divClientAlert").show();
                                    SetTimeOut($("#divClientAlert"));
                                }
                            } else {
                                    $("#divClientAlert").addClass("alert-danger");
                                    $("#divClientAlert > p.m-0").text("Some error occured. Please try again later.");
                                    $("#divClientAlert").show();
                                    SetTimeOut($("#divClientAlert"));
                            }
                        }
                    });
                }
            });
        }
}
catch (e) {
    console.log(e.message);
}