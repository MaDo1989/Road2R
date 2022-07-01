let historyTable;

function manipulateDocumentedRidesModal(button, tableToWithdrawDataFrom) {

    $('#wait').show();

    let rowData = tableToWithdrawDataFrom.row($(button).parents('tr')).data();

    $('#documentedRidesTitle').text("תיעוד הסעות " + rowData.DisplayName)
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetVolunteersDocumentedRides",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: rowData.Id }),
        success: function (data) {
            data = JSON.parse(data.d)

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

        },
        error: function (err) {
            alert("Error in GetVolunteersRideHistory: " + err.responseText);
            $('#wait').hide();
        }
    });

}