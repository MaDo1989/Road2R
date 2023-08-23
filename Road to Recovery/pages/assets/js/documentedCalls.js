let thisVolunteerId;
let documentedCallsTable;


const preDefinedContents = [
    { key: 0, value: '&lt;בחר.י מהרשימה או הכנס.י מלל חופשי בשדה הבא&gt;' },
    { key: 1, value: 'תרשם/ירשם להסעה' },
    { key: 2, value: 'לא נרשמ.ה להסעה' },
    { key: 3, value: 'לא פנוי.ה לשיחה' },
    { key: 4, value: 'להתקשר במועד מאוחד יותר' },
    { key: 5, value: 'לא מסיע.ה בתקופה הקרובה' },
    { key: 6, value: 'נא להסיר מהמערכת' }
];

function manipulateDocumentedCallsModal(button, tableToWithdrawDataFrom) {

    $('#wait').show();

    let rowData = tableToWithdrawDataFrom.row($(button).parents('tr')).data();

    let childrenof_td = button.parentElement.children;

    
    for (let i = 0; i < childrenof_td.length; i++) {
        if (childrenof_td[i].id.includes('badgeOf_')) {
            spanBadgeId = childrenof_td[i].id;
        }
    }

    thisVolunteerId = parseInt(rowData.Id); //this variable is goobal to this page & used also in documentAcall2DB !!!

    $('#DocumentedCallsTitle').text("שיחות עם " + rowData.DisplayName)
    
    /*console.log('Gilad -->', rowData)*/
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetDocumentedCallsByDriverId",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ driverId: thisVolunteerId }),
        success: function (data) {
            data = JSON.parse(data.d)
            if (documentedCallsTable != null) {
                documentedCallsTable.destroy();
            }
            documentedCallsTable = $('#DocumentedCallsTable').DataTable({
                order: [[4, "desc"]],
                pageLength: 5,
                data: data,
                columns: [
                    {
                        data: (data) => {
                            return ConvertDBDate2UIDate(data.CallRecordedDate);
                        }
                    },
                    {
                        data: (data) => {
                            if (data.CallRecordedTime === undefined) return;
                            let time = data.CallRecordedTime;

                            let hh = time.Hours < 10 && time.Hours.toString().length === 1 ? "0" + time.Hours : time.Hours;
                            hh += ":";
                            let mm = time.Minutes < 10 && time.Minutes.toString().length === 1 ? "0" + time.Minutes : time.Minutes;
                            return hh + mm;
                        }
                    },
                    {
                        data: "CoordinatorName"
                    },
                    {
                        data: "CallContent"
                    }
                    ,
                    {
                        data: (data) => {
                            return convertDBDate2FrontEndDate(data.FullDateStemp);
                        }
                    }
                ],
                columnDefs: [
                    { "targets": [0], type: 'de_date' },
                    /*
                     Amir wanted a seperated columns to date and time but still sort by the full time stemps (or the acctual ticks)
                     so my (Yogev) soolution was to
                     1. fetch the fulltime stemp from the back-end
                     2. render and sort by this column
                     3. not showing it to the user
                      ↓*/
                    { "targets": [4], visible: false },
                    //↑

                ]

            });
            $('#wait').hide();
            // Gilad touch here
            // need to render here the AbsenceModal like the CallModal.
            $('.AbsenceBtn')[0].id = thisVolunteerId
            RenderToAbsenceModal(rowData.DisplayName, thisVolunteerId);

        },
        error: function (err) {
            alert("Error in GetVolunteersRideHistory: " + err.responseText);
            $('#wait').hide();
        }
    });


}

const openDocumentAcallModal = () => {
    $('#DocumentAcallsModal').modal('show');

    $('.closeBtn_DocumentAcallsModal').prop('disabled', true);
    $('#saveCallBtn').prop('disabled', false);
    $('#cancellCallBtn').prop('disabled', false);


    //get a list of all coordinators ()
    let allCordinatorsFromDB = [];
    let loggedInUser = {};
    loggedInUser.DisplayName = localStorage.getItem('userCell');
    loggedInUser.UserType = localStorage.getItem('userType');
    $('#DocumentAcall_writeContent').val('');
    $('#DocumentAcall_choooseContent').prop('disabled', false);
    $('#DocumentAcall_writeContent').prop('disabled', false);


    $('#DocumentAcall_contentErrorMsg').hide();
    $('#DocumentAcall_CoordinatorErrorMsg').hide();


    $('#wait').show();
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getCoordinatorsList_version_02",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        success: function (data) {
            $('#wait').hide();
            allCordinatorsFromDB = JSON.parse(data.d);
            let cordinatorsOptions = '<option value="not selected">בחר.י רכז.ת</option>';

            for (var i = 0; i < allCordinatorsFromDB.length; i++) {
                cordinatorsOptions += `<option value="${allCordinatorsFromDB[i].Id}"`;
                cordinatorsOptions += loggedInUser.UserType === 'רכז' &&
                    allCordinatorsFromDB[i].DisplayName === loggedInUser.DisplayName ? 'selected>' : '>';
                cordinatorsOptions += `${allCordinatorsFromDB[i].DisplayName}</option >`;
            }
            document.getElementById('DocumentAcall_choooseCoordinator').innerHTML = cordinatorsOptions;


            let contentOptions = `<option value="${preDefinedContents[0].key}" selected>${preDefinedContents[0].value}</option>`;
            //could be also hidden if needed ↑
            for (var i = 1; i < preDefinedContents.length; i++) {
                contentOptions += `<option value="${preDefinedContents[i].key}">${preDefinedContents[i].value}</option>`;
            }
            document.getElementById('DocumentAcall_choooseContent').innerHTML = contentOptions;



            let now = new Date();
            document.getElementById('DocumentAcallDatePicker').valueAsDate = now; //sets the default date for today

            let time = new Object();
            time.hours = now.getHours() < 10 ? '0' + now.getHours() : now.getHours();
            time.minutes = now.getMinutes() < 10 ? '0' + now.getMinutes() : now.getMinutes();

            document.getElementById('DocumentAcallTime').value = `${time.hours}:${time.minutes}`;


            $('#documentAcallPlusBtn').prop('disabled', true);
        },
        error: function (err) {
            alert("Error in GetCoordinatorsList: " + err.responseText);
            $('#wait').hide();
        }
    });
}

const cancelCallDocument = () => {
    $('#saveCallBtn').prop('disabled', true);
    $('#cancellCallBtn').prop('disabled', true);
    $('#DocumentAcallsModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAcallPlusBtn').prop('disabled', false);

}

const DocumentedAcall_writeContent_clicked = () => {
    $('#DocumentAcall_contentErrorMsg').hide();
    $('#saveCallBtn').prop('disabled', false);
}

const DocumentAcall_writeContent_blured = () => {

    let content = $('#DocumentAcall_writeContent').val();

    if (content.length > 0) {
        $('#DocumentAcall_choooseContent').prop('disabled', true)
    } else {
        $('#DocumentAcall_choooseContent').prop('disabled', false);
    }
}

const DocumentAcall_choooseCoordinator_changed = () => {

    if ($('#DocumentAcall_choooseCoordinator').val() !== "not selected") {
        $('#DocumentAcall_CoordinatorErrorMsg').hide();
        $('#saveCallBtn').prop('disabled', false);
    }
}

const DocumentAcall_choooseContent_changed = () => {

    let preDefinedContents_key = parseInt($('#DocumentAcall_choooseContent').val());

    if (preDefinedContents_key > 0) {
        $('#DocumentAcall_writeContent').prop('disabled', true)
        $('#DocumentAcall_contentErrorMsg').hide();
        $('#saveCallBtn').prop('disabled', false);
    } else {
        $('#DocumentAcall_writeContent').prop('disabled', false);

    }
}

const disableFutureIn_DocumentAcallDatePicker = () => {
    let today = new Date().toISOString().split('T')[0];
    document.getElementById('DocumentAcallDatePicker').setAttribute('max', today);
}

const documentAcall2DB = () => {
    $('#saveCallBtn').prop('disabled', true);

    let documentedCall = {};
    documentedCall.DriverId = thisVolunteerId;
    documentedCall.CoordinatorId = parseInt($('#DocumentAcall_choooseCoordinator').val());
    documentedCall.CallRecordedDate = $('#DocumentAcallDatePicker').val();
    documentedCall.CallRecordedTime = $('#DocumentAcallTime').val();

    let preDefinedContents_key = parseInt($('#DocumentAcall_choooseContent').val());
    let content2pass2DB = '';
    if (preDefinedContents_key === 0) {
        content2pass2DB = $('#DocumentAcall_writeContent').val();
    } else {

        content2pass2DB += preDefinedContents[preDefinedContents_key].value;
    }
    documentedCall.CallContent = content2pass2DB;

    if (!documentedCall.CoordinatorId) {
        $('#DocumentAcall_CoordinatorErrorMsg').show()
        $('#DocumentAcall_contentErrorMsg').hide();
        return;
    }

    if (documentedCall.CallContent.length === 0) {
        $('#DocumentAcall_contentErrorMsg').show();
        $('#DocumentAcall_CoordinatorErrorMsg').hide()
        return;
    }

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/DocumentNewCall",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ documentedCall }),
        success: function (data) {
            // ↓ front-end cheating ↓

            //step 1: show the change on the grid right away

            /*
             in order to add row to this dataTable you should

             1. convert callRecordedDate to the form of → "/Date(1607810400000)/"
             2. convert callRecordedTime to the form of → time object with hours and minutes properties

               or else you will suffer from various errors
             */
            let documentedCallsTable = $('#DocumentedCallsTable').DataTable();
            documentedCall.CoordinatorName = $("#DocumentAcall_choooseCoordinator option:selected").text();



            let timeOfCall = documentedCall.CallRecordedTime;
            let dateOfCall = documentedCall.CallRecordedDate;

            let convertable_CallRecordedFullDateStemp = new Date(`${dateOfCall}, ${timeOfCall}`);
            convertable_CallRecordedFullDateStemp = convertable_CallRecordedFullDateStemp.getTime();
            documentedCall.FullDateStemp = `/Date(${convertable_CallRecordedFullDateStemp})/`;

            let convertable_CallRecordedDate = new Date(documentedCall.CallRecordedDate);
            convertable_CallRecordedDate = convertable_CallRecordedDate.getTime();
            documentedCall.CallRecordedDate = `/Date(${convertable_CallRecordedDate})/`;

            let convertable_CallRecordedTime = {
                Hours: documentedCall.CallRecordedTime[0] + documentedCall.CallRecordedTime[1],
                Minutes: documentedCall.CallRecordedTime[3] + documentedCall.CallRecordedTime[4]
            };


            let addRowData = {
                CallRecordedDate: documentedCall.CallRecordedDate,
                CallRecordedTime: convertable_CallRecordedTime,
                CoordinatorName: documentedCall.CoordinatorName,
                CallContent: documentedCall.CallContent,
                FullDateStemp: documentedCall.FullDateStemp
            }

            documentedCallsTable.row.add(addRowData).draw(false);

            //step 2: add +1 to cals badge

            document.getElementById(spanBadgeId).innerHTML = parseInt(document.getElementById(spanBadgeId).innerHTML) + 1;

            // ↑ front-end cheating ↑

        },
        error: function (err) { alert("Error in DocumentNewCall: " + err.responseText); }
    });

    $('#DocumentAcallsModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAcallPlusBtn').prop('disabled', false);

}

