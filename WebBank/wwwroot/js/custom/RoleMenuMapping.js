try {
    $('#tree1 input[type=checkbox]').change(function () {
        $(this).parent().siblings('ul').find(':checkbox').prop('checked', this.checked);
        if (this.checked) {
            $(this).parentsUntil('#tree1', 'ul').siblings(':checkbox').prop('checked', true);
        } else {
            $(this).parentsUntil('#tree1', 'ul').each(function () {
                var $this = $(this);
                var childSelected = $this.find(':checkbox:checked').length;
                if (!childSelected) {
                    $this.prev(':checkbox').prop('checked', false);
                }
            });
        }
    });
    $('select#lstRole').change(function () {
        ;
        var rolename = $(this).val();
        $('input[name="selectMenuName"]').prop('checked', false);
        GetMenus(rolename);
    });
    function GetMenus(rolename) {
        if (rolename === '') {
            $('input[name="selectMenuName"]').prop('checked', false);
            $('#tree1 li:not(.root-branch)').hide();
            $('#tree1 li:not(.branch)').hide();
            $("#divClientAlert").addClass("alert-danger");
            $("#divClientAlert > p.m-0").text("Please select a role");
            $("#divClientAlert").show();
            SetTimeOut($("#divClientAlert"));
        } else {
            $.get('?handler=MenuByRole', { rolename: rolename }, function (Data) {
                if (Data != null && Data === 'unauthorized') {
                    $("#divClientAlert").addClass("alert-warning");
                    $("#divClientAlert > p.m-0").text("Please login to access this resource");
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                    window.location.href = "/Account/Login";
                } else if (Data != null) {
                    $.each(Data, function (k, v) {
                        $('#tree1 input[name="selectMenuName"]').each(function (l, c) {
                            if (c.value == v.id) {
                                $(this).prop('checked', true);
                            }
                        });
                        $('#tree1 li:not(.root-branch)').show();
                        $('#tree1 li:not(.branch)').show();
                    });
                }
                else {
                    $('input[name="selectMenuName"]').prop('checked', false);
                }
            });
        }
    }
    $('#btnSave').click(function (event) {
        var flag = 0;
        var rolename = $('select#lstRole').val();
        if (rolename == '') {
            $("#divClientAlert").addClass("alert-danger");
            $("#divClientAlert > p.m-0").text("Please select a role");
            $("#divClientAlert").show();
            SetTimeOut($("#divClientAlert"));
            event.preventDefault();
            flag++;
            return;
        }
        if ($('input[name="selectMenuName"]:checked').length == 0) {
            $("#divClientAlert").addClass("alert-danger");
            $("#divClientAlert > p.m-0").text("Please select a menu to assign");
            $("#divClientAlert").show();
            SetTimeOut($("#divClientAlert"));
            event.preventDefault();
            flag++;
            return;
        }
        getConfirm('Are you sure you want to update?', function (result) {
            if (flag == 0 && result) {
                var selectedMenu = new Array();
                $('input[name="selectMenuName"]:checked').each(function () {
                    selectedMenu.push(this.value);
                });
                var obj = [];
                for (var i = 0; i < selectedMenu.length; i++) {
                    obj.push({
                        RoleName: rolename,
                        MenuId: selectedMenu[i]
                    });
                }
                var rolemenus = JSON.stringify(obj);
                $.post('?handler=SaveRoleMenus', { roleMenus: rolemenus }, function (data) {
                    if (data != null) {
                        if (data === 'unauthorized') {
                            $("#divClientAlert").addClass("alert-warning");
                            $("#divClientAlert > p.m-0").text("Please login to access this resource");
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                            window.location.href = "/Account/Login";
                        } else {
                            $("#divClientAlert").addClass("alert-success");
                            $("#divClientAlert > p.m-0").text(data);
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                        }
                    }
                });
            } else {
                event.preventDefault();
            }
        });
    });
}
catch (e) {
    console.log(e.message);
}