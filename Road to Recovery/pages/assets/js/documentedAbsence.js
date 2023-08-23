let documentedAbsenceTable;
const RenderToAbsenceModal = (VolunteerName, VolunteerId) => {
    $('#DocumentedAbsenceTitle').text("היעדרויות של " + VolunteerName);
    console.log('Gilad check -- > ', VolunteerName, VolunteerId)
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetAbsenceByVolunteerId",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: VolunteerId }),
        success: function (data) {
            Absences = JSON.parse(data.d);

            if (documentedAbsenceTable != null) {
                console.log('Gilad check--> im in destroy', documentedAbsenceTable);
                documentedAbsenceTable.destroy();
            }

            console.log('Gilad check--> what im sent?', Absences);
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
                        data: () => {
                            return `<button>click</button><button>click2</button>`;
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