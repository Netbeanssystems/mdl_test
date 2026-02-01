try {
    $(function () {
        GetFirstLayerMenuList();
        $("#SortList").sortable({
            axis: 'y',
            containment: "parent"
        }).disableSelection();
    });
    function GetFirstLayerMenuList() {

        $("#FirstLevelMenu").empty();
        var item1 = "<option value='0'>Select First Level Menu</option>";
        $("div.loading-overlay").show();
        $.getJSON(`?handler=FirstLayerMenuList`, (data) => {
            if (data != null) {
                $("#Priority").css('display', 'block');
                BindPriority(data);
                $.each(data, function (i, item) {
                    item1 += `<option asp-for="${item.id}" data-HIN="${item.englishHeadingName}" data-OTH="${item.englishHeadingName}"  value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`;
                });
                $('#toponeEdit').show();
                $('#topone').show();
                $("div.loading-overlay").hide();
            }
            else {
                $("#Priority").css('display', 'none');
                $('#toponeEdit').hide();
                $('#topone').hide();
                $("div.loading-overlay").hide();
            }
            $("#FirstLevelMenu").append(item1);
        });
    }
    $(".Menu").on("change", function () {
        var categoryId = $(this).val();
        $("#SubCategoryId").empty();
        var item1 = "<option value='0'>Select Second Level Menu</option>";
        if (categoryId != 0) {
            $('#toponeEdit').attr('href', '/bank/Admin/Menuheadings/Edit/' + categoryId);
            $('#topone').attr('href', '/bank/Admin/Menuheadings/Add/' + categoryId);
            $("div.loading-overlay").show();
            $.getJSON('?handler=SubCategories&CategoryId=' + categoryId + '', (data) => {
                if (data != null) {
                    BindPriority(data);
                    $("#Priority").css('display', 'block');
                    $.each(data, function (i, item) {
                        item1 += `<option value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`;
                    });
                    $("div.loading-overlay").hide();
                }
                else {
                    $("#Priority").css('display', 'none');
                    $("div.loading-overlay").hide();
                }
                $("#SubCategoryId").append(item1);
            });
        }
        else {
            $("#SubCategoryId").append(item1);
            $('#toponeEdit').attr('href', 'javscript:void(0);');
            $('#topone').attr('href', 'javscript:void(0);');
        }
    });
    $(".Menu1").on("change", function () {
        var categoryId = $(this).val();
        $("#SubSubCategoryId").empty();
        var item1 = "<option value='0'>Select Second Level Menu</option>";
        if (categoryId != 0) {
            $('#toponeEdit').attr('href', '/bank/Admin/Menuheadings/Edit/' + categoryId);
            $('#topone').attr('href', '/bank/Admin/Menuheadings/Add/' + categoryId);
            $("div.loading-overlay").show();
            $.getJSON('?handler=SubCategories&CategoryId=' + categoryId + '', (data) => {
                if (data != null) {
                    BindPriority(data);
                    $("#Priority").css('display', 'block');
                    $.each(data, function (i, item) {
                        item1 += `<option value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`;
                    });
                    $("div.loading-overlay").hide();
                }
                else {
                    $("#Priority").css('display', 'none');
                    $("div.loading-overlay").hide();
                }
                $("#SubSubCategoryId").append(item1);
            });
        }
        else {
            $("#SubSubCategoryId").append(item1);
            $('#toponeEdit').attr('href', 'javscript:void(0);');
            $('#topone').attr('href', 'javscript:void(0);');
        }
    });

    $(document).on('change', ".Menu1", function () {
        var categoryId = $(this).val();
        if (categoryId != 0) {
            $('#subMenuEdit').attr('href', '/bank/Admin/Menuheadings/Edit/' + categoryId);
            $('#subMenuAdd').attr('href', '/bank/Admin/Menuheadings/Add/' + categoryId);
        }
        else {
            //$("#SubCategoryId").append(item1);
            $('#subMenuEdit').attr('href', 'javscript:void(0);');
            $('#subMenuAdd').attr('href', 'javscript:void(0);');
        }
    });

    $(document).on('change', ".Menu2", function () {
        var categoryId = $(this).val();
        if (categoryId != 0) {
            $('#subsubMenuEdit').attr('href', '/bank/Admin/Menuheadings/Edit/' + categoryId);
            $('#subMenuAdd').attr('href', '/bank/Admin/Menuheadings/Add/' + categoryId);
        }
        else {
            //$("#SubCategoryId").append(item1);
            $('#subsubMenuEdit').attr('href', 'javscript:void(0);');
            $('#subsubMenuEdit').attr('href', 'javscript:void(0);');
        }
    });

    function BindPriority(data) {
        $("#SortList li").remove();
        $.each(data, function (i, item) {
            $("#SortList").append('<li><input type="hidden" name="Priorty" value=\'' + item.id + '\'/> <span  class="ui-icon ui-icon-arrowthick-2-n-s"/>' + item.englishHeadingName + ' (' + item.hindiHeadingName + ')</li>');
        });
    }
    $("#btnPriorty").click(function () {
        ;
        $("#btnPriorty").prop("value", "Please Wait...");
        $("#btnPriorty").disableSelection();
        var menuHeadings = new Array();
        if ($("#SortList").children().length != 0) {
            var i = 1;
            $("input[name=Priorty]").each(function () {
                var HeadingDTO = {}
                HeadingDTO.ID = parseInt($(this).val());
                HeadingDTO.Priority = i;
                menuHeadings.push(HeadingDTO);
                i++;
            })
            var param = JSON.stringify(menuHeadings);
            $("div.loading-overlay").show();
            $.ajax({
                url: "/bank/Admin/MenuHeadings/Index?handler=Priority",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                data: { "menuHeadingDTOs": param },
                dataType: "json",
                success: function (data) {
                    var row = $('.table tr').length
                    var ParentId = 0;
                    if (row < 0) {
                        GetFirstLayerMenuList();
                    }
                    else {
                        if (row == 1) {
                            ParentId = $("#FirstLevelMenu").children('option:selected').val();
                        }
                        else {
                            row = row - 2;
                            ParentId = $('.table tr').eq(row).find('#SubCategoryId').children('option:selected').val();
                        }
                        if (row > 0) {
                            $('.table tr').eq(row - 1).find('#SubCategoryId').empty();
                            $('.table tr').eq(row - 1).find('#SubCategoryId').append(`<option value='0'>Select Menu</option>`)
                            $.getJSON(`?handler=SubCategories&categoryId=${ParentId}`, (data) => {
                                $.each(data, function (i, item) {
                                    $('.table tr').eq(row - 1).find('#SubCategoryId').append(`<option asp-for="${item.id}" data-HIN="${item.englishHeadingName}" data-OTH="${item.englishHeadingName}"  value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`);
                                });
                            })
                        }
                    }
                    $("div.loading-overlay").hide();
                    $("#divClientAlert").addClass("alert-success");
                    $("#divClientAlert > p.m-0").text("Priority set successfully");
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                },
                error: function (data) {
                    $("div.loading-overlay").hide();
                    $("#divClientAlert").addClass("alert-danger");
                    $("#divClientAlert > p.m-0").text("Priority could not be set");
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                }
            });
        }
        $("#btnPriorty").prop("value", "Set Priorty");
        $("#btnPriorty").disableSelection();
    });
    $('ul.dashboard-list li').each(function () {
        var anchorTag = $(this).find('a');
        if (anchorTag.text() === "Add/Update Menu") {
            anchorTag.addClass('active');
        }
    });
}
catch (e) {
    console.log(e);
}