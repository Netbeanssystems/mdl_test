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
                            $("#Year").val(data.year);
                            $("#Description").val(data.description);
                            $("#StartDate").val(data.startDate);
                            $("#StartDate0").data("daterangepicker").setStartDate(new Date(data.startDate));
                            $("#EndDate").val(data.endDate);
                            $("#EndDate0").data("daterangepicker").setStartDate(new Date(data.endDate));
                            $("#CreatedDate").val(data.createdDate);
                            $("#CreatedBy").val(data.createdBy);
                            $("#IsActive").attr("checked", data.isActive);
                            $("#FinancialYearActive").attr("checked", data.financialYearActive);
                            $("#FundRequestAllowed").attr("checked", data.fundRequestAllowed);
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
                $("#Year").val('');
                $("#Description").val('');
                $("#StartDate").val((new Date()).toISOString().split('T')[0]);
                $("#StartDate0").data("daterangepicker").setStartDate(new Date());
                $("#EndDate").val((new Date()).toISOString().split('T')[0]);
                $("#EndDate0").data("daterangepicker").setStartDate(new Date());
                $("#CreatedDate").val((new Date()).toISOString().split('T')[0]);
                $("#IsActive").attr("checked", true);
                $("#FinancialYearActive").attr("checked", false);
                $("#FundRequestAllowed").attr("checked", false);
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
    var today = new Date();
    $("#StartDate0").daterangepicker({
        "showDropdowns": true, //to show dropdowns for Month and year
        "singleDatePicker": true, //to show single date picker not range
        "locale": {
            "format": "DD-MM-YYYY",
            "separator": " - "
        },
        "minDate": today
    },
        function (start, end, label) {
            $('#StartDate').val(start.format('YYYY-MM-DDThh:mm'));
            $('#EndDate').val(start.format('YYYY-MM-DDThh:mm'));
            $('#EndDate0').daterangepicker({
                "showDropdowns": true, //to show dropdowns for Month and year
                "singleDatePicker": true, //to show single date picker not range
                "locale": {
                    "format": "DD-MM-YYYY",
                    "separator": " - "
                },
                "minDate": new Date(Date.parse(start)),
                "startDate": new Date(Date.parse(start))
            },
                function (start1, end1, label1) {
                    $('#EndDate').val(start1.format('YYYY-MM-DDThh:mm'));
                });
        });
    $("#EndDate0").daterangepicker({
        "showDropdowns": true, //to show dropdowns for Month and year
        "singleDatePicker": true, //to show single date picker not range
        "locale": {
            "format": "DD-MM-YYYY",
            "separator": " - "
        },
        "minDate": today
    },
        function (start, end, label) {
            $('#EndDate').val(start.format('YYYY-MM-DDThh:mm'));
        });
}
catch (e) {
    console.log(e.message);
}