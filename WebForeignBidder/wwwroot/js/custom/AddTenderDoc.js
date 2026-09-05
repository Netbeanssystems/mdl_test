let cvFileCount = 0;
let ncvFileCount = 0;
function addNewRow(containerId, count) {
    const container = document.getElementById(containerId);
    const rows = container.getElementsByClassName('form-row row');

    // Check if the last row is saved
    if (rows.length > 0) {
        const lastRow = rows[rows.length - 1];
        const saveButton = lastRow.querySelector('.save-row');

        if (!saveButton.disabled) { // Save button is enabled, meaning the row isn't saved
            showToast("Please save the current row before adding a new one.");
            return count;
        }
    }

    count++;

    const row = document.createElement('div');
    row.className = 'form-group form-row row';

    // Determine the type based on the containerId
    const typeValue = containerId === 'cvfiles' ? 'CV' : 'NCV';

    row.innerHTML = `
        <div class="col-md-1 mt-1">
            <span>${count}</span>
        </div>
        <div class="col-md-3 mt-1">
            <input type="text" class="form-control" minlength="1" maxlength="100" required onkeypress="return lettersNumbersValidate(event)" name="${containerId}FileName${count}" placeholder="Enter Document title" />
        </div>
        <div class="col-md-2 mt-1">
            <input type="file" accept=".pdf, .xls, .xlsx" class="form-control" required name="${containerId}UploadFile${count}" />
        </div>
        <div class="col-md-3 mt-1">
            <input type="text" class="form-control" required name="${containerId}Remarks${count}" />
        </div>
        <div class="col-md-2 mt-1">
        <select class="form-control" required name="${containerId}fileType${count}" >
            <option value="0">--Select--</option>
            <option value="Technical">Technical</option>
            <option value="Financial">Financial</option>
            <option value="Price Bid">Price Bid</option>
            <option value="Miscellaneous">Miscellaneous</option>
        </select>
        </div>
        <div class="col-md-1 text-right mt-1">
            <button type="button" class="btn btn-danger btn-sm remove-row"><i class="fa fa-trash"></i></button>
            <button type="button" class="btn btn-success btn-sm save-row"><i class="fa fa-upload"></i> Save</button>
        </div>
        <input type="hidden" name="fileType${count}" value="${typeValue}" />
    `;

    container.appendChild(row);

    if (containerId === 'cvfiles') {
        cvFileCount++;
        toggleSaveButton('cvSaveContainer', true);
    } else if (containerId === 'ncvfiles') {
        ncvFileCount++;
        toggleSaveButton('ncvSaveContainer', true);
    }

    updateRowNumbers(containerId);

    // Remove row functionality
    row.querySelector('.remove-row').addEventListener('click', function () {
        container.removeChild(row);
        if (containerId === 'cvfiles') {
            cvFileCount--;
            if (cvFileCount === 0) {
                toggleSaveButton('cvSaveContainer', false);
            }
        } else if (containerId === 'ncvfiles') {
            ncvFileCount--;
            if (ncvFileCount === 0) {
                toggleSaveButton('ncvSaveContainer', false);
            }
        }
        updateRowNumbers(containerId);
    });

    // Save row functionality
    row.querySelector('.save-row').addEventListener('click', function () {
        debugger
        const fileName = row.querySelector(`input[name^="${containerId}FileName${count}"]`).value;
        const fileInput = row.querySelector(`input[name^="${containerId}UploadFile${count}"]`).files[0];
        const fileType = row.querySelector(`select[name="${containerId}fileType${count}"]`).value;
        const Remarks = row.querySelector(`input[name="${containerId}Remarks${count}"]`).value;
        const TenderNo = $("#ModelDto_TenderNo").val();

        if (!fileType || fileType === "0") {
            showToast("Please select a Doc Type.");
            return;
        }

        if (fileName && fileInput) {
            const allowedPdfType = 'application/pdf';
            const allowedExcelTypes = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel'];

            let maxSize = 20 * 1024 * 1024; // 20 MB for images and PDFs

            if (allowedExcelTypes.includes(fileInput.type)) {
                maxSize = 70 * 1024 * 1024; // 70 MB for Excel
            }

            // Check if the file type is allowed
            if (![allowedPdfType, ...allowedExcelTypes].includes(fileInput.type)) {
                showToast("Invalid file type. Please upload a PDF or Excel file.");
                return;
            }

            // Check file size
            if (fileInput.size > maxSize) {
                showToast(`File size exceeds the limit. Maximum allowed size is ${maxSize / (1024 * 1024)} MB.`);
                return;
            }

            // Validate Price Bid specific requirements (must be password-protected PDF)
            if (fileType === "Price Bid") {
                if (fileInput.type !== allowedPdfType && !fileInput.name.toLowerCase().endsWith('.pdf')) {
                    showToast("Price Bid document must be a PDF file.");
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (e) {
                    const content = e.target.result;
                    // Check PDF encryption dictionary tag
                    const isEncrypted = content.includes('/Encrypt');
                    if (!isEncrypted) {
                        showToast("Price Bid PDF file must be password protected.");
                        return;
                    }
                    proceedUpload();
                };
                reader.onerror = function () {
                    showToast("Error reading file for password protection validation.");
                };
                reader.readAsText(fileInput.slice(0, Math.min(fileInput.size, 100 * 1024)));
                return;
            }

            proceedUpload();

            function proceedUpload() {
                const modelData = JSON.stringify({
                    TenderNo: TenderNo,
                    DocType: fileType,
                    DocTitle: fileName,
                    Remarks: Remarks
                });

                const formData = new FormData();
                formData.append('model', modelData);
                formData.append('file', fileInput);

                getConfirm('Are you sure you want to upload ' + fileType + ' Documents', function (result) {
                    if (result === true) {
                        $.ajax({
                            type: "POST",
                            url: "?handler=UploadDocuments",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (result) {
                                if (result == "1") {
                                    alert("Document Uploaded Successfully");
                                }
                                else if (result == "9") {
                                    alert("Failed to save the file, because initial files have been already submitted!");
                                }
                                else {
                                    showToast(result);
                                }
                            },
                            error: function (error) {
                                showToast("Server Error!" + error);
                            },
                            complete: function () {
                                $('#loadingDiv').hide();
                            }
                        });
                    }
                    else {
                        event.preventDefault();
                    }
                });
            }



        } else {
            showToast("Please fill in the file name and upload a file.");
        }
    });

    return count;
}

document.getElementById('btnAddResCVFiles').addEventListener('click', function () {
    cvFileCount = addNewRow('cvfiles', cvFileCount);
});

function toggleSaveButton(saveContainerId, show) {
    const saveContainer = document.getElementById(saveContainerId);
    saveContainer.style.display = show ? 'none' : 'none';
}

function updateRowNumbers(containerId) {
    const container = document.getElementById(containerId);
    const rows = container.getElementsByClassName('form-row row');
    for (let i = 0; i < rows.length; i++) {
        const srNoElement = rows[i].querySelector('span');
        srNoElement.textContent = i + 1;
    }
}

function showToast(message) {
    //const toastBody = document.querySelector('#validationToast .toast-body');
    //toastBody.textContent = message;

    //const toastElement = document.getElementById('validationToast');
    //const toast = new bootstrap.Toast(toastElement, { delay: 5000 });
    //toast.show();
    alert(message);
}

function lettersNumbersValidate(key) {

    var keycode = (key.which) ? key.which : key.keyCode;

    if ((keycode >= 48 && keycode <= 57) || (keycode > 64 && keycode < 91) || (keycode > 96 && keycode < 123) || (keycode == 32)) {
        return true;
    }
    else {
        return false;
    }

}