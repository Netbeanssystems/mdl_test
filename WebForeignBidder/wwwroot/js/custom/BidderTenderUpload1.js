$('#tenderUpload_ProjectIds').on('change', function () {
    var projectIdArray = $(this).val(); // This is assumed to be an array of strings
    var projectId = Array.isArray(projectIdArray) ? projectIdArray.join(',') : projectIdArray;
    $('#tenderUpload_YardIds').html('<option value="">Loading...</option>');
    var form = $(this).closest('form');

    if (form.find('#hdProjectId').length === 0) {
        $('<input>').attr({
            type: 'hidden',
            id: 'hdProjectId',
            name: 'hdProjectId'
        }).appendTo(form);
    }
    form.find('#hdProjectId').val(projectId);
    $('#tenderUpload_YardIds').html('<option value="">Loading...</option>');
    if (projectId) {
        $.ajax({
            url: '/bidder/Admin/TenderUpload/Manage?handler=GetYardsByProject',
            type: 'GET',
            data: { projectId: projectId },
            success: function (data) {
                var selectedYardIdStr = $('#tenderUpload_YardIds').val();
                var selectedYardIdArr = selectedYardIdStr ? selectedYardIdStr.split(',').map(id => id.trim()) : [];
                $('#tenderUpload_YardIds').empty().append('<option value="">-select-</option>');

                $.each(data, function (i, yard) {
                    $('#tenderUpload_YardIds').append('<option value="' + yard.id + '">' + yard.yardNumber + '</option>');
                });

            },
            error: function () {
                $('#tenderUpload_YardIds').html('<option value="">-select-</option>');
                alert('Failed to load yards.');
            }
        });
    } else {
        $('#tenderUpload_YardIds').html('<option value="">-select-</option>');
    }
});
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
        const yardId = $('#tenderUpload_YardId').val();
        const tenderNo = $('#tenderUpload_TenderNo').val();

        if (projectId && yardId && tenderNo) {
            $.ajax({
                url: '/bidder/Admin/TenderUpload/Manage?handler=CheckTender',
                type: 'GET',
                data: {
                    projectId: projectId,
                    yardId: yardId,
                    tenderNo: tenderNo
                },
                success: function (response) {
                    debugger
                    const isNewTender = response.status === "0";
                    $(".card-header h3").html(isNewTender ? "Add Tender" : "Edit Tender");

                    const tender = isNewTender ? {} : response?.data?.value;

                    if (isNewTender || tender) {
                        $("#tenderUpload_Id").val(tender.id || '');
                        $("#tenderUpload_CreatedBy").val(tender.createdBy || '');
                        $("#tenderUpload_CreatedDate").val(tender.createdDate || '');
                        $("#tenderUpload_TenderDoc").val(tender.tenderDoc || '');


                        $("#tenderUpload_TenderClosingDate").val(tender.tenderClosingDate || '');
                        $("#tenderUpload_TenderOpeningDate").val(tender.tenderOpeningDate || '');
                        $("#tenderUpload_TenderDescription").val(tender.tenderDescription || '');
                        $("#tenderUpload_ForeignBidderId").val(tender.foreignBidderId || '');
                        if (!isNewTender && tender.tenderDoc && tender.projectId && tender.tenderNo) {
                            const existingDocLink = `
                                <span id="existdoc">
                                    <a href='/bidder/BidderTenders/${tender.projectId}/${tender.tenderNo}/${tender.tenderDoc}' target='_blank'>View Doc</a>
                                </span>`;
                            $("#tenderUpload_IFFTenderDoc").parent().append(existingDocLink);

                            $("#corrigendumOptionContainer").show();
                        } else {
                            $('input[name="IsNew"]').val(true);
                            $("#tenderUpload_IFFTenderDoc").parent().find("#existdoc").remove();
                            $("#corrigendumOptionContainer").hide();
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
    $('.js-example-basic-multiple').select2();
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

//    const formSelector = "form"; // Change this to "#tenderForm" if needed

//    function validateField($field) {
//        const value = $field.val()?.trim();
//        const isInvalid = !value || value === "-select-";

//        $field.toggleClass("is-invalid", isInvalid);
//        return !isInvalid;
//    }

//    function validateCorrigendumFields() {
//        let isValid = true;
//        $("#corrigendumFields").find("input.form-control").each(function () {
//            if (!validateField($(this))) {
//                isValid = false;
//            }
//        });
//        return isValid;
//    }

//    function validateFormFields() {
//        let isValid = true;

//        // Validate required fields
//        $(`${formSelector} input.form-control, ${formSelector} select.form-control`).each(function () {
//            if (!validateField($(this))) {
//                isValid = false;
//            }
//        });

//        // Corrigendum section
//        const addCorrigendum = $("input[name='addCorrigendum']:checked").val();
//        if (addCorrigendum === "yes") {
//            if (!validateCorrigendumFields()) {
//                isValid = false;
//            }
//        }

//        return isValid;
//    }

//    // Live validation on change/blur
//    $(formSelector).on("change blur", "input.form-control, select.form-control", function () {
//        validateField($(this));
//    });

//    // Show/hide corrigendum fields
//    $("input[name='addCorrigendum']").on("change", function () {
//        const show = $(this).val() === "yes";
//        $("#corrigendumFields").toggle(show);

//        // Re-validate corrigendum fields when toggled
//        if (show) {
//            validateCorrigendumFields();
//        }
//    });

//    // On form submit
//    $(formSelector).on("submit", function (e) {
//        if (!validateFormFields()) {
//            e.preventDefault();
//            alert("Please correct all required fields.");
//        }
//    });
});