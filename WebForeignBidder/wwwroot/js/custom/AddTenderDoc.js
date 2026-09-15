let maxRows = 12;

$(document).ready(function () {
    // Add default initial row
    addDocRow();

    // Add Row Button
    $('#btnAddDocRow').click(function () {
        addDocRow();
    });

    // Handle Tender/Reference No Change -> Load Uploaded History
    $('#ModelDto_TenderNo').change(function () {
        const tenderNo = $(this).val();
        if (tenderNo) {
            loadUploadedHistory(tenderNo);
        } else {
            $('#uploadedDocsContainer').hide();
            $('#uploadedHistoryBody').empty();
        }
    });

    // Single Submit Button
    $('#btnSubmitTenderDocs').click(function () {
        submitAllDocuments();
    });
});

function addDocRow() {
    const currentRows = $('#tenderDocsBody tr').length;
    if (currentRows >= maxRows) {
        alert("Maximum " + maxRows + " document rows allowed.");
        return;
    }

    const rowIdx = currentRows + 1;
    const tr = `
        <tr data-row="${rowIdx}">
            <td class="text-center align-middle sr-no">${rowIdx}</td>
            <td>
                <input type="text" class="form-control form-control-sm doc-title" placeholder="Enter Document Title" maxlength="200" required />
            </td>
            <td>
                <select class="form-control form-control-sm doc-type" required>
                    <option value="">-- Select Doc Type --</option>
                    <option value="Technical">Technical</option>
                    <option value="Financial">Financial</option>
                    <option value="Price Bid">Price Bid</option>
                    <option value="Miscellaneous">Miscellaneous</option>
                </select>
            </td>
            <td>
                <input type="file" class="form-control-file doc-file" accept=".zip,.rar,.pdf,.xlsx,.xls" required />
                <small class="form-text text-muted file-hint">Allowed: .zip, .rar, .pdf, .xlsx, .xls</small>
            </td>
            <td>
                <input type="text" class="form-control form-control-sm doc-remark" placeholder="Optional remarks..." maxlength="500" />
            </td>
            <td class="text-center align-middle">
                <button type="button" class="btn btn-sm btn-outline-danger btn-remove-row" title="Remove Row">
                    <i class="fa fa-trash"></i>
                </button>
            </td>
        </tr>
    `;

    $('#tenderDocsBody').append(tr);
    updateRowIndices();
}

function updateRowIndices() {
    $('#tenderDocsBody tr').each(function (index) {
        $(this).find('.sr-no').text(index + 1);
    });
    const count = $('#tenderDocsBody tr').length;
    $('#docCountInfo').text(`Rows: ${count} / ${maxRows}`);
}

$(document).on('click', '.btn-remove-row', function () {
    if ($('#tenderDocsBody tr').length === 1) {
        alert("At least one document row is required.");
        return;
    }
    $(this).closest('tr').remove();
    updateRowIndices();
});

$(document).on('change', '.doc-type', function () {
    const selectedType = $(this).val();
    const row = $(this).closest('tr');
    const fileInput = row.find('.doc-file');
    const hint = row.find('.file-hint');

    if (selectedType === 'Price Bid') {
        fileInput.attr('accept', '.pdf,.xlsx,.xls,.xlx');
        hint.html('<span class="text-danger font-weight-bold">Price Bid: Only password-protected .pdf, .xls, .xlsx allowed</span>');
        if (fileInput[0].files && fileInput[0].files.length > 0) {
            const ext = fileInput[0].files[0].name.split('.').pop().toLowerCase();
            if (ext !== 'pdf' && ext !== 'xls' && ext !== 'xlsx' && ext !== 'xlx') {
                fileInput.val('');
                alert("For Price Bid, only password-protected .pdf, .xls, or .xlsx files are allowed. Please select a valid file.");
            }
        }
    } else {
        fileInput.attr('accept', '.zip,.rar,.pdf,.xlsx,.xls');
        hint.text('Allowed: .zip, .rar, .pdf, .xlsx, .xls');
    }
});

$(document).on('change', '.doc-file', function () {
    const row = $(this).closest('tr');
    const docType = row.find('.doc-type').val();
    const file = this.files && this.files[0];
    if (!file) return;

    const ext = file.name.split('.').pop().toLowerCase();
    if (docType === 'Price Bid') {
        if (ext !== 'pdf' && ext !== 'xls' && ext !== 'xlsx' && ext !== 'xlx') {
            alert("For Price Bid, only password-protected .pdf, .xls, or .xlsx files are allowed.");
            $(this).val('');
            return;
        }
    } else {
        const allowed = ['zip', 'rar', 'pdf', 'xls', 'xlsx'];
        if (!allowed.includes(ext)) {
            alert("Invalid file format. Allowed formats: .zip, .rar, .pdf, .xlsx, .xls");
            $(this).val('');
            return;
        }
    }

    if (file.size > 30 * 1024 * 1024) {
        alert("File size exceeds the 30 MB limit.");
        $(this).val('');
        return;
    }
});

function loadUploadedHistory(tenderNo) {
    $.ajax({
        type: 'GET',
        url: '?handler=GetUploadedDocs',
        data: { tenderNo: tenderNo },
        success: function (docs) {
            const tbody = $('#uploadedHistoryBody');
            tbody.empty();

            if (docs && docs.length > 0) {
                docs.forEach((doc, idx) => {
                    const docJson = escapeHtml(JSON.stringify(doc));
                    tbody.append(`
                        <tr>
                            <td class="text-center">${idx + 1}</td>
                            <td>${escapeHtml(doc.docTitle || 'N/A')}</td>
                            <td><span class="badge badge-info">${escapeHtml(doc.docType || 'N/A')}</span></td>
                            <td>${escapeHtml(doc.docName || 'N/A')}</td>
                            <td>${escapeHtml(doc.remarks || '-')}</td>
                            <td>${escapeHtml(doc.createdDate || '-')}</td>
                            <td class="text-center">
                                <button type="button" class="btn btn-sm btn-outline-primary btn-edit-doc mr-1" data-doc='${docJson}' title="Edit Document">
                                    <i class="fa fa-edit"></i>
                                </button>
                                <button type="button" class="btn btn-sm btn-outline-danger btn-delete-doc" data-id="${doc.id}" data-title="${escapeHtml(doc.docTitle || '')}" title="Delete Document">
                                    <i class="fa fa-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `);
                });
                $('#uploadedDocsContainer').show();
            } else {
                tbody.append(`<tr><td colspan="7" class="text-center text-muted">No documents uploaded yet for this tender.</td></tr>`);
                $('#uploadedDocsContainer').show();
            }
        },
        error: function () {
            $('#uploadedDocsContainer').hide();
        }
    });
}

$(document).on('click', '.btn-delete-doc', function () {
    const docId = $(this).data('id');
    const docTitle = $(this).data('title');
    const tenderNo = $('#ModelDto_TenderNo').val();

    if (!docId) return;

    if (!confirm(`Are you sure you want to delete "${docTitle || 'this document'}"?`)) {
        return;
    }

    $.ajax({
        type: "POST",
        url: `?handler=DeleteDocument&id=${docId}`,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (response) {
            if (response && response.success) {
                alert(response.message);
                if (tenderNo) {
                    loadUploadedHistory(tenderNo);
                }
            } else {
                alert(response.message || "Failed to delete document.");
            }
        },
        error: function () {
            alert("Server error occurred while deleting document.");
        }
    });
});

$(document).on('click', '.btn-edit-doc', function () {
    const doc = $(this).data('doc');
    if (!doc) return;

    // Check if there is an empty first row to reuse
    let targetRow = $('#tenderDocsBody tr').last();
    if ($('#tenderDocsBody tr').length === 1 && !targetRow.find('.doc-title').val().trim()) {
        // Reuse current empty row
    } else {
        if ($('#tenderDocsBody tr').length >= maxRows) {
            alert("Maximum " + maxRows + " document rows reached.");
            return;
        }
        addDocRow();
        targetRow = $('#tenderDocsBody tr').last();
    }

    targetRow.find('.doc-title').val(doc.docTitle || '');
    targetRow.find('.doc-type').val(doc.docType || '');
    targetRow.find('.doc-remark').val(doc.remarks && doc.remarks !== 'NA' ? doc.remarks : '');

    $('html, body').animate({
        scrollTop: targetRow.offset().top - 100
    }, 400);
});

function submitAllDocuments() {
    const tenderNo = $('#ModelDto_TenderNo').val();
    if (!tenderNo) {
        alert("Please select a Tender/Reference No.");
        return;
    }

    const rows = $('#tenderDocsBody tr');
    if (rows.length === 0) {
        alert("Please add at least one document row.");
        return;
    }

    let isValid = true;
    let formData = new FormData();
    formData.append("TenderNo", tenderNo);

    rows.each(function (idx) {
        const title = $(this).find('.doc-title').val().trim();
        const type = $(this).find('.doc-type').val();
        const remark = $(this).find('.doc-remark').val().trim();
        const fileInput = $(this).find('.doc-file')[0];

        if (!title) {
            alert(`Row ${idx + 1}: Document Title is required.`);
            isValid = false;
            return false;
        }
        if (!type) {
            alert(`Row ${idx + 1}: Doc Type is required.`);
            isValid = false;
            return false;
        }
        if (!fileInput.files || fileInput.files.length === 0) {
            alert(`Row ${idx + 1}: Please select a document file.`);
            isValid = false;
            return false;
        }

        const file = fileInput.files[0];
        const ext = file.name.split('.').pop().toLowerCase();

        if (type === 'Price Bid') {
            if (ext !== 'pdf' && ext !== 'xls' && ext !== 'xlsx' && ext !== 'xlx') {
                alert(`Row ${idx + 1}: For Price Bid, only password-protected .pdf, .xls, or .xlsx files are allowed.`);
                isValid = false;
                return false;
            }
        } else {
            const allowed = ['zip', 'rar', 'pdf', 'xls', 'xlsx'];
            if (!allowed.includes(ext)) {
                alert(`Row ${idx + 1}: Invalid file format. Allowed formats: .zip, .rar, .pdf, .xlsx, .xls`);
                isValid = false;
                return false;
            }
        }

        if (file.size > 30 * 1024 * 1024) {
            alert(`Row ${idx + 1}: File size exceeds 30 MB.`);
            isValid = false;
            return false;
        }

        formData.append("DocTitles", title);
        formData.append("DocTypes", type);
        formData.append("Remarks", remark);
        formData.append("Files", file);
    });

    if (!isValid) return;

    if (!confirm("Are you sure you want to submit all bid documents?")) {
        return;
    }

    $.ajax({
        type: "POST",
        url: "?handler=UploadDocuments",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response && response.success) {
                alert(response.message);
                // Reset form rows to 1 row
                $('#tenderDocsBody').empty();
                addDocRow();
                // Refresh uploaded history
                loadUploadedHistory(tenderNo);
            } else {
                alert(response.message || "Upload failed.");
            }
        },
        error: function (xhr) {
            alert("Server error occurred during file upload.");
        }
    });
}

function escapeHtml(text) {
    if (!text) return '';
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}