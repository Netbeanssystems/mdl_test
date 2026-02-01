$('button[type="submit"]').click(function (e) {
    e.preventDefault(); // Stop default form submit for now

    var button = $(this);
    var ctrlOldHtml = button.html();
    var parentform = button.parents('form:first');
    debugger
    if (parentform.valid()) {
        button.attr('disabled', true);
        button.html('<i class="fa fa-spinner fa-spin"></i> Please wait');

        var fileInput = parentform.find('input[type="file"]')[0];
        var file = fileInput ? fileInput.files[0] : null;
        var projectId = parentform.find('#hdProjectId').val();
        if (file) {
            var fileSizeInMB = (file.size / 1024 / 1024).toFixed(2).toString();

            // AJAX to save file size
            $.ajax({
                url: '/bidder/Admin/TenderUpload/Manage?handler=UploadQuota',
                type: 'GET',
                data: { projectId: projectId, fileSize: fileSizeInMB },
                success: function (response) {
                    console.log('File size saved:', response);
                    // Now finally submit the form
                    parentform.off('submit').submit();
                },
                error: function (xhr, status, error) {
                    console.error('Error saving file size:', error);
                    button.attr('disabled', false);
                    button.html(ctrlOldHtml);
                }
            });
        } else {
            // No file, just submit immediately
            parentform.off('submit').submit();
        }
    } else {
        button.attr('disabled', false);
        button.html(ctrlOldHtml);
    }
});