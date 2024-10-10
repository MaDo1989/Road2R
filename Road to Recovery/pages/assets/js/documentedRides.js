let historyTable;

//make the flat object to complex object like the old API
const CustomRideObject = (thisRide) => {

    let CustomObj = {};
    CustomObj.RidePatNum = thisRide.RidePatNum;
    CustomObj.Status = thisRide.Status;
    CustomObj.Remark = thisRide.Remark;
    CustomObj.OnlyEscort = thisRide.OnlyEscort;
    CustomObj.LastModified = thisRide.LastModified;
    CustomObj.Date = thisRide.PickupTime;

    CustomObj.Pat = {};
    CustomObj.Pat.DisplayName = thisRide.PatientName;
    CustomObj.Pat.Id = thisRide.PatientId;
    CustomObj.Pat.DisplayNameA = ''
    CustomObj.Pat.EnglishName = ''
    CustomObj.Pat.PatientIdentity = 0;
    CustomObj.Pat.Equipment = thisRide.PatientEquipments == null ? [] : thisRide.PatientEquipments;
    CustomObj.Pat.IsAnonymous = thisRide.IsAnonymous;
    CustomObj.Pat.RidePatPatientStatus = {};
    CustomObj.Pat.RidePatPatientStatus.Status = (thisRide.PatientStatus == "" || thisRide.PatientStatus == "Not Finished") ? -1 : 1;
    CustomObj.Pat.RidePatPatientStatus.EditTimeStamp = thisRide.PatientStatusEditTime

    CustomObj.Pat.EscortedList = [];
    CustomObj.Escorts = [];
    //for (var i = 0; i < thisRide[18].Value.length; i++) {
    //    let oneEscort = {}
    //    oneEscort.Id = thisRide[18].Value[i][0].Value;
    //    oneEscort.DisplayName = thisRide[18].Value[i][1].Value;
    //    CustomObj.Escorts.push(oneEscort);
    //}
    for (var i = 0; i < thisRide.AmountOfEscorts; i++) {
        let oneEscort = {}
        oneEscort.Id = 0
        oneEscort.DisplayName = ''
        CustomObj.Pat.EscortedList.push(oneEscort);
    }

    for (var i = 0; i < thisRide.AmountOfEscorts; i++) {
        let oneEscort = {}
        oneEscort.Id = 0
        oneEscort.DisplayName = ''
        CustomObj.Escorts.push(oneEscort);
    }

    CustomObj.Drivers = [];
    let Maindriver = {};
    Maindriver.DisplayName = thisRide.DriverName;
    Maindriver.Id = thisRide.MainDriver;
    Maindriver.CellPhone = thisRide.DriverCellPhone
    CustomObj.Drivers.push(Maindriver);
    if (Maindriver.Id == null || Maindriver.Id == -1) {
        CustomObj.Drivers = [];
    }

    CustomObj.Destination = {};
    CustomObj.Destination.Name = thisRide.Destination
    CustomObj.Origin = {};
    CustomObj.Origin.Name = thisRide.Origin;

    CustomObj.Pat.Barrier = {}
    CustomObj.Pat.Hospital = {}
    CustomObj.Pat.Barrier.Name = CustomObj.Destination.Name;
    CustomObj.Pat.Hospital.Name = CustomObj.Origin.Name;

    CustomObj.Coordinator = {};
    CustomObj.Coordinator.DisplayName = thisRide.CoorName
    CustomObj.Coordinator.Id = thisRide.CoorId

    return CustomObj;
}


function manipulateDocumentedRidesModal(button, tableToWithdrawDataFrom) {

    $('#wait').show();

    let rowData = tableToWithdrawDataFrom.row($(button).parents('tr')).data();

    $('#documentedRidesTitle').text("תיעוד הסעות " + rowData.DisplayName.split('<br>')[0])
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetVolunteersDocumentedUnityRides",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: rowData.Id }),
        success: function (data) {
            data = JSON.parse(data.d)
            //console.log('Gilad Need check before - >',data)
            let arrRes = []
            for (var i = 0; i < data.length; i++) {
                let ur = CustomRideObject(data[i]);
                arrRes.push(ur);
            }
            //console.log('Gilad Need check after - >', arrRes)
            data = arrRes; 

            if (historyTable != null) {
                historyTable.destroy();
            }


            historyTable = $('#documentedRidesTable').DataTable({
                order: [[5, "desc"]],
                pageLength: 10,
                data: data,
                columns: [
                    {
                        data: (data) => {
                            return ConvertDBDate2UIDate(data.Date);
                        }
                    },
                    {
                        data: (data) => {
                            if (data.Date === undefined) return;

                            let fullTimeStempStr = data.Date;
                            let startTrim = fullTimeStempStr.indexOf('(') + 1;
                            let endTrim = fullTimeStempStr.indexOf(')');
                            let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
                            let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));

                            if (fullTimeStemp.getMinutes() === 14) {
                                if (fullTimeStemp.getHours() === 19 || fullTimeStemp.getHours() === 20 || fullTimeStemp.getHours() === 21 || fullTimeStemp.getHours() === 22) {
                                    return 'אחה"צ';
                                }
                            }

                            let hh = fullTimeStemp.getHours() < 10 ? "0" + fullTimeStemp.getHours() : fullTimeStemp.getHours();
                            hh += ":";
                            let mm = fullTimeStemp.getMinutes() < 10 ? "0" + fullTimeStemp.getMinutes() : fullTimeStemp.getMinutes();
                            return hh + mm;
                        }
                    },
                    {
                        data: (data) => {
                            if (data.Origin === undefined || data.Destination === undefined) return;

                            let fullPath = data.Origin.Name + " ← " + data.Destination.Name;
                            return fullPath;
                        }
                    },
                    { data: "Pat.DisplayName" },
                    { data: "Remark" },
                    {
                        data: (data) => {
                            return convertDBDate2FrontEndDate(data.Date);
                        }
                    },

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
                    { "targets": [5], visible: false },
                    //↑
                    {
                        "targets": [0],
                        render: function (data, type, full, meta) {
                            let now = new Date();
                            if (convertDBDate2FrontEndDate(full.Date) > now) {
                                var rowIndex = meta.row + 1;
                                $('#documentedRidesTable tbody tr:nth-child(' + rowIndex + ')').addClass('futureRide');
                                return data;
                            } else {
                                return data;
                            }
                        }
                    },
                    { "targets": 0, width: "10%" },
                    { "targets": 1, width: "10%" },
                    { "targets": 2, width: "20%" },
                    { "targets": 3, width: "25%" },
                    { "targets": 4, width: "35%" }

                ]
            });
            $('#wait').hide();
            $('#documentedRidesTable').css('width', '100%');

        },
        error: function (err) {
            alert("Error in GetVolunteersRideHistory: " + err.responseText);
            $('#wait').hide();
        }
    });

}