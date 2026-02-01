/*My Custom JS for Alert Message Server Side*/
window.setTimeout(function () {
    $("#divAlertMessage").attr('class', 'alert').hide("slide", { direction: "left" }, 300).find('p').text('');
}, 5000);
//Global function to set time out on alert divs client side
function SetTimeOut(target) {
    window.setTimeout(function () {
        target.attr('class', 'alert').hide("slide", { direction: "left" }, 300).find('p').text('');
    }, 5000);
}
//for hiding client alerts instead of dismiss
$(function () {
    $("[data-hide]").on("click", function () {
        $(this).closest("." + $(this).attr("data-hide")).attr('class', 'alert').hide().find('p').text('');
    });
});
//for handling opening of menus
$('ul.nav li.nav-item a').each(function () {
    if ($(location).attr("pathname") === '/' && $(this).attr('href') === '/') {
        $(this).addClass('active');
    } else {
        $(this).removeClass('active');
        if ($(location).attr("pathname") === $(this).attr('href') && $(this).attr('href') != '/') {
            $(this).addClass('active');
            var parentli = $(this).parents('ul').parents('li');
            if (parentli.hasClass('has-treeview')) {
                parentli.addClass('menu-open');
                parentli.find('ul').show();
                parentli.find('a').first().addClass('active');
            }
        }
    }
});
//Enable Bootstrap tooltips
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
    //Code added to initialize tooltips on click of + button in datatable to show more columns
    $("#tblDataTable").on('DOMNodeInserted', function (e) {
        $('[data-toggle="tooltip"]').tooltip();
        //    console.log(e.target, ' was inserted');
    });
    $('[data-toggle="tooltip"]').on('click', function () {
        $(this).tooltip('hide');
    });
    ////online or offline
    //if (window.navigator.onLine) {
    //    $("#netStatus").removeClass("text-secondary").addClass("text-success");
    //} else {
    //    $("#netStatus").removeClass("text-secondary").addClass("text-danger");
    //}
});
//Client confirmation modal dialog
function getConfirm(confirmMessage, callback) {
    confirmMessage = confirmMessage || '';
    $('#clientConfirmationModal').modal(
        {
            show: true,
            backdrop: false,
            keyboard: false
        });
    $('#confirmMessage').html(confirmMessage);
    $('#confirmFalse').off().click(function () {
        $('#clientConfirmationModal').modal('hide');
        if (callback) callback(false);
    });
    $('#confirmTrue').off().click(function () {
        $('#clientConfirmationModal').modal('hide');
        if (callback) callback(true);
    });
}
////For removing notification
//function removeNotification(id) {
//    getConfirm('Are you sure you want to delete?', function (result) {
//        if (result) {
//            //$('#notificationModal').modal('toggle');
//            $.ajax({
//                type: "GET",
//                url: "/Index?handler=RemoveNotification",
//                data: { id: id },
//                success: function (data) {
//                    if (data != null) {
//                        $("#liNotifications").html(data);
//                        $("#divClientAlert").addClass("alert-success");
//                        $("#divClientAlert > p.m-0").text("Notification deleted");
//                        window.location.reload(true);
//                    } else {
//                        $("#divClientAlert").addClass("alert-danger");
//                        $("#divClientAlert > p.m-0").text("Notification couldn't be deleted");
//                    }
//                    $("#divClientAlert").show();
//                    SetTimeOut($("#divClientAlert"));
//                }
//            });
//        }
//    });
//}
//$('#notificationModal').on('show.bs.modal', function (event) {
//    $('#notificationModalTitle').text($(event.relatedTarget).data('title'));
//    $('#notificationModalText').text($(event.relatedTarget).data('text'));
//    var targeturl = $(event.relatedTarget).data('url');
//    if (targeturl != '') {
//        $('#targetLink').attr('href', targeturl).show();
//    } else {
//        $('#targetLink').hide();
//    }
//    $('#deleteMe').attr('onclick', 'removeNotification(' + $(event.relatedTarget).data('id') + ')');
//});
//$('#notificationGroupModal').on('show.bs.modal', function (event) {
//    $('#notificationGroupModalTitle').text($(event.relatedTarget).data('title'));
//    //$('#notificationGroupModalText').text($(event.relatedTarget).data('text'));
//    var targeturl = $(event.relatedTarget).data('url');
//    if (targeturl != '') {
//        $('#targetLink').attr('href', targeturl).show();
//    } else {
//        $('#targetLink').hide();
//    }
//    $('#deleteMe').attr('onclick', 'removeNotification(' + $(event.relatedTarget).data('id') + ')');
//});
$("#GlobalAcademicYearId").change(function () {
    var id = parseInt($(this).children("option:selected").val());
    $.getJSON(`/Index?handler=ChangeAcademicYear&IdPP=${id}`, (data) => {
        window.location.reload();
    });
});
$('form').on('blur', 'input[type="text"], textarea', function () {
    // ES6
    // $(this).val((i, value) => value.trim());

    // ES5
    $(this).val(function (i, value) {
        return value.trim();
    });
});

//Show spinner on submit buttons
$('button[type="submit"]').click(function () {
    var ctrlOldHtml = $(this).html();
    var parentform = $(this).parents('form:first');
    //$(this).parents('form:first input[type="text"]').each(function() {
    //    $(this).val(function(i, value) {
    //        return $.trim(value);
    //    });
    //});
    if (parentform.valid()) {
        $(this).attr('disabled', true);
        $(this).html('<i class="fa fa-spinner fa-spin"></i> Please wait');
        parentform.submit();
    } else {
        $(this).attr('disabled', false);
        $(this).html(ctrlOldHtml);
    }
});
function getCurrentDate(format) {
    var output = "";
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    if (format == "DD-MM-YYYY") {
        output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();
    }
    if (format == "YYYY-MM-DD") {
        output = d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
    }
    return output;
}
//To reset ManageModal form and validation errors
$("#ManageModal").on("hide.bs.modal",
    function (event) {
        var $form = $($('#ManageModal form')[0]);
        $form.validate().resetForm();
        $form.find('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').children().remove();
    });
//To allow only --- charaters
$('.no-special-characters').keydown(function (e) {
    var key = e.keyCode | e.which;
    if (key >= 48 && key <= 57 || key == 45 || key == 189 || key == 8) {
        return true;
    } else {
        return false;
    }
});
//Reset the given dropdown with first empty option
function ResetDropdown(dropdown) {
    $(dropdown).empty().append('<option value="">-select-</option>');
};
//Fill given Child Dropdown from path and input
function FillChildDropdown(childDropdown, path, input, errmsg, idasstr, selectedVal) {
    $.get(path, input)
        .done(function (data) {
            if (data) {
                if (data === "unauthorized") {
                    $("#divClientAlert").addClass("alert-warning");
                    $("#divClientAlert > p.m-0").text("Please login");
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                    window.location.href = "/Account/Login";
                } else if (data.indexOf('not found') === -1) {
                    var options = '';
                    if (idasstr === 'idasstr') {
                        $(data).each(function (i, e) {
                            var option = '<option value="' + e.text + '">' + e.text + '</option>';
                            if ((selectedVal != null || selectedVal != '' || selectedVal != 'undefined') && e.text == selectedVal) {
                                option = '<option selected="selected" value="' + e.text + '">' + e.text + '</option>';
                            }
                            options += option;
                        });
                    } else {
                        $(data).each(function (i, e) {
                            var option = '<option value="' + e.id + '">' + e.text + '</option>';
                            if ((selectedVal != null || selectedVal != '' || selectedVal != 'undefined') && e.id == selectedVal) {
                                option = '<option selected="selected" value="' + e.id + '">' + e.text + '</option>';
                            }
                            options += option;
                        });
                    }
                    childDropdown.append(options);
                } else {
                    $("#divClientAlert").addClass("alert-danger");
                    if (errmsg == null || errmsg == '' || errmsg == 'undefined') {
                        $("#divClientAlert > p.m-0").text(data);
                    } else {
                        $("#divClientAlert > p.m-0").text(errmsg);
                    }
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                }
            }
        });
};

//To extend dataTable defaults
$.extend(true, $.fn.dataTable.defaults, {
    "lengthMenu": [[30, 60, 90, 120, 150, -1], [30, 60, 90, 120, 150, "All"]],
    "pageLength": 150,
    "paging": true,
    "lengthChange": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": false,
    "colReorder": true,
    "columnDefs": [{
        "targets": 'noSort',
        "orderable": false
    }],
    "responsive": true,
    dom: "<'row'<'col-sm-6'l><'col-sm-6 text-right'<'d-inline-block'f><'d-inline-block'B>>>" +
        "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-4'i><'col-sm-8'p>>",
    buttons: [
        {
            extend: "colvis",
            className: "btn btn-sm btn-danger ml-2",
            text: '<i class="fa fa-list" data-toggle="tooltip" data-placement="top" title="Show/Hide Columns"></i>',
            exportOptions: {
                columns: "th:not(.noExport)"
            },
            title: null
        },
        {
            extend: "excelHtml5",
            className: "btn btn-sm btn-success ml-2",
            text: '<i class="fa fa-file-excel" data-toggle="tooltip" data-placement="top" title="Download Excel"></i>',
            exportOptions: {
                columns: "th:not(.noExport)"
            },
            title: null
        },
        {
            extend: "pdfHtml5",
            className: "btn btn-sm btn-warning ml-2",
            text: '<i class="fa fa-file-pdf" data-toggle="tooltip" data-placement="top" title="Download PDF"></i>',
            exportOptions: {
                columns: "th:not(.noExport)"
            },
            title: null
        }
        //    {
        //        extend: "print",
        //        className: "btn btn-sm btn-secondary ml-2",
        //        text: '<i class="fa fa-print" data-toggle="tooltip" data-placement="top" title="Print"></i>',
        //        exportOptions: {
        //            columns: "th:not(.noExport)"
        //        }
        //    }
    ],
    "drawCallback": function () {
        $('[data-toggle="tooltip"]').tooltip();
    }
});
//function generateP(rawPass) {
//    var pass = '';
//    var str = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' +
//        'abcdefghijklmnopqrstuvwxyz0123456789@#$';
//    for (var i = 1; i <= 8; i++) {
//        var char = Math.floor(Math.random()
//            * str.length + 1);
//        pass += str.charAt(char);
//    }
//    return pass + rawPass;