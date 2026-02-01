$(document).ready(function () {
    $('#user_ProjectIds').trigger("change");
});
$('#user_Email').on('blur', function () {
    var email = $(this).val();
    if (email.length > 0) {
        $.get("/bidder/Admin/CommercialEx/Manage?handler=CheckEmail", { email: email })
            .done(function (data) {
                if (data === true) {
                    $("#divClientAlert").addClass("alert-danger");
                    $("#divClientAlert > p.m-0").text("This email has already been registered.");
                    $("#divClientAlert").show();
                    SetTimeOut($("#divClientAlert"));
                    $('#user_Email').removeClass('valid').addClass('input-validation-error');
                    $('#user_Email').next('div.input-group-append').children('div.input-group-text')
                        .css('border-color', '#dc3545');
                    $('#user_Email').val('');
                }
            });
    }
});

$(function () {
    $("#user_Name").keypress(function (e) {
        var keyCode = e.keyCode || e.which;

        $("#usererror").html("");

        //Regex for Valid Characters i.e. Alphabets and Numbers.
        var regex = /^[ A-Za-z0-9 .]*$/
        //Validate TextBox value against the Regex.
        var isValid = regex.test(String.fromCharCode(keyCode));
        if (!isValid) {
            $("#usererror").html("Invalid Input.");
        }

        return isValid;
    });
});

$('#user_ProjectIds').on('change', function () {
    var projectIdArray = $(this).val(); // This is assumed to be an array of strings
    var projectId = Array.isArray(projectIdArray) ? projectIdArray.join(',') : projectIdArray;
    $('#user_YardIds').html('<option value="">Loading...</option>');
    if (projectId) {
        $.ajax({
            url: '/bidder/Admin/CommercialEx/Manage?handler=GetYardsByProject', // Adjust to match your endpoint
            type: 'GET',
            data: { projectId: projectId },
            success: function (data) {
                var selectedYardIdStr = $('#user_YardId').val();
                var selectedYardIdArr = selectedYardIdStr ? selectedYardIdStr.split(',').map(id => id.trim()) : [];
                $('#user_YardIds').empty().append('<option value="">-select-</option>');

                $.each(data, function (i, yard) {
                    $('#user_YardIds').append('<option value="' + yard.id + '">' + yard.yardNumber + '</option>');
                });

                // Reassign the previously selected value(s) if they exist
                if (selectedYardIdArr) {
                    $('#user_YardIds').val(selectedYardIdArr);
                }
            },
            error: function () {
                $('#user_YardIds').html('<option value="">-select-</option>');
                alert('Failed to load yards.');
            }
        });
    } else {
        $('#user_YardIds').html('<option value="">-select-</option>');
    }
});
$(document).ready(function () {
    $('.js-example-basic-multiple').select2();
});