
let documentedAbsenceTable;
let VolunteerID = 0;
let ThisDisplayName = ``;
let ThisAbsencesList = [];
let isEdit = false;
let ThisAbsenceId = -1;
let thisRowClicked = -1;


// this function render the dataTable stracture and render the data.(used also as a data refresh)
const RenderToAbsenceModal = (VolunteerName, VolunteerId) => {
    $('#wait').show();
    $('#DocumentedAbsenceTitle').text("היעדרויות של " + VolunteerName);
    VolunteerID = VolunteerId
    ThisDisplayName = VolunteerName;
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetAbsenceByVolunteerId",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: VolunteerId }),
        success: function (data) {
            Absences = JSON.parse(data.d);
            ThisAbsencesList = Absences;
            ColorCallBtn_afterChange(ThisAbsenceId, ThisAbsencesList, VolunteerID)


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
                            <button id="AbsenceEdit${data.Id}" type='button' onclick="Delete_O_Edit_AbsenceRow(this)" data-absence='${JSON.stringify( data ) }' class='btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5' id='edit' title='עריכה'>
                            <i class='ti-pencil'></i>
                            </button>
                            </div>
                            <div class='btnWrapper-left'>
                            <button id="AbsenceDelete${data.Id}" type='button' onclick="Delete_O_Edit_AbsenceRow(this)" data-absence='${JSON.stringify(data) }' class='btn btn-icon waves-effect waves-light btn-danger btn-sm m-b-5' id='remove' title='מחיקה'>
                            <i class='fa fa-remove'></i>
                            </button>
                            </div>`;

                            return btnStr;
                        }
                    },

                    

                ],


            });
            $('#wait').hide();
        },
        error: function (err) {
            alert("Error in GetVolunteersAbsences !: " + err.responseText);
            $('#wait').hide();
        }
    });
}

const Delete_O_Edit_AbsenceRow = (btn) => {
    const AbsenceData = JSON.parse(btn.getAttribute('data-absence')); 
    const ThisRow = btn.parentNode.parentNode.parentNode;
    let absenceID = ThisRow.id != '' ? parseInt(ThisRow.id) : AbsenceData.Id;
    ThisAbsenceId = absenceID
    //Delete This Row
    if (btn.id.includes('AbsenceDelete')) {
        const DataTable = $('#DocumentedAbsenceTable').DataTable();


        //need to get the identity scop from database to prevent this error.
       // gilad fixed that there is no reason to this "if" just for safety.
        if (absenceID==undefined) {
            alert('לא ניתן למחוק את השורה יש לצאת מהחלון ולחזור כדי למחוק.');
            return;
        }
        swal({
            title: "האם אתם בטוחים?",
            type: "warning",
            text: "היעדרות זו תימחק",
            showCancelButton: true,
            cancelButtonText: "בטל",
            confirmButtonClass: 'btn-warning',
            confirmButtonText: "מחיקה",
            closeOnConfirm: true
        }, function () {
            $.ajax({
                dataType: "json",
                url: "WebService.asmx/DeleteAbsenceById",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify({ AbsenceId: absenceID }),
                success: (data) => {
                    
                    //To render the deletion immediately, a deletion must also be performed in the data table and draw.
                    //Front end cheat
                    //const RowIndex = [...ThisRow.parentNode.children].indexOf(ThisRow);
                    //DataTable.row(RowIndex).remove().draw();
                    //^^^cause to alot of synchronization probalmes, therefore refreshing the table.

                    //mabye need to color the call btn after delete
                    //console.log('Gilad check --> Delete sucsses ', data.d, absenceID, ThisAbsencesList, thisVolunteerId);
                    ColorCallBtn_afterChange(absenceID, ThisAbsencesList, thisVolunteerId)
                    RenderToAbsenceModal(ThisDisplayName, VolunteerID)
                },

                error: function (err) {
                    alert("Error in GetVolunteersAbsences !: " + err.responseText);
                    $('#wait').hide();
                }
            });
        });







    }
    //Edit This Row
    else {
        thisRowClicked = [...ThisRow.parentNode.children].indexOf(ThisRow)
        //console.log('Gilad check -- > what i got', AbsenceData);
        
        //need to get the identity-scop from database to prevent this error.
        // gilad fixed that there is no reason to this "if" just for safety.
        if (AbsenceData.Id == undefined) {
            alert('לא ניתן לעדכן את השורה יש לצאת מהחלון ולחזור כדי לעדכן.');
            return;
        }
        else {
            openDocumentAAbsenceModal();
            isEdit = true;
            $('#AAbsenceModalTitle').text('עריכת היעדרות');
        }
        $("#DocumentAAbsenceDatePickerFrom").val(ConvertDBDate2PickerFormat(AbsenceData.FromDate));
        $("#DocumentAAbsenceDatePickerUntil").val(ConvertDBDate2PickerFormat(AbsenceData.UntilDate));
        $("#DocumentAAbsence_choooseContent").val(AbsenceData.Cause);
        $("#DocumentAAbsence_writeContent").val(AbsenceData.Note);
    }

}

// this function color the btn -for indcation of availble
const ColorCallBtn_afterChange = (absenceID,absencesListOfThisVolunteer, volunteerId) => {
    if (absencesListOfThisVolunteer != undefined && absenceID!=-1) {
        const BtnToColor = document.getElementById(`${volunteerId}`).childNodes[11].childNodes[3].childNodes[1];

        const resFilter = absencesListOfThisVolunteer.filter((absence) => {
            return (absence.Id != absenceID && absence.VolunteerId == volunteerId && absence.AbsenceStatus)
        });

        //console.log('Gilad res :', ThisAbsencesList, resFilter, volunteerId, BtnToColor);
        if (resFilter.length == 0) {
            //blue available
            BtnToColor.setAttribute('style', 'background-color:#3bafda !important; border: 1px solid #3bafda !important');

        }
        else {
            //orange busy 
            BtnToColor.setAttribute('style', 'background-color:#efa834 !important; border: 1px solid #efa834 !important');

        }
    }
}

const openDocumentAAbsenceModal = () => {
    isEdit = false;
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
    $('#AAbsenceModalTitle').text('הוספת היעדרות');

    $('#DocumentAAbsence_contentErrorMsg').hide();
    $('#DocumentAAbsence_CoordinatorErrorMsg').hide();
    $('#DocumentAAbsence_DatesError').hide();


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

const checkColidAbsence = (absencList, from, until) => {
    from = new Date(from).getTime();
    until = new Date(until).getTime();
    

    if (absencList.length > 0) {
        const filter = absencList.filter((abs) => {
            ABSfrom = parseInt(abs.FromDate.replace('/Date(', '').replace(')/', ''));
            ABSuntil = parseInt(abs.UntilDate.replace('/Date(', '').replace(')/', ''));
            //console.log((ABSfrom >= from && until >= ABSfrom) || (from <= ABSuntil && until >= ABSuntil), ABSfrom, ABSuntil)
            return (ABSfrom >= from && until >= ABSfrom) || (from <= ABSuntil && until >= ABSuntil)

        })
        if (filter.length > 0) {
            return true;
        }


    }
    return false;

}

const documentAAbsence2DB = () => {

    $('#wait').show();

    //int volunteerId, int coorId, DateTime from, DateTime until, string cause, string note
    //int AbsenceId, int coorId, DateTime from, DateTime until, string cause, string note

    const NewAbsence = {
        volunteerId: parseInt(VolunteerID),
        coorId: parseInt($("#DocumentAAbsence_choooseCoordinator").val()),
        from: $("#DocumentAAbsenceDatePickerFrom").val(),
        until: $("#DocumentAAbsenceDatePickerUntil").val(),
        cause: $("#DocumentAAbsence_choooseContent").val(),
        note: $("#DocumentAAbsence_writeContent").val()
    }
    const UpdateAbsence = {
        AbsenceId: parseInt(ThisAbsenceId),
        coorId: parseInt($("#DocumentAAbsence_choooseCoordinator").val()),
        from: $("#DocumentAAbsenceDatePickerFrom").val(),
        until: $("#DocumentAAbsenceDatePickerUntil").val(),
        cause: $("#DocumentAAbsence_choooseContent").val(),
        note: $("#DocumentAAbsence_writeContent").val()
    }

    let Method = 'InsertNewAbsence';
    let dataToSend = NewAbsence;
    let until = new Date(dataToSend.until).getTime(); 
    let today = new Date()
    today = new Date(`${today.getFullYear()}-${today.getMonth() + 1}-${today.getDate()}`);
    today = today.getTime();//because today is alredy begin and until its like (00:00) in time.

    if (isEdit) {
        Method = 'UpdateAbsenceById';
        dataToSend = UpdateAbsence;
        if (dataToSend.AbsenceId==-1) {
            alert('Error no absence id in -- >documentAAbsence2DB')
            return;
        }
    }



    if (dataToSend.note == '') {
        NewAbsence.note = 'אין';
    }
   

    if (dataToSend.cause == '-1' || dataToSend.from == '' || dataToSend.until == '') {
        $('#DocumentAAbsence_contentErrorMsg').show();
        $('#DocumentAAbsence_CoordinatorErrorMsg').hide();
        $('#DocumentAAbsence_DatesError').hide();


        return;
    }
    if (new Date(dataToSend.until) < new Date(dataToSend.from)) {
        $('#DocumentAAbsence_CoordinatorErrorMsg').show();
        $('#DocumentAAbsence_contentErrorMsg').hide();
        $('#DocumentAAbsence_DatesError').hide();


        return;
    }

    if (until < today) {
        $('#DocumentAAbsence_DatesError').show();
        $('#DocumentAAbsence_contentErrorMsg').hide();
        $('#DocumentAAbsence_CoordinatorErrorMsg').hide();
        return;
    }



      
    if (checkColidAbsence(ThisAbsencesList, dataToSend.from, dataToSend.until)) {
        swal({
            title: "קיימת כפילות בהיעדרות שהכנסת להיעדרויות אחרות של המתנדב",
            type: "warning",
            text: "האם בכל זאת להקצות היעדרות זו למתנדב זה ?",
            showCancelButton: true,
            cancelButtonText: "בטל",
            confirmButtonClass: 'btn-warning',
            confirmButtonText: "כן",
            closeOnConfirm: true
        }, () => {
            $.ajax({
                dataType: "json",
                url: "WebService.asmx/" + Method,
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify(dataToSend),
                success: function (data) {

                    //let from = new Date(dataToSend.from).getTime();
                    //let until = new Date(dataToSend.until).getTime();
                    //let today = new Date()
                    //today = new Date(`${today.getFullYear()}-${today.getMonth() + 1}-${today.getDate()}`)
                    //today = today.getTime();
                    //console.log('Gilad check -->', data.d)
                    ThisAbsenceId = data.d

                    // to get the phone btn of this volunteer by DOM
                    // check the dates and color it
                    // orange mean busy
                    // blue mean available
                    //const BtnToColor = document.getElementById(`${VolunteerID}`).childNodes[11].childNodes[3].childNodes[1];

                    //if (today >= from && today<=until) {

                    //    BtnToColor.setAttribute('style', 'background-color:#efa834 !important; border: 1px solid #efa834 !important');

                    //}
                    //else {
                    //    BtnToColor.setAttribute('style', 'background-color:#3bafda !important; border: 1px solid #3bafda !important');

                    //}


                    RenderToAbsenceModal(ThisDisplayName, VolunteerID);

                },
                error: function (err) { alert("Error in Insert New Absence Ajax: " + err.responseText); $('#wait').hide(); }
            });
        });
    }
    else {
        $.ajax({
            dataType: "json",
            url: "WebService.asmx/" + Method,
            contentType: "application/json; charset=utf-8",
            type: "POST",
            data: JSON.stringify(dataToSend),
            success: function (data) {

                //let from = new Date(dataToSend.from).getTime();
                //let until = new Date(dataToSend.until).getTime();
                //let today = new Date()
                //today = new Date(`${today.getFullYear()}-${today.getMonth() + 1}-${today.getDate()}`)
                //today = today.getTime();
                //console.log('Gilad check -->', data.d)
                ThisAbsenceId = data.d

                // to get the phone btn of this volunteer by DOM
                // check the dates and color it
                // orange mean busy
                // blue mean available
                //const BtnToColor = document.getElementById(`${VolunteerID}`).childNodes[11].childNodes[3].childNodes[1];

                //if (today >= from && today<=until) {

                //    BtnToColor.setAttribute('style', 'background-color:#efa834 !important; border: 1px solid #efa834 !important');

                //}
                //else {
                //    BtnToColor.setAttribute('style', 'background-color:#3bafda !important; border: 1px solid #3bafda !important');

                //}


                RenderToAbsenceModal(ThisDisplayName, VolunteerID);

            },
            error: function (err) { alert("Error in Insert New Absence Ajax: " + err.responseText); $('#wait').hide(); }
        });
    }
    




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