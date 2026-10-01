function splitHeader(header) {
    return header
        .replace(/([a-z])([A-Z])/g, '$1 $2')
        .replace(/([A-Z]+)([A-Z][a-z])/g, '$1 $2')
        .replace(/\b\w/g, char => char.toUpperCase());
}

let debounceTimer;

function triggerCheckTender() {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(function () {
        const projectId = $('#tenderUpload_ProjectIds').val();
        var projectIdArray = projectId;
        var normalizedValue = Array.isArray(projectIdArray) ? projectIdArray.join(',') : (projectIdArray || '');
        const yardId = $('#tenderUpload_YardIds').val();
        var yardIdArray = yardId;
        var yardNormalizedValue = Array.isArray(yardIdArray) ? yardIdArray.join(',') : (yardIdArray || '');
        const tenderNo = $('#tenderUpload_TenderNo').val() ? $('#tenderUpload_TenderNo').val().trim() : '';

        if (normalizedValue && yardNormalizedValue && tenderNo) {
            $.ajax({
                url: '/bidder/Admin/TenderUpload/Manage?handler=CheckTender',
                type: 'GET',
                data: {
                    projectId: normalizedValue,
                    yardId: yardNormalizedValue,
                    tenderNo: tenderNo
                },
                success: function (response) {
                    const isNewTender = (response.status === "0" || !response.data);
                    $(".card-header h3").html(isNewTender ? "Add Tender" : "Edit Tender");

                    const tender = isNewTender ? {} : (response?.data?.value || response?.data || {});

                    function formatDateTimeForInput(dateVal, $input) {
                        if (!dateVal) return '';
                        try {
                            const d = new Date(dateVal);
                            if (isNaN(d.getTime())) return dateVal;
                            const pad = n => String(n).padStart(2, '0');
                            const year = d.getFullYear();
                            const month = pad(d.getMonth() + 1);
                            const day = pad(d.getDate());
                            const hours = pad(d.getHours());
                            const minutes = pad(d.getMinutes());

                            const dtLocal = `${year}-${month}-${day}T${hours}:${minutes}`;
                            const dtDate = `${year}-${month}-${day}`;

                            const type = $input.attr('type');
                            if (type === 'datetime-local') return dtLocal;
                            if (type === 'date') return dtDate;

                            $input.val(dtLocal);
                            if ($input.val()) return dtLocal;
                            $input.val(dtDate);
                            if ($input.val()) return dtDate;
                            return dateVal;
                        } catch (e) {
                            return dateVal;
                        }
                    }

                    if (isNewTender || tender) {
                        $("#tenderUpload_Id").val(tender.id || tender.Id || 0);
                        $("#tenderUpload_CreatedBy").val(tender.createdBy || tender.CreatedBy || '');
                        $("#tenderUpload_CreatedDate").val(tender.createdDate || tender.CreatedDate || '');
                        $("#tenderUpload_TenderDoc").val(tender.tenderDoc || tender.TenderDoc || '');

                        const rawStartDate = tender.tenderStartDate || tender.TenderStartDate;
                        const rawOpeningDate = tender.tenderOpeningDate || tender.TenderOpeningDate;
                        const rawClosingDate = tender.tenderClosingDate || tender.TenderClosingDate;

                        $("#tenderUpload_TenderStartDate").val(formatDateTimeForInput(rawStartDate, $("#tenderUpload_TenderStartDate")));
                        $("#tenderUpload_TenderOpeningDate").val(formatDateTimeForInput(rawOpeningDate, $("#tenderUpload_TenderOpeningDate")));
                        $("#tenderUpload_TenderClosingDate").val(formatDateTimeForInput(rawClosingDate, $("#tenderUpload_TenderClosingDate")));

                        $("#tenderUpload_TenderDescription").val(tender.tenderDescription || tender.TenderDescription || '');
                        
                        const fbVal = tender.foreignBidderId || tender.ForeignBidderId;
                        if (fbVal) {
                            const selectedBidders = fbVal.split(',').map(s => s.trim());
                            $('#tenderUpload_ForeignBidderIds').val(selectedBidders).trigger('change');
                        }

                        if (!isNewTender && tender.id) {
                            $('input[name="IsNew"]').val('false');
                            $("#tenderUpload_TenderStartDate").prop("readonly", true).addClass("bg-light");
                            $("#tenderUpload_TenderClosingDate").prop("readonly", true).addClass("bg-light");
                            $("#tenderUpload_TenderOpeningDate").prop("readonly", false).removeClass("bg-light");

                            $("#corrigendumOptionContainer").show();

                            // Populate existing documents
                            $('#tenderDocsBody').empty();
                            deletedDocIds = [];
                            $('#tenderUpload_DeletedDocIds').val('');
                            const docs = tender.tenderDocuments || tender.TenderDocuments;
                            if (docs && docs.length > 0) {
                                docs.forEach(doc => {
                                    addTenderDocRow(doc.originalFileName || doc.OriginalFileName, true, doc.id || doc.Id, doc.encryptedFileName || doc.EncryptedFileName, tender.projectId || tender.ProjectId, tender.tenderNo || tender.TenderNo);
                                });
                            } else if (tender.tenderDoc || tender.TenderDoc) {
                                addTenderDocRow("Tender Document", true, 0, tender.tenderDoc || tender.TenderDoc, tender.projectId || tender.ProjectId, tender.tenderNo || tender.TenderNo);
                            } else {
                                addTenderDocRow();
                            }

                            // Populate existing corrigendum documents
                            $('#corrigendumDocsBody').empty();
                            deletedCorrigendumDocIds = [];
                            $('#tenderUpload_DeletedCorrigendumDocIds').val('');
                            const corrigendums = tender.tenderCorrigendums || tender.TenderCorrigendums;
                            if (corrigendums && corrigendums.length > 0) {
                                corrigendums.forEach(cor => {
                                    addCorrigendumDocRow(
                                        cor.originalFileName || cor.OriginalFileName || cor.corrigendumDocDecrypted || cor.CorrigendumDocDecrypted,
                                        true,
                                        cor.id || cor.Id,
                                        cor.hashedFileName || cor.HashedFileName || cor.corrigendumDoc || cor.CorrigendumDoc,
                                        tender.projectId || tender.ProjectId,
                                        tender.tenderNo || tender.TenderNo
                                    );
                                });
                            }
                        } else {
                            $('input[name="IsNew"]').val('true');
                            $("#tenderUpload_TenderStartDate").prop("readonly", false).removeClass("bg-light");
                            $("#tenderUpload_TenderClosingDate").prop("readonly", false).removeClass("bg-light");
                            $("#tenderUpload_TenderOpeningDate").prop("readonly", false).removeClass("bg-light");

                            $("#corrigendumOptionContainer").hide();
                            $('#tenderDocsBody').empty();
                            deletedDocIds = [];
                            $('#tenderUpload_DeletedDocIds').val('');
                            addTenderDocRow();

                            $('#corrigendumDocsBody').empty();
                            deletedCorrigendumDocIds = [];
                            $('#tenderUpload_DeletedCorrigendumDocIds').val('');
                        }

                        const corrigendums = tender.tenderCorrigendums || tender.TenderCorrigendums;

                        if (!isNewTender && corrigendums && Array.isArray(corrigendums) && corrigendums.length > 0) {
                            const thead = $('#corrigendumTable thead');
                            thead.html(`
                                <tr>
                                    <th>Description</th>
                                    <th>Original Document Name</th>
                                    <th>Document File</th>
                                    <th>Extended Date</th>
                                    <th>Created Date</th>
                                    <th>Actions</th>
                                </tr>
                            `);

                            const tbody = $('#corrigendumTable tbody');
                            tbody.empty();

                            const currentProjectId = normalizedValue;
                            const currentTenderNo = tenderNo;

                            corrigendums.forEach(row => {
                                const id = row.id || row.Id || 0;
                                const desc = row.corrigendumDescription || row.CorrigendumDescription || '';
                                const origName = row.originalFileName || row.OriginalFileName || row.corrigendumDocDecrypted || row.CorrigendumDocDecrypted || 'Corrigendum Document';
                                const fileDoc = row.hashedFileName || row.HashedFileName || row.corrigendumDoc || row.CorrigendumDoc || '';
                                const extDate = row.extendedDate || row.ExtendedDate ? new Date(row.extendedDate || row.ExtendedDate).toLocaleString() : '';
                                const crDate = row.createdDate || row.CreatedDate ? new Date(row.createdDate || row.CreatedDate).toLocaleString() : '';

                                let fileLinkHtml = '';
                                if (fileDoc) {
                                    fileLinkHtml = `<a href="/bidder/BidderTenders/${currentProjectId}/${currentTenderNo}/${fileDoc}" target="_blank" class="btn btn-sm btn-outline-primary"><i class="fa fa-download"></i> Download Zip</a>`;
                                }

                                const rowHtml = `
                                    <tr data-id="${id}" data-desc="${desc}" data-origname="${origName}" data-file="${fileDoc}" data-extdate="${row.extendedDate || row.ExtendedDate || ''}">
                                        <td>${desc}</td>
                                        <td>${origName}</td>
                                        <td>${fileLinkHtml}</td>
                                        <td>${extDate}</td>
                                        <td>${crDate}</td>
                                        <td>
                                            <button type="button" class="btn btn-sm btn-outline-info edit-btn mr-1" data-id="${id}" data-desc="${desc}" data-file="${fileDoc}" data-extdate="${row.extendedDate || row.ExtendedDate || ''}"><i class="fa fa-edit"></i> Edit</button>
                                            <button type="button" class="btn btn-sm btn-outline-danger delete-btn" data-id="${id}"><i class="fa fa-trash"></i> Delete</button>
                                        </td>
                                    </tr>
                                `;
                                tbody.append(rowHtml);
                            });

                            if ($.fn.DataTable.isDataTable('#corrigendumTable')) {
                                $('#corrigendumTable').DataTable().destroy();
                            }
                            $('#corrigendumTable').DataTable({
                                responsive: true,
                                autoWidth: false
                            });
                            $('#corrigendumTable').closest('.table-responsive').show();
                        } else {
                            if ($.fn.DataTable.isDataTable('#corrigendumTable')) {
                                $('#corrigendumTable').DataTable().destroy();
                            }
                            $('#corrigendumTable').closest('.table-responsive').hide();
                        }

                    }
                },
                error: function (xhr) {
                    console.error('Error in CheckTender:', xhr.responseText);
                }
            });
        }
    }, 500);
}

$(document).on('input change', '#tenderUpload_TenderNo', triggerCheckTender);
$(document).on('change', '#tenderUpload_ProjectIds, #tenderUpload_YardIds', triggerCheckTender);

let tenderDocRowIndex = 0;
const MAX_DOC_ROWS = 8;
let deletedDocIds = [];

function getActiveRowCount() {
    return $('#tenderDocsBody tr:not(.deleted-row)').length;
}

function updateDocCountInfo() {
    const count = getActiveRowCount();
    $('#docCountInfo').text(`Rows: ${count} / ${MAX_DOC_ROWS}`);
    if (count >= MAX_DOC_ROWS) {
        $('#btnAddDocRow').prop('disabled', true);
    } else {
        $('#btnAddDocRow').prop('disabled', false);
    }
    reindexDocRows();
}

function reindexDocRows() {
    let sr = 1;
    $('#tenderDocsBody tr:not(.deleted-row)').each(function () {
        $(this).find('.doc-sr-no').text(sr++);
    });
}

function addTenderDocRow(docName = '', isExisting = false, existingDocId = 0, encryptedFileName = '', projectId = '', tenderNo = '') {
    if (getActiveRowCount() >= MAX_DOC_ROWS) {
        alert('Maximum 8 document rows allowed.');
        return;
    }

    tenderDocRowIndex++;
    const rowId = `docRow_${tenderDocRowIndex}`;
    let rowHtml = '';

    if (isExisting) {
        rowHtml = `
            <tr id="${rowId}" data-existing-id="${existingDocId}">
                <td class="text-center font-weight-bold doc-sr-no"></td>
                <td>
                    <span>${docName || encryptedFileName}</span>
                </td>
                <td>
                    <a href="/bidder/BidderTenders/${projectId}/${tenderNo}/${encryptedFileName}" target="_blank" class="btn btn-sm btn-outline-info">
                        <i class="fa fa-download"></i> View Document
                    </a>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger btn-remove-existing-doc" data-id="${existingDocId}" data-row="${rowId}">
                        <i class="fa fa-trash"></i> Delete
                    </button>
                </td>
            </tr>
        `;
    } else {
        rowHtml = `
            <tr id="${rowId}">
                <td class="text-center font-weight-bold doc-sr-no"></td>
                <td>
                    <input type="text" name="tenderUpload.UploadDocNames" class="form-control form-control-sm doc-name-input" placeholder="Enter Document Name" value="${docName}" required />
                </td>
                <td>
                    <input type="file" name="tenderUpload.UploadDocFiles" class="form-control form-control-sm doc-file-input" accept=".zip,.rar,.pdf,.xlsx,.xls,.csv" required onchange="validateArchiveFileInput(this)" />
                    <small class="text-muted">.pdf, .xlsx, .xls, .csv, .zip, .rar (max 30 MB)</small>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger btn-remove-doc-row" data-row="${rowId}">
                        <i class="fa fa-trash"></i> Delete
                    </button>
                </td>
            </tr>
        `;
    }

    $('#tenderDocsBody').append(rowHtml);
    updateDocCountInfo();
}

let corrigendumDocRowIndex = 0;
let deletedCorrigendumDocIds = [];

function getActiveCorrigendumRowCount() {
    return $('#corrigendumDocsBody tr:not(.deleted-row)').length;
}

function updateCorrigendumDocCountInfo() {
    const count = getActiveCorrigendumRowCount();
    $('#corrigendumDocCountInfo').text(`Rows: ${count} / ${MAX_DOC_ROWS}`);
    if (count >= MAX_DOC_ROWS) {
        $('#btnAddCorrigendumDocRow').prop('disabled', true);
    } else {
        $('#btnAddCorrigendumDocRow').prop('disabled', false);
    }
    reindexCorrigendumDocRows();
}

function reindexCorrigendumDocRows() {
    let sr = 1;
    $('#corrigendumDocsBody tr:not(.deleted-row)').each(function () {
        $(this).find('.corrigendum-doc-sr-no').text(sr++);
    });
}

function addCorrigendumDocRow(docName = '', isExisting = false, existingDocId = 0, encryptedFileName = '', projectId = '', tenderNo = '') {
    if (getActiveCorrigendumRowCount() >= MAX_DOC_ROWS) {
        alert('Maximum 8 corrigendum document rows allowed.');
        return;
    }

    corrigendumDocRowIndex++;
    const rowId = `corrigendumDocRow_${corrigendumDocRowIndex}`;
    let rowHtml = '';

    if (isExisting) {
        rowHtml = `
            <tr id="${rowId}" data-existing-id="${existingDocId}">
                <td class="text-center font-weight-bold corrigendum-doc-sr-no"></td>
                <td>
                    <span>${docName || encryptedFileName}</span>
                </td>
                <td>
                    <a href="/bidder/BidderTenders/${projectId}/${tenderNo}/${encryptedFileName}" target="_blank" class="btn btn-sm btn-outline-info">
                        <i class="fa fa-download"></i> View Document
                    </a>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger btn-remove-existing-corrigendum-doc" data-id="${existingDocId}" data-row="${rowId}">
                        <i class="fa fa-trash"></i> Delete
                    </button>
                </td>
            </tr>
        `;
    } else {
        rowHtml = `
            <tr id="${rowId}">
                <td class="text-center font-weight-bold corrigendum-doc-sr-no"></td>
                <td>
                    <input type="text" name="tenderUpload.UploadCorrigendumDocNames" class="form-control form-control-sm doc-name-input" placeholder="Enter Document Name" value="${docName}" />
                </td>
                <td>
                    <input type="file" name="tenderUpload.UploadCorrigendumDocFiles" class="form-control form-control-sm doc-file-input" accept=".zip,.rar,.pdf,.xlsx,.xls,.csv" onchange="validateArchiveFileInput(this)" />
                    <small class="text-muted">.pdf, .xlsx, .xls, .csv, .zip, .rar (max 30 MB)</small>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger btn-remove-corrigendum-doc-row" data-row="${rowId}">
                        <i class="fa fa-trash"></i> Delete
                    </button>
                </td>
            </tr>
        `;
    }

    $('#corrigendumDocsBody').append(rowHtml);
    updateCorrigendumDocCountInfo();
}

function validateArchiveFileInput(input) {
    if (!input.files || input.files.length === 0) return true;
    const file = input.files[0];
    const fileName = file.name;

    // Check for multiple dots in filename
    const dotCount = (fileName.match(/\./g) || []).length;
    if (dotCount === 0) {
        alert(`File '${fileName}' is missing an extension.`);
        input.value = '';
        return false;
    }
    if (dotCount > 1) {
        alert(`File '${fileName}' has an invalid name. Multiple dots in file name are strictly prohibited for security reasons.`);
        input.value = '';
        return false;
    }

    // Check for dangerous extensions
    const ext = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
    const dangerousExtensions = ['.exe', '.dll', '.bat', '.cmd', '.sh', '.vbs', '.ps1', '.js', '.jsp', '.asp', '.aspx', '.php', '.cgi', '.msi', '.scr', '.com', '.pif', '.hta', '.jar', '.reg'];
    if (dangerousExtensions.includes(ext)) {
        alert(`File '${fileName}' has a prohibited extension '${ext}'. Executable and script files are not allowed.`);
        input.value = '';
        return false;
    }

    const allowedExtensions = ['.pdf', '.xlsx', '.xls', '.csv', '.zip', '.rar'];
    if (!allowedExtensions.includes(ext)) {
        alert(`Invalid format for '${fileName}'. Allowed formats: .pdf, .xlsx, .xls, .csv, .zip, .rar.`);
        input.value = '';
        return false;
    }

    const maxSize = 30 * 1024 * 1024;
    if (file.size > maxSize) {
        alert(`File '${fileName}' exceeds the maximum allowed size of 30 MB.`);
        input.value = '';
        return false;
    }

    return true;
}

function validateDates() {
    const startDateVal = document.getElementById("tenderUpload_TenderStartDate")?.value;
    const closingDateVal = document.getElementById("tenderUpload_TenderClosingDate")?.value;
    const openingDateVal = document.getElementById("tenderUpload_TenderOpeningDate")?.value;

    const startDate = startDateVal ? new Date(startDateVal) : null;
    const closingDate = closingDateVal ? new Date(closingDateVal) : null;
    const openingDate = openingDateVal ? new Date(openingDateVal) : null;

    if (startDate && closingDate && closingDate < startDate) {
        alert("Tender Closing Date cannot be earlier than Tender Start Date.");
        document.getElementById("tenderUpload_TenderClosingDate").value = "";
        return;
    }

    if (closingDate && openingDate && openingDate < closingDate) {
        alert("Tender Opening Date cannot be earlier than Tender Closing Date.");
        document.getElementById("tenderUpload_TenderOpeningDate").value = "";
        return;
    }

    const extendedDateInput = document.getElementById("tenderUpload_TenderCorrigendums_ExtendedDate");
    if (extendedDateInput && extendedDateInput.value) {
        const extendedDate = new Date(extendedDateInput.value);
        if (closingDate && extendedDate && extendedDate < closingDate) {
            alert("Extended Date cannot be earlier than Tender Closing Date.");
            extendedDateInput.value = "";
            return;
        }
        if (openingDate && extendedDate && openingDate < extendedDate) {
            alert("Tender Opening Date cannot be earlier than Corrigendum Extended Date.");
            document.getElementById("tenderUpload_TenderOpeningDate").value = "";
            return;
        }
    }
}

$(document).ready(function () {
    $('.js-example-basic-multiple').select2({
        placeholder: "-select-",
        allowClear: true,
        width: '100%'
    });

    if ($('#tenderDocsBody tr').length === 0) {
        addTenderDocRow();
    }

    $('#btnAddDocRow').on('click', function () {
        addTenderDocRow();
    });

    $('#btnAddCorrigendumDocRow').on('click', function () {
        addCorrigendumDocRow();
    });

    $(document).on('click', '.btn-remove-doc-row', function () {
        const rowId = $(this).data('row');
        $(`#${rowId}`).remove();
        updateDocCountInfo();
    });

    $(document).on('click', '.btn-remove-corrigendum-doc-row', function () {
        const rowId = $(this).data('row');
        $(`#${rowId}`).remove();
        updateCorrigendumDocCountInfo();
    });

    $(document).on('click', '.btn-remove-existing-doc', function () {
        const docId = $(this).data('id');
        const rowId = $(this).data('row');
        if (confirm('Are you sure you want to remove this document?')) {
            if (docId > 0) {
                deletedDocIds.push(docId);
                $('#tenderUpload_DeletedDocIds').val(deletedDocIds.join(','));
            }
            $(`#${rowId}`).remove();
            updateDocCountInfo();
        }
    });

    $(document).on('click', '.btn-remove-existing-corrigendum-doc', function () {
        const docId = $(this).data('id');
        const rowId = $(this).data('row');
        if (confirm('Are you sure you want to remove this corrigendum document?')) {
            if (docId > 0) {
                deletedCorrigendumDocIds.push(docId);
                $('#tenderUpload_DeletedCorrigendumDocIds').val(deletedCorrigendumDocIds.join(','));
            }
            $(`#${rowId}`).remove();
            updateCorrigendumDocCountInfo();
        }
    });

    $('#btnOpenSaveModal').on('click', function () {
        const form = $(this).closest('form')[0];
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const activeRows = getActiveRowCount();
        if (activeRows === 0) {
            alert('Please add at least one document row.');
            return;
        }

        // Validate all file inputs before opening modal
        let fileInputsValid = true;
        $('.doc-file-input').each(function () {
            if (this.files && this.files.length > 0) {
                if (!validateArchiveFileInput(this)) {
                    fileInputsValid = false;
                    return false;
                }
            }
        });

        if (!fileInputsValid) {
            return;
        }

        $('#confirmSaveModal').modal('show');
    });

    $('#btnConfirmSave').on('click', function () {
        $('#confirmSaveModal').modal('hide');
        const form = $('#btnOpenSaveModal').closest('form')[0];
        form.submit();
    });

    $('#corrigendumTable').on('click', '.delete-btn', function () {
        const id = $(this).data('id');
        if (confirm('Are you sure you want to delete this corrigendum?')) {
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '?handler=DeleteCorrigendum&id=' + id,
                type: 'POST',
                data: { 
                    id: id,
                    __RequestVerificationToken: token
                },
                headers: {
                    "RequestVerificationToken": token,
                    "XSRF-TOKEN": token
                },
                success: function (res) {
                    if (res && res.success) {
                        alert(res.message || 'Corrigendum deleted successfully.');
                        triggerCheckTender();
                    } else {
                        alert(res.message || 'Failed to delete.');
                    }
                },
                error: function () {
                    alert('Failed to delete.');
                }
            });
        }
    });

    $('#corrigendumTable').on('click', '.edit-btn', function () {
        const btn = $(this);
        const id = btn.data('id');
        const desc = btn.data('desc');
        const fileDoc = btn.data('file');
        const extDateRaw = btn.data('extdate');

        $("input[name='addCorrigendum'][value='yes']").prop("checked", true).trigger("change");

        $("#tenderUpload_TenderCorrigendums_Id").val(id || 0);
        $("#tenderUpload_TenderCorrigendums_CorrigendumDescription").val(desc || '');
        if (extDateRaw) {
            try {
                const d = new Date(extDateRaw);
                if (!isNaN(d.getTime())) {
                    const pad = n => String(n).padStart(2, '0');
                    const formatted = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
                    $("#tenderUpload_TenderCorrigendums_ExtendedDate").val(formatted);
                }
            } catch (e) { }
        }

        $('html, body').animate({
            scrollTop: $("#corrigendumFields").offset().top - 100
        }, 400);
    });

    $("input[name='addCorrigendum']").on("change", function () {
        const show = $(this).val() === "yes";
        $("#corrigendumFields").toggle(show);
        if (show && getActiveCorrigendumRowCount() === 0) {
            addCorrigendumDocRow();
        }
    });
});