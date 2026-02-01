try {
    //Function to show preview of choosen file to be uploaded
    function PreviewImage(input) {
        var reader = new FileReader();
        var file = input.files[0];
        reader.onload = function (e) {
            var img = $("#imgPreview");
            img.attr("src", e.target.result);
        };
        if (file && file.type.match('image.*')) {
            reader.readAsDataURL(file);
            $('#btnUserImage').attr("disabled", false);
        } else {
            $('#btnUserImage').attr("disabled", true);
        }
    }
    function UploadFile(inputId, postUrl) {
        var fileoldname = $('#UserProfileDto_ProfileImage').val();
        var fileInput = $(inputId).get(0);
        if (fileInput.files.length > 0) {
            $('#btnUserImage i').removeClass('fa-upload').addClass('fa-spinner');
            $('#btnUserImage').attr("disabled", true);
            var formData = new FormData();
            formData.append('UserImage', fileInput.files[0], fileInput.files[0].name);
            formData.append('FileOldName', fileoldname);
            $.ajax(
                {
                    type: "POST",
                    url: postUrl + "?handler=UploadFile",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    data: formData,
                    dataType: "json",
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        if (data != '' && data != null) {
                            if (data === 'unauthorized') {
                                $("#divClientAlert").addClass("alert-warning");
                                $("#divClientAlert > p.m-0").text("Please login to access this resource");
                                $("#divClientAlert").show();
                                SetTimeOut($("#divClientAlert"));
                                window.location.href = "/Account/Login";
                            } else {
                                $('#UserProfileDto_ProfileImage').val(data);
                                $('#btnUserImage i').removeClass('fa-spinner').addClass('fa-upload');
                                $('#btnUserImage').attr("disabled", false);
                                $("#divClientAlert").addClass("alert-success");
                                $("#divClientAlert > p.m-0").text("File uploaded successfully");
                                $("#divClientAlert").show();
                                SetTimeOut($("#divClientAlert"));
                                $('#UserProfileDto_ProfileImage').parents('form:first').submit();
                            }
                        } else {
                            //$('#UserProfileDto_ProfileImage').val('default_user100.png');
                            $('#btnUserImage i').removeClass('fa-spinner').addClass('fa-upload');
                            $('#btnUserImage').attr("disabled", false);
                            $("#divClientAlert").addClass("alert-danger");
                            $("#divClientAlert > p.m-0").text("File upload failed");
                            $("#divClientAlert").show();
                            SetTimeOut($("#divClientAlert"));
                        }
                    },
                    error: function (err) {
                        //$('#UserProfileDto_ProfileImage').val('default_user100.png');
                        $('#btnUserImage i').removeClass('fa-spinner').addClass('fa-upload');
                        $('#btnUserImage').attr("disabled", false);
                        $("#divClientAlert").addClass("alert-danger");
                        $("#divClientAlert > p.m-0").text(err.responseText);
                        $("#divClientAlert").show();
                        SetTimeOut($("#divClientAlert"));
                    }
                }
            );
        } else {
            $("#divClientAlert").addClass("alert-warning");
            $("#divClientAlert > p.m-0").text("Please select a file first");
            $("#divClientAlert").show();
            SetTimeOut($("#divClientAlert"));
        }
    }
    function DeleteFile(inputId, baseUrl) {
        var fileoldname = $('#UserProfileDto_ProfileImage').val();
        if (fileoldname != '' && fileoldname != 'default_user100.png') {
            getConfirm('Are you sure you want to delete?',
                function (result) {
                    if (result) {
                        $('#delUserImage i').removeClass('fa-times').addClass('fa-spinner');
                        $('#delUserImage').attr("disabled", true);
                        $.ajax(
                            {
                                type: "POST",
                                url: "?handler=DeleteFile",
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                                },
                                data: { fileoldname: fileoldname },
                                dataType: "json",
                                success: function (data) {
                                    if (data === 'unauthorized') {
                                        $("#divClientAlert").addClass("alert-warning");
                                        $("#divClientAlert > p.m-0").text("Please login to access this resource");
                                        $("#divClientAlert").show();
                                        SetTimeOut($("#divClientAlert"));
                                        window.location.href = "/Account/Login";
                                    } else if (data === 'true') {
                                        $('#UserProfileDto_ProfileImage').val('default_user100.png');
                                        $(inputId).val('');
                                        $('img#imgPreview').attr("src", baseUrl + '/img/users/default_user100.png');
                                        $('#delUserImage i').removeClass('fa-spinner').addClass('fa-times');
                                        $('#delUserImage').attr("disabled", false);
                                        $("#divClientAlert").addClass("alert-success");
                                        $("#divClientAlert > p.m-0").text("File deleted successfully");
                                        $("#divClientAlert").show();
                                        SetTimeOut($("#divClientAlert"));
                                        $('#UserProfileDto_ProfileImage').parents('form:first').submit();
                                    } else {
                                        $('#btnUserImage i').removeClass('fa-spinner').addClass('fa-times');
                                        $('#btnUserImage').attr("disabled", false);
                                        $("#divClientAlert").addClass("alert-danger");
                                        $("#divClientAlert > p.m-0").text("File deletion failed");
                                        $("#divClientAlert").show();
                                        SetTimeOut($("#divClientAlert"));
                                    }
                                },
                                error: function (err) {
                                    $('#btnUserImage i').removeClass('fa-spinner').addClass('fa-times');
                                    $('#btnUserImage').attr("disabled", false);
                                    $("#divClientAlert").addClass("alert-danger");
                                    $("#divClientAlert > p.m-0").text("File deletion failed");
                                    $("#divClientAlert").show();
                                    SetTimeOut($("#divClientAlert"));
                                }
                            }
                        );
                    }
                });
        } else {
            $("#divClientAlert").addClass("alert-warning");
            $("#divClientAlert > p.m-0").text("Please upload a file first");
            $("#divClientAlert").show();
            SetTimeOut($("#divClientAlert"));
        }
    }

    //$(document).ready(function () {
    //    var projectId = $('#UserProfileDto_ProjectId').val();
    //    var selectedYardId = $('#HDUserProfileDto_YardId').val(); // We'll store the selected yard in a data attribute

    //    if (projectId) {
    //        loadYards(projectId, selectedYardId);
    //    }

    //    $('#UserProfileDto_ProjectId').on('change', function () {
    //        var projectId = $(this).val();
    //        $('#UserProfileDto_YardId').html('<option value="">Loading...</option>');

    //        if (projectId) {
    //            loadYards(projectId, null); // Pass null for yardId in change mode
    //        } else {
    //            $('#UserProfileDto_YardId').html('<option value="">-select-</option>');
    //        }
    //    });

    //    function loadYards(projectId, selectedYardId) {
    //        $.ajax({
    //            url: '/bidder/Account/Profile?handler=GetYardsByProject',
    //            type: 'GET',
    //            data: { projectId: projectId },
    //            success: function (data) {
    //                $('#UserProfileDto_YardId').empty().append('<option value="">-select-</option>');
    //                $.each(data, function (i, yard) {
    //                    var option = $('<option></option>')
    //                        .val(yard.id)
    //                        .text(yard.yardNumber);
    //                    if (selectedYardId && yard.id == selectedYardId) {
    //                        option.prop('selected', true);
    //                    }
    //                    $('#UserProfileDto_YardId').append(option);
    //                });
    //            },
    //            error: function () {
    //                $('#UserProfileDto_YardId').html('<option value="">-select-</option>');
    //                alert('Failed to load yards.');
    //            }
    //        });
    //    }
    //});
}
catch (e) {
    console.log(e.message);
}