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
    function GetUsersByRole(role) {
        if (role != "" && role != "Anonymous") {
            $.get("?handler=UsersByRole", { role: role })
                .done(function (data) {
                    if (data != null) {
                        $("#divUsers").show();
                        var usersHtml = '';
                        $(data).each(function (i, e) {
                            usersHtml += '<p class="badge-pill badge-primary d-inline-block">' +
                                e.userName +
                                ' (' +
                                e.name +
                                ')</p>';
                        });
                        $("#UsersInRole").html(usersHtml);
                    } else {
                        $("#divUsers").hide();
                        $("#UsersInRole").html("");
                    }
                });
        } else {
            $("#divUsers").hide();
            $("#UsersInRole").html("");
        }
    };
    $("#ManageModal").on("show.bs.modal",
        function (event) {
            var roleName = "";
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
                            $("#Name").prop("readonly", true);
                            $("#CanView").attr("checked", data.canView);
                            $("#CanCreate").attr("checked", data.canCreate);
                            $("#CanEdit").attr("checked", data.canEdit);
                            $("#CanDelete").attr("checked", data.canDelete);
                            roleName = data.name;
                        } else {
                            $("#divClientAlert").addClass("alert-danger");
                            $("#divClientAlert > p.m-0").text("Not found");
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                        }
                    },
                    complete: function () {
                        GetUsersByRole(roleName);
                    }
                });
            } else {
                $("#Id").val('');
                $("#Name").val('');
                $("#Name").prop("readonly", false);
                $("#CanView").attr("checked", false);
                $("#CanCreate").attr("checked", false);
                $("#CanEdit").attr("checked", false);
                $("#CanDelete").attr("checked", false);
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
    $(function () {
        $("#Name").keypress(function (e) {
            var keyCode = e.keyCode || e.which;

            $("#nameerror").html("");

            //Regex for Valid Characters i.e. Alphabets and Numbers.
            var regex = /^[ A-Za-z0-9 .]*$/
            //Validate TextBox value against the Regex.
            var isValid = regex.test(String.fromCharCode(keyCode));
            if (!isValid) {
                $("#nameerror").html("Invalid Input.");
            }

            return isValid;
        });
    });
}
catch (e) {
    console.log(e.message);
}