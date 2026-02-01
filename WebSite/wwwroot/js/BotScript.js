var Name;
var height = 0;
$(document).ready(function () {
    $.get('https://www.cloudflare.com/cdn-cgi/trace', function (data) {
        data = data.trim().split('\n').reduce(function (obj, pair) {
            pair = pair.split('=');
            return obj[pair[0]] = pair[1], obj;
        }, {});
        $("#IPAddress").val(data.ip);
    });
    $("#lblsession").hide();
    $(".bot-title-close-button").click(function () {
        $("#bot-launch-button").show();
    });
});
function Show(show = "") {
    $("#bot-launch-button").hide();
    $("#bot-input-box").val('');
    document.getElementById("bot-input-box").disabled = true;
    document.getElementById('bot-input-box').placeholder = 'Please click on any of the above options';
    var x = document.getElementById("bot-container");

    if (show == "2") {
        x.style.display = "block";
        showSecondMenu($("#IDname").val(), '');
        $("#FAQ").show();
        $("#General---Application").show();
        $("#Financial").show();
        $("#Technical-and-other-Document").show();
        $("#Legal").show();
        $("#Homebtn").show();
        $("#menuname").val("Child");
        return;
    }

    if (x.style.display === "block" && show != "1") {
        x.style.display = "none";
        $("#maindiv label").remove();
        $("#maindiv img").remove();
        $("#maindiv br").remove();
        $("#maindiv a").remove();
        $('#maindiv li').remove();
        $('#maindiv').append('<li id="liDemo" class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');

        $("#divBillinfor label").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();

        $('#divBillinfor li').remove();
        $('#divBillinfor').append('<li id="cmenu" class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');
    }
    else {
        x.style.display = "block";
        $("#maindiv label").remove();
        $("#maindiv img").remove();
        $("#maindiv br").remove();
        $("#maindiv a").remove();
        $('#maindiv li').remove();
        $('#maindiv').append('<li id="liDemo" class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');

        // For Bill Info Section by Roshan
        $("#divBillinfor label").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#divBillinfor li').remove();
        $('#divBillinfor').append('<li id="cmenu" class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');

        // For Response Section by Roshan
        $("#divResponse label").remove();
        $("#divResponse img").remove();
        $("#divResponse br").remove();
        $("#divResponse a").remove();
        $('#divResponse li').remove();
        $('#divResponse').append('<li id="cmenu" class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');

        // For Account Info Section by Roshan
        $("#divaccountno label").remove();
        $("#divaccountno img").remove();
        $("#divaccountno br").remove();
        $("#divaccountno a").remove();
        $('#divaccountno li').remove();
        $('#divaccountno').append('');

        // For Account Info Section by Roshan
        $("#divnewconnection label").remove();
        $("#divnewconnection img").remove();
        $("#divnewconnection br").remove();
        $("#divnewconnection a").remove();
        $('#divnewconnection li').remove();
        $('#divnewconnection').append('<li class="message user appeared" style="display: none;">' +
            '    <div class="message-avatar"></div>' +
            '    <div class="message-wrapper">' +
            '        <div class="message-text"></div>' +
            '    </div>' +
            '</li>');
    }

    var div = document.getElementById("maindiv");
    div.style.display = "block";
    var div = document.getElementById("divBillinfor");
    div.style.display = "none";

    var div1 = document.getElementById("divnewconnection");
    div1.style.display = "none";
    $("#bot-input-box").val('Wait...');
    $.ajax({
        url: 'Index?handler=ChatBotData',
        success: function (res) {
            $("#bot-input-box").val('');
            var jsonobj = JSON.parse(res);
            $('#maindiv').find('li').not(':first').remove();
            // Select the 'message-text' div within the 'liDemo' element
            $('#liDemo').css('display', 'none');
            $('#liDemo .message-text').css('text-align', 'left');
            for (var ind = 0; ind < jsonobj.length; ind++) {
                var cln = $('#maindiv').find('#liDemo').clone();
                $('div.message-text', cln).html(jsonobj[ind].P_MenuName);
                $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'OnClickMethod("showSecondMenu",' + jsonobj[ind].Id + ',"","","click button [' + jsonobj[ind].P_MenuName + ']","","https://www.google.com/","","Please click on the following link","' + jsonobj[ind].P_MenuName.replace(/\s+/g, '-') + '") ');
                ID = $("#IDname").val();
                $('#TitelText').text('Our Smart Bot!');
                $("#menuname").val("perant");
                $(cln).removeAttr('id');
                $('#maindiv').append(cln.show());
            }
        }
    });
}
function showbillinfo(Id, DivId) {
    $("#divBillinfor label").remove();
    $("#divBillinfor br").remove();
    $("#divBillinfor a").remove();
    document.getElementById("bot-input-box").disabled = true;
    $("#IDname").val(Id);
    if ($("#menuname").val() == "SubChild") {
        var div = document.getElementById("maindiv");
        var div1 = document.getElementById("divBillinfor");
        div.style.display = "none";
        div1.style.display = "block";
    } else {
        var div = document.getElementById("maindiv");
        if (div.style.display !== "none") {
            div.style.display = "none";
        } else {
            div.style.display = "block";
        }
        var div1 = document.getElementById("divBillinfor");
        if (div1.style.display !== "none") {
            div1.style.display = "none";
        } else {
            div1.style.display = "block";
        }
    }
    showSecondMenu(Id, DivId)
}
function showSecondMenu(Id, DivId) {
    $.ajax({
        url: 'Index?handler=ChatBotChildData',
        data: {
            id: Id
        },
        success: function (res) {

            document.getElementById("maindiv").style.display = "none";
            document.getElementById("maindiv").innerHTML = "";

            $("#bot-input-box").val('');
            var jsonobj = JSON.parse(res);
            if (jsonobj.length != 0) {
                $('#divBillinfor').find('li').not(':first').remove();
                //$('#cmenu').hide();
                for (var ind = 0; ind < jsonobj.length; ind++) {
                    var cln = $('#divBillinfor').find('#cmenu').clone();
                    $('div.message-text', cln).html(jsonobj[ind].ChildMenu);
                    $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'OnClickMethod("ChatBotQuestionsData","' + jsonobj[ind].ChildMenuId + '","' + jsonobj[ind].parantMenuId + '","","click button [' + jsonobj[ind].ChildMenu + ']","","https://www.google.com/","","Please click on the following link","' + jsonobj[ind].ChildMenu.replace(/\s+/g, '-').replace(/&/g, '-') + '") ');
                    $("#menuname").val("Child");
                    $(cln).attr('id', jsonobj[ind].ChildMenu.replace(/\s+/g, '-').replace(/&/g, '-'));
                    $('#divBillinfor').append(cln.show());
                    document.getElementById("bot-input-box").disabled = true;
                    $("#bot-input-box").attr("placeholder", "Please click on any of the above options");
                }
            }

            // If #Homebtn does not exist in the DOM
            if ($("#Homebtn").length === 0) {
                // Clone the existing element
                var cln = $('#divBillinfor').find('#cmenu').clone();

                // Update the cloned element
                $('div.message-text', cln).html("Home");
                $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'Show(1)');

                // Set the value for an input field
                $("#menuname").val("Child");

                // Assign a unique ID to the cloned element
                $(cln).attr('id', 'Homebtn');

                // Append the cloned element to #divBillinfor and make it visible
                $('#divBillinfor').append(cln.show());
                $('#divBillinfor').show();
            }
        }
    });
}

function ChatBotQuestionsData(Id, DivId) {
    $.ajax({
        url: 'Index?handler=ChatBotQuestionsData',
        data: {
            Id: Id
        },
        success: function (res) {
            document.getElementById("maindiv").style.display = "none";
            document.getElementById("maindiv").innerHTML = "";

            var jsonobj = JSON.parse(res);
            if (jsonobj.length != 0) {
                //$('#divBillinfor').find('li').not(':first').remove();

                // Create dropdown menu
                var dropdown = $('<select class="message user"></select>').attr('id', 'jsonDropdown');
                $('#divBillinfor').append($('<li class="message user"></li>').append(dropdown));

                var select = dropdown;

                for (var ind = 0; ind < jsonobj.length; ind++) {
                    if (jsonobj[ind].ChildMenu != null) {
                        select.append($('<option></option>').attr('value', jsonobj[ind].ChildMenu).text(jsonobj[ind].ChildMenu));
                    } else if (jsonobj[ind].Questions != null) {
                        select.append($('<option></option>').attr('value', jsonobj[ind].Questions).text(jsonobj[ind].Questions));
                    } else {
                        select.append($('<option></option>').attr('value', jsonobj[ind].Answers).text(jsonobj[ind].Answers));
                    }
                }

                // Add event listener to dropdown
                select.change(function () {
                    var selectedValue = $(this).val();
                    var selectedQuestion = jsonobj.find(item => item.ChildMenu === selectedValue || item.Questions === selectedValue || item.Answers === selectedValue);
                    if (selectedQuestion) {
                        OnClickMethod("showAnswer", selectedQuestion.Q_Id, selectedQuestion.P_Id, "", "click button [" + selectedQuestion.Questions + "]", "", "https://www.google.com/", "", "Please click on the following link", selectedQuestion.Questions.replace(/[^\w\s]/g, '').replace(/\s+/g, '-').replace(/&/g, '-'));
                    }
                });
                $("#Homebtn").remove();
            }

            $("#P_Id").val(Id);

            if ($("#Homebtn").length === 0) {
                var cln = $('#divBillinfor').find('#cmenu').clone();
                $('div.message-text', cln).html("Home");
                $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'Show(1)');
                $("#menuname").val("SubChild");
                $(cln).attr('id', 'Homebtn');
                $('#divBillinfor').append(cln.show());
            }
            $("#FAQ").hide();
            $("#General---Application").hide();
            $("#Financial").hide();
            $("#Technical-and-other-Document").hide();
            $("#Legal").hide();
            $("#Homebtn").show();
        }

    });
}
function showThirdMenu(Id, P_Id, DivId) {
    $("#divBillinfor label").remove();
    $("#divBillinfor br").remove();
    $("#divBillinfor a").remove();
    document.getElementById("bot-input-box").disabled = true;
    $("#bot-input-box").val('Wait...');
    $("#divBillinfor li").each(function () {
        $(this).show(); // show each list item
    });
    $("#" + DivId).hide();
    if (DivId == "Profile") {
        $('#cmenu .message-text').html(`
            <label>For over five decades, we have been delivering world-class projects for our clients across the globe.</label><br>
            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    } else if (DivId == "Leadership") {
        $('#cmenu .message-text').html(`
            <label>Meet our Board of Directors, who provide strategic direction for corporate governance and value creation.</label><br>
            <label><a href="https://engineersindia.com/Leadership">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Petroleum-Refining") {
        $('#cmenu .message-text').html(`
            <label>EIL is the leading engineering consultancy service provider to the Refinery sector in India, having executed a combined refining capacity of over 150 MMTPA.</label><br>
            <label><a href="https://engineersindia.com/petroleum-refining">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Pipelines") {
        $('#cmenu .message-text').html(`
            <label>EIL has an impressive record of implementing pipeline projects in varied terrains, having executed over 50 Pipeline Projects.</label><br>
            <label><a href="https://engineersindia.com/pipeline">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Strategic-Storages") {
        $('#cmenu .message-text').html(`
            <label>EIL has executed 5.33 MMT underground crude oil storage projects as part of Govt. of India's strategic crude oil storage programme.</label><br>
            <label><a href="https://engineersindia.com/strategic-storages">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Petrochemicals") {
        $('#cmenu .message-text').html(`
            <label>EIL has built an unmatched track record of providing services to major petrochemical projects and is credited with engineering 11 of the 12 mega petrochemical complexes in India.</label><br>
            <label><a href="https://engineersindia.com/petrochemicals">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Fertilizers") {
        $('#cmenu .message-text').html(`
            <label>EIL provides the complete array of services for fertilizer projects having worked with all renowned licensors/contractors of ammonia and urea technologies.</label><br>
            <label><a href="https://engineersindia.com/fertilizers">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Mining---Metallurgy") {
        $('#cmenu .message-text').html(`
            <label>EIL has implemented a multitude of mining and metallurgical projects involving large non-ferrous metallurgical plants for alumina, aluminium, copper, zinc, lead titanium, magnesium etc.</label><br>
            <label><a href="https://engineersindia.com/mining-&-metallurgy">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Infrastructure") {
        $('#cmenu .message-text').html(`
            <label>EIL provides the complete range of services for infrastructure development projects involving urban development, Airports, Data Centres, Sports Complex and Institutional Buildings.</label><br>
            <label><a href="https://engineersindia.com/infrastructure">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Bio-Fuels") {
        $('#cmenu .message-text').html(`
            <label>EIL is geared up to harness the power of Bio fuels to catalyse India's journey towards a cleaner & greener tomorrow.</label><br>
            <label><a href="https://engineersindia.com/Bio-Fuels">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Financial-Results") {
        $('#cmenu .message-text').html(`
            <label>A snapshot of Quarterly Financial Results of the Company</label><br>
            <label><a href="https://engineersindia.com/Investor/Reports/FinancialReports">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Annual-Reports") {
        $('#cmenu .message-text').html(`
            <label>Annual Reports detailing operational & financial performance of the Company</label><br>
            <label><a href="https://engineersindia.com/Investor/Reports/AnnualReports">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Policies-&-Codes") {
        $('#cmenu .message-text').html(`
            <label>A glossary of Policies & Codes for Business Conduct</label><br>
            <label><a href="https://engineersindia.com/Investor/Internal/Policies-and-Codes">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Disclosures") {
        $('#cmenu .message-text').html(`
            <label>Disclosures to Stock Exchanges under Regulation 30 of SEBI LODR</label><br>
            <label><a href="https://engineersindia.com/Investor/Internal/Regl-30">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Why-Work-at-EIL") {
        $('#cmenu .message-text').html(`
            <label>An enabling environment ensures professional development and career growth of our people.</label><br>
            <label><a href="https://engineersindia.com/Why-Work-at-EIL">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Applying-to-EIL") {
        $('#cmenu .message-text').html(`
            <label>Selection process and eligibility criteria for various positions.</label><br>
            <label><a href="https://engineersindia.com/Applying-to-EIL">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    else if (DivId == "Opportunities") {
        $('#cmenu .message-text').html(`
            <label>Details application, FAQs etc for advertised vacancies.</label><br>
            <label><a href="https://recruitment.eil.co.in/">Know More</a></label>
        `);
        $('#cmenu').css('display', '');
        $('#cmenu .message-text').css('text-align', 'left');
    }
    let homebtn = $("#Homebtn")[0];
    let secondDiv = $(homebtn).find("div").eq(1); // `eq(1)` refers to the second `<div>`

    // Check if the `onclick` property is defined
    let hasOnClick = secondDiv.attr('onclick') == 'Show(1)' ? true : false;
    if (hasOnClick) {
        $("#Homebtn")[0].remove;
    } else {
    }
    // If #Homebtn does not exist in the DOM
    if ($("#Homebtn").length === 0 && hasOnClick) {
        // Clone the existing element
        var cln = $('#divBillinfor').find('#cmenu').clone();

        // Update the cloned element
        $('div.message-text', cln).html("Home");
        $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'showSecondMenu(' + P_Id + ')');

        // Set the value for an input field
        $("#menuname").val("Child");

        // Assign a unique ID to the cloned element
        $(cln).attr('id', 'Homebtn');

        // Append the cloned element to #divBillinfor and make it visible
        $('#divBillinfor').append(cln.show());
    }
}
function Back() {
    var back = $("#menuname").val();
    var Id = $("#IDname").val();
    if (back == "parent") {
        $("#divBillinfor label").remove();
        $("#divBillinfor select").remove();
        $("#divBillinfor .container").empty();
        $("#divBillinfor .row").html('');
        $("#divBillinfor .container").remove();
        $(".MessageAdded1").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#maindiv select').remove();
        $('#districts1').val("");
        $("#discription").val("");
        $("#url").val("");
        $("#ACC").val("");

        $("#divResponse label").remove();
        $("#divResponse").hide();
        Show("1");
    }
    if (back == "Child") {
        $("#divBillinfor label").remove();
        $("#divBillinfor .container").empty();
        $("#divBillinfor .row").html('');
        $("#divBillinfor .container").remove();
        $(".MessageAdded1").remove();
        $("#divBillinfor select").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#maindiv select').remove();
        $('#districts1').val("");
        $("#discription").val("");
        $("#url").val("");
        $("#ACC").val("");

        $("#divResponse label").remove();
        $("#divResponse").hide();
        Show('1');
    }
    else if (back == "SubChild") {
        $("#cmenu").hide();
        $("#divBillinfor label").remove();
        $("#divBillinfor .container").empty();
        $("#divBillinfor .row").html('');
        $("#divBillinfor .container").remove();
        $(".MessageAdded1").remove();
        $("#divBillinfor select").remove();
        $("#divBillinfor > li.message.user").hide();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#maindiv select').remove();
        $('#districts1').val("");
        $("#discription").val("");
        $("#url").val("");
        $("#ACC").val("");
        $("#divResponse").hide();
        $("#divResponse label").remove();
        $("#divBillinfor > ul.ul-box").hide();
        //$("#bot-message-grid > ul > li .message user").remove();
        //$("#Homebtn").remove();
        Show("2");
    }
}
function myFunction() {
    var x = document.getElementById("bot-container");
    if (x.style.display === "none") {
        x.style.display = "block";
        $("#divBillinfor label").remove();
        $("#divBillinfor select").remove();
        $("#divBillinfor .container").empty();
        $("#divBillinfor .row").html('');
        $("#divBillinfor .container").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#divBillinfor districts').remove();
        $('#divBillinfor Divisions').remove();
        $('#divBillinfor Subdivisions').remove();
        $('#maindiv districts').remove();
        $('#maindiv Divisions').remove();
        $('#maindiv Subdivisions').remove();
        $('#districts1').val("");

        $("#discription").val("")
        $("#url").val("");
        $("#ACC").val("");
    }
    else {
        x.style.display = "none";
        $("#divBillinfor label").remove();
        $("#divBillinfor select").remove();
        $("#divBillinfor .container").empty();
        $("#divBillinfor .row").html('');
        $("#divBillinfor .container").remove();
        $("#divBillinfor img").remove();
        $("#divBillinfor br").remove();
        $("#divBillinfor a").remove();
        $('#divBillinfor districts').remove();
        $('#divBillinfor Divisions').remove();
        $('#divBillinfor Subdivisions').remove();
        $('#maindiv districts').remove();
        $('#maindiv Divisions').remove();
        $('#maindiv Subdivisions').remove();
        $('#districts1').val("");
        $("#discription").val("")
        $("#url").val("");
        $("#ACC").val("");
    }
}
function OnClickMethod(ClickFunctionName = "", ID = "", P_ID = "", ToMessage = "", FromMessage = "", OpenUrl_PageName = "", OpenUrl_Url = "", addAccountno_Remove = "", addAccountno_name = "", DivId = "") {
    $("#divaccountno").hide();

    if (ID == "" || ID == undefined || ID == null) {
        ID = $("#IDname").val();
    }
    if (ClickFunctionName == "showbillinfo") {
        showbillinfo(ID, DivId);
    }
    else if (ClickFunctionName == "OpenUrl") {
        OpenUrl(OpenUrl_PageName, OpenUrl_Url, addAccountno_name, FromMessage);
    }
    else if (ClickFunctionName == "showThirdMenu") {
        $("#url").val(OpenUrl_Url);
        showThirdMenu(ID, P_ID, DivId);
    }
    else if (ClickFunctionName == "showAnswer") {
        $("#url").val(OpenUrl_Url);
        showAnswer(ID, P_ID, DivId);
    }
    else if (ClickFunctionName == "ChatBotQuestionsData") {
        ChatBotQuestionsData(ID, DivId);
    }
    else if (ClickFunctionName == "showSecondMenu") {
        $("#url").val(OpenUrl_Url);
        showSecondMenu(ID, DivId);
    }
}

function showAnswer(ID, P_ID, DivId) {
    //console.log("ID " + ID + ", P ID " + P_ID + ", Div Id " + DivId);
    $("#divBillinfor label").remove();
    $("#divBillinfor br").remove();
    $("#divBillinfor a").remove();
    document.getElementById("bot-input-box").disabled = true;
    $("#bot-input-box").val('Wait...');
    //$("#divBillinfor li").each(function () {
    //    $(this).show(); // show each list item
    //});
    //$("#" + DivId).hide();
    //if (DivId == "Which-department-is-dealing-with-Exports-in-MDL") {
    //    $('#cmenu .message-text').html(`
    //            <label>For over five decades, we have been delivering world-class projects for our clients across the globe.</label><br>
    //            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
    //        `);
    //    $('#cmenu').css('display', '');
    //    $('#cmenu .message-text').css('text-align', 'left');
    //}
    //else if (DivId == "What-are-the-contact-details-of-section-dealing-with-Exports-in-MDL") {
    //    $('#cmenu .message-text').html(`
    //            <label>Meet our Board of Directors, who provide strategic direction for corporate governance and value creation.</label><br>
    //            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
    //        `);
    //    $('#cmenu').css('display', '');
    //    $('#cmenu .message-text').css('text-align', 'left');
    //}
    //else if (DivId == "List-of-products-offered-for-Exports-in-MDL") {
    //    $('#cmenu .message-text').html(`
    //            <label>EIL is the leading engineering consultancy service provider to the Refinery sector in India, having executed a combined refining capacity of over 150 MMTPA.</label><br>
    //            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
    //        `);
    //    $('#cmenu').css('display', '');
    //    $('#cmenu .message-text').css('text-align', 'left');
    //}
    //else if (DivId == "List-of-Services-Offered-for-Export-by-MDL") {
    //    $('#cmenu .message-text').html(`
    //            <label>For over five decades, we have been delivering world-class projects for our clients across the globe.</label><br>
    //            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
    //        `);
    //    $('#cmenu').css('display', '');
    //    $('#cmenu .message-text').css('text-align', 'left');
    //}
    //else if (DivId == "How-many-Marine-Vessels-Platforms-are-exported-by-MDL-till-date") {
    //    $('#cmenu .message-text').html(`
    //            <label>EIL has an impressive record of implementing pipeline projects in varied terrains, having executed over 50 Pipeline Projects.</label><br>
    //            <label><a href="https://engineersindia.com/Profile">Know More</a></label>
    //        `);
    //    $('#cmenu').css('display', '');
    //    $('#cmenu .message-text').css('text-align', 'left');
    //}

    $.ajax({
        url: 'Index?handler=ChatBotAnswersData',
        data: {
            Id: ID
        },
        success: function (res) {
            document.getElementById("maindiv").style.display = "none";
            document.getElementById("maindiv").innerHTML = "";
            var jsonobj = JSON.parse(res);
            if (jsonobj.length != 0) {
                $(".ul-box").remove();
                $(".Answers").remove();
                $('#divBillinfor').append("<ul class='ul-box'></ul>");
                for (var ind = 0; ind < jsonobj.length; ind++) {
                    var cln = $('#divBillinfor').find('#cmenu').clone();
                    if (jsonobj[ind].ChildMenu != null) {
                        $('div.message-text', cln).html(jsonobj[ind].ChildMenu);
                        $(cln).attr('id', jsonobj[ind].ChildMenu.replace(/\s+/g, '-').replace(/&/g, '-'));
                    }
                    else if (jsonobj.length == 0) { }
                    else if (jsonobj[ind].Questions != null) {
                        $('div.message-text', cln).html(jsonobj[ind].Questions);
                        $(cln).attr('id', jsonobj[ind].Questions.replace(/\s+/g, '-').replace(/&/g, '-'));
                        $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'OnClickMethod("showAnswer",' + jsonobj[ind].A_Id + ',' + jsonobj[ind].P_Id + ',"","click button [' + jsonobj[ind].Questions + ']","","https://www.google.com/","","Please click on the following link","' + jsonobj[ind].Questions.replace(/\s+/g, '-') + '") ');
                    }
                    else {
                        $('div.message-text', cln).html(jsonobj[ind].Answers);
                        $(cln).attr('id', "Ans" + jsonobj[ind].A_Id + "Ques" + jsonobj[ind].Q_Id);
                        $(cln).addClass("Answers");
                    }                    
                    $("#Homebtn").remove();
                    //$('#divBillinfor .message-text').each().remove();
                    //$('#divBillinfor .message user appeared').remove();
                    //$(".message.user.appeared").hide();
                    //$(".message.user.appeared").css({
                    //    'display':'none'
                    //});
                    $('.ul-box').append(cln.show());
                    //$(".message.user.appeared").slice(1).show();
                    $("#Homebtn").show();
                    document.getElementById("bot-input-box").disabled = true;
                    $("#bot-input-box").attr("placeholder", "Please click on any of the above options");
                }
                $("#menuname").val("SubChild");
                //$('#divBillinfor').append("<ul class='ul-box'></ul>");
            }


            // If #Homebtn does not exist in the DOM
            if ($("#Homebtn").length === 0) {
                // Clone the existing element
                var cln = $('#divBillinfor').find('#cmenu').clone();

                // Update the cloned element
                $('div.message-text', cln).html("Home");
                $('div.message-text', cln).parent('div.message-wrapper').attr('onclick', 'Show(1)');

                // Set the value for an input field
                $("#menuname").val("SubChild");

                // Assign a unique ID to the cloned element
                $(cln).attr('id', 'Homebtn');

                // Append the cloned element to #divBillinfor and make it visible
                $('#divBillinfor').append(cln.show());
            }
        }
    });
}
function openurl(Url) {
    if (Url != "") {
        var win = window.open(Url, '_blank');
        if (win) {
            win.focus();
        }
    }
    else {
        var win = window.open($('#lblUrls').attr('values'), '_blank');
        if (win) {
            win.focus();
        }
    }
}
function RedirectOnLink() {
}
function showbillinfoBack(Id) {
    $("#divBillinfor label").remove();
    $("#divBillinfor br").remove();
    $("#divBillinfor a").remove();
    document.getElementById("bot-input-box").disabled = true;
    $("#IDname").val(Id);
    if ($("#menuname").val() == "SubChild") {
        var div = document.getElementById("maindiv");
        var div1 = document.getElementById("divBillinfor");
        div.style.display = "none";
        div1.style.display = "block";
    } else {
        var div = document.getElementById("maindiv");
        if (div.style.display !== "none") {
            div.style.display = "none";
        } else {
            div.style.display = "block";
        }
        var div1 = document.getElementById("divBillinfor");
        if (div1.style.display !== "none") {
            div1.style.display = "none";
        } else {
            div1.style.display = "block";
        }
    }
    showSecondMenu(Id);
}