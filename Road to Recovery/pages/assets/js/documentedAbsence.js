let documentedAbsenceTable;
let VolunteerID = 0;
const RenderToAbsenceModal = (VolunteerName, VolunteerId) => {
    $('#DocumentedAbsenceTitle').text("היעדרויות של " + VolunteerName);
    VolunteerID = VolunteerId
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetAbsenceByVolunteerId",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: VolunteerId }),
        success: function (data) {
            Absences = JSON.parse(data.d);

            if (documentedAbsenceTable != null) {
                documentedAbsenceTable.destroy();
            }


            documentedAbsenceTable = $('#DocumentedAbsenceTable').DataTable({
                /*order: [[5, "desc"]],*/
                pageLength: 5,
                rowId: 'Id',
                data: Absences,
                columnDefs: [{ "targets": [0], type: 'de_date' },{ "targets": [1], type: 'de_date' }],
                columns: [
                    {
                        data: (data) => {
                            return ConvertDBDate2UIDate(data.FromDate);
                        }
                    },
                    {
                        data: (data) => {
                            return ConvertDBDate2UIDate(data.UntilDate);
                        }
                    },


                    {
                        data: (data) => {
                            if (data.DaysToReturn == -2) {
                                return 'עתידי';
                            }
                            else {
                                return data.DaysToReturn;
                            }


                        }
                    },
                    {
                        data: "CoorName"
                    },
                    {
                        data: "Cause"
                    },
                    {
                        data: "Note"
                    },
                    {
                        data: (data) => {
                            let btnStr = `<div class='btnWrapper-right'>
                            <button id="AbsenceEdit${data.Id}" type='button' class='btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5' id='edit' title='עריכה'>
                            <i class='ti-pencil'></i>
                            </button>
                            </div>
                            <div class='btnWrapper-left'>
                            <button id="AbsenceDelete${data.Id}" type='button' class='btn btn-icon waves-effect waves-light btn-danger btn-sm m-b-5' id='remove' title='מחיקה'>
                            <i class='fa fa-remove'></i>
                            </button>
                            </div>`;

                            return btnStr;
                        }
                    },

                    

                ],


            });

        },
        error: function (err) {
            alert("Error in GetVolunteersAbsences !: " + err.responseText);
            $('#wait').hide();
        }
    });
}



const openDocumentAAbsenceModal = () => {
    $('#DocumentAAbsencesModal').modal('show');

    $('.closeBtn_DocumentAcallsModal').prop('disabled', true);
    $('#saveAbsenceBtn').prop('disabled', false);
    $('#cancellAbsenceBtn').prop('disabled', false);


    //get a list of all coordinators ()
    let allCordinatorsFromDB = [];
    let loggedInUser = {};
    loggedInUser.DisplayName = localStorage.getItem('userCell');
    loggedInUser.UserType = localStorage.getItem('userType');
    $('#DocumentAcall_writeContent').val('');
    $('#DocumentAcall_choooseContent').prop('disabled', false);
    $('#DocumentAcall_writeContent').prop('disabled', false);


    $('#DocumentAAbsence_contentErrorMsg').hide();
    $('#DocumentAAbsence_CoordinatorErrorMsg').hide();

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getCoordinatorsList_version_02",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        success: function (data) {
            allCordinatorsFromDB = JSON.parse(data.d);
            let cordinatorsOptions = '<option value="not selected">בחר.י רכז.ת</option>';

            for (var i = 0; i < allCordinatorsFromDB.length; i++) {
                cordinatorsOptions += `<option value="${allCordinatorsFromDB[i].Id}"`;
                cordinatorsOptions += loggedInUser.UserType === 'רכז' &&
                    allCordinatorsFromDB[i].DisplayName === loggedInUser.DisplayName ? 'selected>' : '>';
                cordinatorsOptions += `${allCordinatorsFromDB[i].DisplayName}</option >`;
            }
            document.getElementById('DocumentAAbsence_choooseCoordinator').innerHTML = cordinatorsOptions;
        }
    });
 
}


const documentAAbsence2DB = () => {
    //int volunteerId, int coorId, DateTime from, DateTime until, string cause, string note
    const NewAbsence = {
        volunteerId: parseInt(VolunteerID),
        coorId: parseInt($("#DocumentAAbsence_choooseCoordinator").val()),
        from: $("#DocumentAAbsenceDatePickerFrom").val(),
        until: $("#DocumentAAbsenceDatePickerUntil").val(),
        cause: $("#DocumentAAbsence_choooseContent").val(),
        note: $("#DocumentAAbsence_writeContent").val()
    }
    console.log('Gilad --- > ', NewAbsence);
    if (NewAbsence.note == '') {
        NewAbsence.note = 'אין';
        console.log('Gilad --- >אין הערה ', NewAbsence.note);
    }
   

    if (NewAbsence.cause == '-1' || NewAbsence.from == '' || NewAbsence.until == '') {
        $('#DocumentAAbsence_contentErrorMsg').show();
        $('#DocumentAAbsence_CoordinatorErrorMsg').hide();

        return;
    }



    if (new Date(NewAbsence.until) < new Date(NewAbsence.from)) {
        $('#DocumentAAbsence_CoordinatorErrorMsg').show();
        $('#DocumentAAbsence_contentErrorMsg').hide();

        return;
    }



    console.log('Gilad --- > what im send ?', NewAbsence);
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/InsertNewAbsence",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify( NewAbsence ),
        success: function (data) {
            // ↓ front-end cheating ↓

            //step 1: show the change on the grid right away

            /*
             in order to add row to this dataTable you should

             1. convert callRecordedDate to the form of → "/Date(1607810400000)/"
             2. convert callRecordedTime to the form of → time object with hours and minutes properties

               or else you will suffer from various errors
             */
            let DocumentedAbsenceTable = $('#DocumentedAbsenceTable').DataTable();

            let from = new Date(NewAbsence.from).getTime();
            let until = new Date(NewAbsence.until).getTime();
            let today = new Date().getTime();
            let daysToReturn = null;
            if (from > today) {
                daysToReturn = 'עתידי'
            }
            else {
                daysToReturn = Math.ceil((until - today) / 86400000);
            }
            from = `/Date(${from})/`;
            until = `/Date(${until})/`;
            NewAbsence.CoorName = $("#DocumentAAbsence_choooseCoordinator option:selected").text();





            const addRowData = {
                Cause: NewAbsence.cause,
                CoorName: NewAbsence.CoorName,
                DaysToReturn: daysToReturn,
                Note: NewAbsence.note,
                UntilDate: until,
                FromDate: from
            }

            DocumentedAbsenceTable.row.add(addRowData).draw(false);
            console.log(data);

        },
        error: function (err) { alert("Error in Insert New Absence Ajax: " + err.responseText); }
    });

    $('#DocumentAAbsencesModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAAbsencePlusBtn').prop('disabled', false);


    
    
    
    
}

const cancelAbsenceDocument = () => {
    $('#saveAbsenceBtn').prop('disabled', true);
    $('#cancellAbsenceBtn').prop('disabled', true);
    $('#DocumentAAbsencesModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAAbsencePlusBtn').prop('disabled', false);
}