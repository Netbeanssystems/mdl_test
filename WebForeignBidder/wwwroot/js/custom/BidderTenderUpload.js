(function () {
    // local state (avoid polluting global namespace)
    var lastProjectValue = '';
    var lastProjectTime = 0;
    var inFlight = false;
    var inFlightValue = null;
    var currentRequest = null;
    var DUPLICATE_WINDOW_MS = 1000; // treat identical triggers within 1s as duplicate

    //// ensure single binding
    //$(document).off('change', '#tenderUpload_ProjectIds').on('change', '#tenderUpload_ProjectIds', function () {
    //    var $this = $(this);

    //    // Normalize selected value to comma-separated string
    //    var projectIdArray = $this.val();
    //    var normalizedValue = Array.isArray(projectIdArray) ? projectIdArray.join(',') : (projectIdArray || '');

    //    var now = Date.now();
    //    console.log('[tender] change fired. normalizedValue:', normalizedValue, 'now:', now);

    //    // quick duplicate-skip: if same value and fired very soon after previous -> skip
    //    if (normalizedValue === lastProjectValue && (now - lastProjectTime) < DUPLICATE_WINDOW_MS) {
    //        console.log('[tender] Skipping duplicate change (time guard).');
    //        return;
    //    }

    //    // if same value is already being requested, skip
    //    if (inFlight && normalizedValue === inFlightValue) {
    //        console.log('[tender] Skipping because identical request already in-flight.');
    //        // update lastProjectTime so subsequent identical fast triggers are also skipped
    //        lastProjectTime = now;
    //        return;
    //    }

    //    // update last seen
    //    lastProjectValue = normalizedValue;
    //    lastProjectTime = now;

    //    // set or update hidden input
    //    var form = $this.closest('form');
    //    if (form.find('#hdProjectId').length === 0) {
    //        $('<input>').attr({
    //            type: 'hidden',
    //            id: 'hdProjectId',
    //            name: 'hdProjectId'
    //        }).appendTo(form);
    //    }
    //    form.find('#hdProjectId').val(normalizedValue);

    //    // UI: show loading on the correct element(s)
    //    // NOTE: your original code used both '#tenderUpload_YardId' and '#tenderUpload_YardIds' —
    //    // make sure you actually want two different elements. I update both below to be safe.
    //    $('#tenderUpload_YardId').html('<option value="">Loading...</option>');
    //    $('#tenderUpload_YardIds').html('<option value="">Loading...</option>');

    //    if (!normalizedValue) {
    //        $('#tenderUpload_YardId, #tenderUpload_YardIds').html('<option value="">-select-</option>');
    //        return;
    //    }

    //    // if a previous request exists and it's for a different value, abort it (we will send new)
    //    if (currentRequest && currentRequest.readyState !== 4) {
    //        try {
    //            console.log('[tender] Aborting previous request for different value.');
    //            currentRequest.abort();
    //        } catch (e) {
    //            console.warn('[tender] abort failed', e);
    //        }
    //        currentRequest = null;
    //        inFlight = false;
    //        inFlightValue = null;
    //    }

    //    // make ajax, mark as in-flight
    //    inFlight = true;
    //    inFlightValue = normalizedValue;
    //    currentRequest = $.ajax({
    //        url: '/bidder/Admin/TenderUpload/Manage?handler=GetYardsByProject',
    //        type: 'GET',
    //        data: { projectId: normalizedValue, _: new Date().getTime() }, // cache buster
    //        cache: false,
    //        success: function (data) {
    //            console.log('[tender] Ajax success for', normalizedValue, 'received', Array.isArray(data) ? data.length : typeof data);
    //            // populate yard select(s)
    //            var selectedYardIdStr = $('#tenderUpload_YardId').val();
    //            var selectedYardIdArr = selectedYardIdStr ? selectedYardIdStr.split(',').map(id => id.trim()) : [];
    //            $('#tenderUpload_YardIds').empty().append('<option value="">-select-</option>');

    //            $.each(data, function (i, yard) {
    //                $('#tenderUpload_YardIds').append('<option value="' + yard.id + '">' + yard.yardNumber + '</option>');
    //            });
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            if (textStatus === 'abort') {
    //                console.log('[tender] Ajax aborted for', normalizedValue);
    //            } else {
    //                console.error('[tender] Ajax error', textStatus, errorThrown);
    //                $('#tenderUpload_YardId, #tenderUpload_YardIds').html('<option value="">-select-</option>');
    //                alert('Failed to load yards.');
    //            }
    //        },
    //        complete: function () {
    //            console.log('[tender] Ajax complete for', normalizedValue);
    //            inFlight = false;
    //            inFlightValue = null;
    //            currentRequest = null;
    //        }
    //    });
    //});
})();

function splitHeader(header) {
    return header
        .replace(/([a-z])([A-Z])/g, '$1 $2') // Insert space before capital letters
        .replace(/([A-Z]+)([A-Z][a-z])/g, '$1 $2') // Handle acronyms like XMLHTTPRequest → XML HTTP Request
        .replace(/\b\w/g, char => char.toUpperCase()); // Capitalize each word
}

let debounceTimer;
$('#tenderUpload_TenderNo').on('input', function () {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(function () {
        const projectId = $('#tenderUpload_ProjectIds').val();
        var projectIdArray = projectId;
        var normalizedValue = Array.isArray(projectIdArray) ? projectIdArray.join(',') : (projectIdArray || '');
        const yardId = $('#tenderUpload_YardIds').val();
        var yardIdArray = yardId;
        var yardNormalizedValue = Array.isArray(yardIdArray) ? yardIdArray.join(',') : (yardIdArray || '');
        const tenderNo = $('#tenderUpload_TenderNo').val();

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
                    debugger
                    const isNewTender = response.status === "0";
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
                        $("#tenderUpload_Id").val(tender.id || tender.Id || '');
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

                        if (!isNewTender) {
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
                            }
                        } else {
                            $('input[name="IsNew"]').val(true);
                            $("#tenderUpload_TenderStartDate").prop("readonly", false).removeClass("bg-light");
                            $("#tenderUpload_TenderClosingDate").prop("readonly", false).removeClass("bg-light");
                            $("#tenderUpload_TenderOpeningDate").prop("readonly", false).removeClass("bg-light");

                            $("#corrigendumOptionContainer").hide();
                            $('#tenderDocsBody').empty();
                            deletedDocIds = [];
                            $('#tenderUpload_DeletedDocIds').val('');
                            addTenderDocRow();
                        }

                        const corrigendums = tender.tenderCorrigendums;

                        if (!isNewTender) {
                            // Define only the columns you want to show
                            const visibleHeaders = ['corrigendumDescription', 'corrigendumDoc', 'extendedDate', 'createdDate'];

                            // Create the table header dynamically
                            const thead = $('#corrigendumTable thead');
                            let headerHtml = '<tr>';
                            visibleHeaders.forEach(header => {
                                headerHtml += `<th>${splitHeader(header)}</th>`;
                            });
                            headerHtml += '<th>Actions</th>'; // Add actions column
                            headerHtml += '</tr>';
                            thead.html(headerHtml);

                            // Create the table body
                            const tbody = $('#corrigendumTable tbody');
                            corrigendums.forEach(row => {
                                let rowHtml = '<tr>';
                                visibleHeaders.forEach(header => {
                                    let cellValue = row[header];

                                    // Format specific fields
                                    if (header.toLocaleLowerCase().includes('date')) {
                                        if (cellValue != null) cellValue = new Date(cellValue).toLocaleString();
                                    }

                                    if (header === 'corrigendumDoc') {
                                        if (cellValue) cellValue = `<a href="/uploads/${cellValue}" target="_blank">${cellValue}</a>`;
                                        else cellValue = '';
                                    }
                                    if (header === 'corrigendumDescription') {
                                        if (cellValue) {
                                        } else cellValue = '';
                                    }

                                    if (header.toLocaleLowerCase().includes('date')) {
                                        if (cellValue) {
                                        } else cellValue = '';
                                    }


                                    rowHtml += `<td>${cellValue}</td>`;
                                });

                                // Add Edit and Delete buttons
                                rowHtml += `
                                    <td>
                                        <span class="edit-btn" data-id="${row.id}"><i class="fa fa-edit text-dark"></i></span>
                                        <span class="delete-btn" data-id="${row.id}"><i class="fa fa-trash text-dark"></i></span>
                                    </td>
                                `;
                                rowHtml += '</tr>';
                                tbody.append(rowHtml);
                            });
                            $('#corrigendumTable').DataTable();
                            $('#corrigendumTable').closest('.dataTables_wrapper').parent().show();
                        } else {
                            if ($.fn.DataTable.isDataTable('#corrigendumTable')) {
                                $('#corrigendumTable').closest('.dataTables_wrapper').parent().hide();
                            }
                        }


                    } else {
                        console.error("response.data.value is not available or invalid.");
                    }

                },
                error: function (xhr) {
                    console.error('Error:', xhr.responseText);
                }
            });
        }
    }, 800);
});

$("input[name='addCorrigendum']").on("change", function () {
    if ($(this).val() === "yes") {
        $("#corrigendumFields").slideDown();
    } else {
        $("#corrigendumFields").slideUp();
    }
});

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
                    <input type="file" name="tenderUpload.UploadDocFiles" class="form-control form-control-sm doc-file-input" accept=".zip,.rar" required onchange="validateArchiveFileInput(this)" />
                    <small class="text-muted">.zip or .rar (max 30 MB)</small>
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

function validateArchiveFileInput(input) {
    if (!input.files || input.files.length === 0) return;
    const file = input.files[0];
    const fileName = file.name;
    const ext = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();

    // Check extension
    if (ext !== '.zip' && ext !== '.rar') {
        alert(`Invalid format for '${fileName}'. Only .zip and .rar formats are allowed.`);
        input.value = '';
        return false;
    }

    // Check size (30 MB = 30 * 1024 * 1024 bytes)
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
    const openingDateVal = document.getElementById("tenderUpload_TenderOpeningDate")?.value;
    const closingDateVal = document.getElementById("tenderUpload_TenderClosingDate")?.value;

    const startDate = startDateVal ? new Date(startDateVal) : null;
    const openingDate = openingDateVal ? new Date(openingDateVal) : null;
    const closingDate = closingDateVal ? new Date(closingDateVal) : null;

    if (startDate && openingDate && openingDate < startDate) {
        alert("Tender Opening Date cannot be earlier than Tender Start Date.");
        document.getElementById("tenderUpload_TenderOpeningDate").value = "";
    }

    if (openingDate && closingDate && closingDate < openingDate) {
        alert("Tender Closing Date cannot be earlier than Tender Opening Date.");
        document.getElementById("tenderUpload_TenderClosingDate").value = "";
    }

    const extendedDateInput = document.getElementById("tenderUpload_TenderCorrigendums_ExtendedDate");
    if (extendedDateInput && extendedDateInput.value) {
        const extendedDate = new Date(extendedDateInput.value);
        if (closingDate && extendedDate && extendedDate < closingDate) {
            alert("Extended Date cannot be earlier than Tender Closing Date.");
            extendedDateInput.value = "";
        }
    }
}

$(document).ready(function () {
    $('.js-example-basic-multiple').select2();

    // Initialize 1 row if empty
    if ($('#tenderDocsBody tr').length === 0) {
        addTenderDocRow();
    }

    // Dynamic row addition
    $('#btnAddDocRow').on('click', function () {
        addTenderDocRow();
    });

    // Remove newly added row
    $(document).on('click', '.btn-remove-doc-row', function () {
        const rowId = $(this).data('row');
        $(`#${rowId}`).remove();
        updateDocCountInfo();
    });

    // Remove existing document
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

    // Confirmation modal logic
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

        $('#confirmSaveModal').modal('show');
    });

    $('#btnConfirmSave').on('click', function () {
        $('#confirmSaveModal').modal('hide');
        const form = $('#btnOpenSaveModal').closest('form')[0];
        form.submit();
    });
    // Delete Action
    $('#corrigendumTable').on('click', '.delete-btn', function () {
        debugger
        const id = $(this).data('id');
        if (confirm('Are you sure you want to delete this corrigendum?')) {
            $.ajax({
                url: `/api/corrigendums/${id}`,
                type: 'DELETE',
                success: function () {
                    alert('Deleted successfully!');
                    // Reload the table or remove the row dynamically
                    $('#corrigendumTable').DataTable().ajax.reload();
                },
                error: function () {
                    alert('Failed to delete.');
                }
            });
        }
    });

    // Edit Action
    $('#corrigendumTable').on('click', '.edit-btn', function () {
        const table = $('#corrigendumTable').DataTable();
        const tr = $(this).closest('tr');
        const row = table.row(tr.hasClass('child') ? tr.prev() : tr);
        const rowData = row.data();


        console.log('Edit Row Data:', rowData);
        debugger
        $("input[name='addCorrigendum']").val("yes").trigger("change");
        //$("#corrigendumFields").slideDown();

        const extendedDate = new Date(rowData[2]);
        const createdDate = new Date(rowData[3]);
        $("#tenderUpload_TenderCorrigendums_Id").val($(rowData[4]).data("id") || '');
        $("#tenderUpload_TenderCorrigendums_CreatedBy").val(rowData[0] || '');
        $("#tenderUpload_TenderCorrigendums_CreatedDate").val(createdDate.toISOString().slice(0, 16) || '');

        $("#tenderUpload_TenderCorrigendums_CorrigendumDescription").val(rowData[0] || '');
        $("#tenderUpload_TenderCorrigendums_CorrigendumDoc").val($(rowData[1]).text() || '');
        const projectId = $("#tenderUpload_ProjectIds").val();
        const tenderNo = $("#tenderUpload_TenderNo").val();
        const existingDocLink = `
                                <span id="existdoc">
                                    <a href='/bidder/BidderTenders/${projectId}/${tenderNo}/${$(rowData[1]).text()}' target='_blank'>View Doc</a>
                                </span>`;
        $("#tenderUpload_TenderCorrigendums_IFFCorrigendumDoc").parent().append(existingDocLink);
        $("#tenderUpload_TenderCorrigendums_ExtendedDate").val(extendedDate.toISOString().slice(0, 16) || '');
    });

    const formSelector = "form"; // Change this to "#tenderForm" if needed

    //function validateField($field) {
    //    const value = $field.val()?.trim();
    //    const isInvalid = !value || value === "-select-";

    //    $field.toggleClass("is-invalid", isInvalid);
    //    return !isInvalid;
    //}

    //function validateCorrigendumFields() {
    //    let isValid = true;
    //    $("#corrigendumFields").find("input.form-control").each(function () {
    //        if (!validateField($(this))) {
    //            isValid = false;
    //        }
    //    });
    //    return isValid;
    //}

    //function validateFormFields() {
    //    let isValid = true;

    //    // Validate required fields
    //    $(`${formSelector} input.form-control, ${formSelector} select.form-control`).each(function () {
    //        if (!validateField($(this))) {
    //            isValid = false;
    //        }
    //    });

    //    // Corrigendum section
    //    const addCorrigendum = $("input[name='addCorrigendum']:checked").val();
    //    if (addCorrigendum === "yes") {
    //        if (!validateCorrigendumFields()) {
    //            isValid = false;
    //        }
    //    }

    //    return isValid;
    //}

    // Live validation on change/blur
    $(formSelector).on("change blur", "input.form-control, select.form-control", function () {
        //validateField($(this));
    });

    // Show/hide corrigendum fields
    $("input[name='addCorrigendum']").on("change", function () {
        const show = $(this).val() === "yes";
        $("#corrigendumFields").toggle(show);

        // Re-validate corrigendum fields when toggled
        if (show) {
           // validateCorrigendumFields();
        }
    });

    // On form submit
    //$(formSelector).on("submit", function (e) {
    //    if (!validateFormFields()) {
    //        e.preventDefault();
    //        alert("Please correct all required fields.");
    //    }
    //});
});