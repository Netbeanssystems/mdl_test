try {
    $("#tblDataTable").DataTable({
        initComplete: function () {
            var r = $("#tblDataTable tfoot tr");
            r.find("th").each(function () {
                if ($(this).hasClass("search")) {
                    $(this).css("padding", 5);
                    var title = $(this).text();
                    $(this).html('<input type="text" class="form-control" placeholder="' + title + '" />');
                } else {
                    $(this).html("");
                }
            });
            $("#tblDataTable thead").prepend(r);
            this.api().columns().every(function () {
                var that = this;
                $("input", this.footer()).on("keyup change clear",
                    function () {
                        if (that.search() !== this.value) {
                            that.search(this.value).draw();
                        }
                    });
            });
        }
    });
    $("#ManageModal").on("show.bs.modal",
        function (event) {
            var url = "?handler=Model";
            var id = $(event.relatedTarget).data("id");
            if (typeof id != "undefined") {
                $.ajax({
                    type: "GET",
                    url: url,
                    data: { id: id },
                    success: function (data) {
                        if (data != null) {
                            $("#Id").val(data.id);
                            $("#Name").val(data.name);
                            $("#Code3").val(data.code3);
                            $("#Code2").val(data.code2);
                            $("#Capital").val(data.capital);
                            $("#CurrencyCode").val(data.currencyCode);
                            $("#CreatedDate").val(data.createdDate);
                            $("#CreatedBy").val(data.createdBy);
                            $("#IsActive").attr("checked", data.isActive);
                        } else {
                            $("#divClientAlert").addClass("alert-danger");
                            $("#divClientAlert > p.m-0").text("Not found");
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                        }
                    }
                });
            } else {
                $("#Id").val("0");
                $("#Name").val('');
                $("#Code3").val('');
                $("#Code2").val('');
                $("#Capital").val('');
                $("#CurrencyCode").val('');
                $("#CreatedDate").val((new Date()).toISOString().split('T')[0]);
                $("#IsActive").attr("checked", true);
            }
        });
    $('#btnSave').click(function (event) {
        var theForm = $(this).parents('form:first');
        if (theForm.valid()) {
            getConfirm('Are you sure?', function (result) {
                result === true ? theForm.submit() : event.preventDefault();
            });
        } else {
            event.preventDefault();
        }
    });
    function Delete(id) {
        getConfirm("Are you sure you want to delete?", function (result) {
            if (result) {
                $.ajax({
                    type: "GET",
                    url: "?handler=Delete",
                    data: { id: id },
                    success: function (data) {
                        if (data != null) {
                            if (data === "unauthorized") {
                                $("#divClientAlert").addClass("alert-warning");
                                $("#divClientAlert > p.m-0").text("Please login");
                                $("#divClientAlert").show();
                                SetTimeOut($("#divClientAlert"));
                                window.location.href = "/Account/Login";
                            } else if (data === "success") {
                                $("#divClientAlert").addClass("alert-success");
                                $("#divClientAlert > p.m-0").text("Deleted successfully");
                                $("#divClientAlert").show();
                                SetTimeOut($("#divClientAlert"));
                                window.location.reload(true);
                            } else if (data === "fail") {
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
    //// Handle form submission event
    //$('#frm-example').on('submit', function(e){
    //    var form = this;
    //
    //    var rows_selected = table.column(0).checkboxes.selected();
    //
    //    // Iterate over all selected checkboxes
    //    $.each(rows_selected, function(index, rowId){
    //        // Create a hidden element
    //        $(form).append(
    //            $('<input>')
    //            .attr('type', 'hidden')
    //            .attr('name', 'id[]')
    //            .val(rowId)
    //        );
    //    });
    //});
}
catch (e) {
    console.log(e.message);
}