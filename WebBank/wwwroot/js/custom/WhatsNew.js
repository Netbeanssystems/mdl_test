try {
    $(function () {
        GetFirstLayerMenuList();
        $("#SortList").sortable({
            axis: 'y',
            containment: "parent"
        }).disableSelection();

    });


    function GetFirstLayerMenuList() {
        $("#FirstLabelMenu").empty();

        $.getJSON(`?handler=FirstLayerMenuList`, (data) => {

            if (data != null) {
                $("#FirstLabelMenu").append("<option value='0'>Select Heading</option>");
                $("#Priority").css('display', 'block');
                BindPriority(data);
                $.each(data, function (i, item) {
                    $("#FirstLabelMenu").append(`<option asp-for="${item.id}" data-HIN="${item.englishHeadingName}" data-OTH="${item.englishHeadingName}"  value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`);
                });
            }
            else {
                $("#Priority").css('display', 'none');
                $("#FirstLabelMenu").append("<option value='0'>Not Found Any Heading</option>");
                $('#toponeEdit').hide();
                $('#topone').hide();
            }
        });
        $(".table > tbody").empty('tr');
    }
    $(".Menu").on("change", function () {
        var categoryId = $(this).val();
        if (categoryId != 0) {; 
            $('#toponeEdit').attr('href', '/bank/Admin/WhatsNew/Edit/' + categoryId);
            $('#topone').attr('href', '/bank/Admin/WhatsNew/Add/' + categoryId);
            $.getJSON('?handler=SubCategories&CategoryId=' + categoryId + '', (data) => {
                $("#FF").empty();
                if (data != null) {
                    BindPriority(data);
                    $("#Priority").css('display', 'block');
                    var item1 = "<tr><td style='width: 181px;'> <label class='control-label' for='edit - submitted - codepostal' style='color:#c75a39; font-weight: 500; font - size: 18px'> Sub Menu : </label ></td><td style='width: 341px;'><select name='number' onchange='GetMenuID(this)' id='SubCategoryId'  class='form-control Menu1' ><option value='0'>Select Second Level</option>";
                    $.each(data, function (i, item) {
                        item1 += `<option value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`;
                    });
                    item1 += '</select></td><td><a class="fa fa-edit SubMenuHeadingEdit" asp-page="Edit" title="Edit"></a> <a class="fa fa-plus SubMenuHeadingAdd" title="Add"></a></td></tr><br>';
                    $("#FF").append(item1);
                }
                else {
                    $("#Priority").css('display', 'none');
                    var item1 = "<tr><td><select name='number' id='SubCategoryId' class='form-control Menu1' ><option value='0'>Select Second Level</option>";
                    item1 = +"<option value='0'>Not Found Any Menu</option>"
                    item1 += '</select></td><td></td></tr><br>';
                    $("#FF").append(item1);
                }
            });
        }
        else {
            $('#toponeEdit').attr('href', 'javscript:void(0);');
            $('#topone').attr('href', 'javscript:void(0);');
        }
    });
    $(document).on('change', ".Menu1", function () {
        var categoryId = $(this).val();
        if (categoryId != 0) {
            $(this).closest('tr').find('.SubMenuHeadingEdit').attr('href', '/bank/Admin/WhatsNew/Edit/' + categoryId);
            $(this).closest('tr').find('.SubMenuHeadingAdd').attr('href', '/bank/Admin/WhatsNew/Add/' + categoryId);
            $(this).closest('tr').nextAll().remove();
            $.getJSON(`?handler=SubCategories&categoryId=${categoryId}`, (data) => {
                var item1 = "<tr><td style='width: 181px;'> <label class='control-label' for='edit - submitted - codepostal' style='color:#c75a39; font-weight: 500; font - size: 18px'> Sub Menu : </label ></td><td style='width: 341px;'><select name='number' id='SubCategoryId'  onchange='GetMenuID(this)' class='form-control Menu1' ><option value='0'>Select Second Level</option>";
                if (data != null) {
                    $("#Priority").css('display', 'block');
                    BindPriority(data);

                    $.each(data, function (i, item) {
                        item1 += `<option value="${item.id}">${item.englishHeadingName} (${item.hindiHeadingName})</option>`;
                    });
                    item1 += '</select></td><td><a class="fa fa-edit SubMenuHeadingEdit"></a>&nbsp;&nbsp;<a class="fa fa-plus SubMenuHeadingAdd" disable ><i class=""></a></td></tr><br>';
                    $("#FF").append(item1);
                }
                else {

                    $(this).closest('tr').next().empty();
                    $("#Priority").css('display', 'none');
                }
            });
        }
        else {
            $(this).closest('tr').find('.SubMenuHeadingEdit').attr('href', 'javscript:void(0);');
            $(this).closest('tr').find('.SubMenuHeadingAdd').attr('href', 'javscript:void(0);');
            $(this).closest('tr').nextAll().remove();

        }

    });

    //---------------------------Start Naman Work-------------------------------
    function GetMenuID(value) {

        var ID;
        var TrIndex = $(value).closest('tr').index();
        if (TrIndex >= 0) {
            if ($(value).val() == 0) {
                if (TrIndex == 0) {
                    ID = $('#FirstLabelMenu').select('option:selected').val();
                }
                else {
                    ID = $('.table tr').eq(TrIndex - 1).find('#SubCategoryId').children('option:selected').val();
                }
                $.getJSON(`?handler=SubCategories&categoryId=${ID}`, (data) => {
                    if (data != null) {
                        BindPriority(data);
                        $("#Priority").css('display', 'block');
                    }
                });
            }
            else {
                ID = $(value).children('option:selected').val();
            }
        }
        else {
            ID = $(value).children('option:selected').val();
            if (ID == 0) {
                $.getJSON(`?handler=FirstLayerMenuList`, (data) => {
                    if (data != null) {
                        $(".table > tbody").empty('tr');
                        if (data) {
                            BindPriority(data)
                        }
                    }
                });
            }
        }
        GetMenuContent(value);
    }
    function BindPriority(data) {
        $("#SortList li").remove();
        $.each(data, function (i, item) {
            $("#SortList").append('<li><input type="hidden" name="Priorty" value=\'' + item.id + '\'/> <span  class="ui-icon ui-icon-arrowthick-2-n-s"/>' + item.englishHeadingName + ' (' + item.hindiHeadingName + ')</li>');
        });
    }
    $("#btnPriorty").click(function () {
        $("#btnPriorty").prop("value", "Please Wait...");
        $("#btnPriorty").disableSelection();

        var menuHeadingDTOs = new Array();
        if ($("#SortList").children().length != 0) {
            var i = 1;
            $("input[name=Priorty]").each(function () {
                var HeadingDTO = {}
                HeadingDTO.ID = parseInt($(this).val());
                HeadingDTO.Priority = i;
                menuHeadingDTOs.push(HeadingDTO);
                i++;
            })
            var param = JSON.stringify(menuHeadingDTOs);
            $.ajax({
                url: "/bank/Admin/WhatsNew/Index?handler=Priority",
                type: "GET",
                data: { menuHeadingDTOs: param },
                success: function (data) {

                    var row = $('.table tr').length
                    var ParentId = 0;
                    if (row < 0) {
                        GetFirstLayerMenuList();
                    }
                    else {
                        if (row == 1) {
                            ParentId = $("#FirstLabelMenu").children('option:selected').val();
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
                    $("#alert").html(`<div id="divAlertMessage" class="alert alert-success alert-dismissible" role="alert">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><i class="fa fa-times"></i></button>
                                <p class="m-0">Priority Successfully Set</p>
                            </div>`);

                },
                error: function (data) {
                    $("#alert").html(`<div id="divAlertMessage" class="alert alert-danger alert-dismissible" role="alert">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><i class="fa fa-times"></i></button>
                                <p class="m-0">Priority Not Set</p>
                            </div>`);
                }
            });
        }
        $("#btnPriorty").prop("value", "Set Priorty");

        $("#btnPriorty").disableSelection();
        //alert($("input[name=Priorty]").val())
    });
    function GetMenuContent(value) {

        var ID;
        var TrIndex = $(value).closest('tr').index();
        if (TrIndex >= 0) {
            if ($(value).val() == 0) {
                if (TrIndex == 0) {
                    ID = $('#FirstLabelMenu').select('option:selected').val();
                }
                else {
                    ID = $('.table tr').eq(TrIndex - 1).find('#SubCategoryId').children('option:selected').val();
                }
            }
            else {
                ID = $(value).children('option:selected').val();
            }
        }
        else {
            ID = $(value).children('option:selected').val();
        }
        $('#FirstLblMenuDTO_Id').val(ID);
        $.ajax({
            type: "GET",
            url: "/bank/Admin/WhatsNew/Index?handler=MenuHeading",
            data: { ID: ID },
            success: function (message) {
                var jn = JSON.parse(message);
                $('#FirstLblMenuDTO_EnglishPageLink').val(jn.englishPageLink);
                $('#FirstLblMenuDTO_HindiPageLink').val(jn.hindiPageLink);
                if (jn.englishContentDesc == null) {
                    tinyMCE.get('EnglishContentDesc').setContent(' ');
                }
                else {
                    tinyMCE.get('EnglishContentDesc').setContent(jn.englishContentDesc);
                }
                if (jn.hindiContentDesc == null) {
                    tinyMCE.get('HindiContentDesc').setContent(' ');
                }
                else {
                    tinyMCE.get('HindiContentDesc').setContent(jn.hindiContentDesc);
                }
            },
            error: function () {

            }
        });
    }
    //---------------------------End Naman Work---------------------------------
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