GlobalScoresObjectListChange = {};
let OriginalDataFromServer = [];

EnglishToHebrewDictionary = {
    this_day_week: 'יום בשבוע',
    point_to_point: 'מנקודה לנקודה (מאוחד)',
    point_to_area: 'מנקודה לאזור',
    area_to_area: 'מאזור לאזור',
    this_time_inDay: 'החלק הזה ביום',
    Time_since_last_ride: 'זמן מהסעה אחרונה (בשבועות)',
    is_future_Ride: 'האם יש הסעה עתידית',
    AVG_rides_week: 'ממוצע הסעות בשבוע',

}
const DefaultScoreValue = [
    {
        "Id": 1,
        "Parameter": "this_day_week",
        "MinRangeValue": 0,
        "MaxRangeValue": 0.2,
        "Score": 0
    },
    {
        "Id": 2,
        "Parameter": "this_day_week",
        "MinRangeValue": 0.2,
        "MaxRangeValue": 0.4,
        "Score": 1
    },
    {
        "Id": 3,
        "Parameter": "this_day_week",
        "MinRangeValue": 0.4,
        "MaxRangeValue": 0.6,
        "Score": 3
    },
    {
        "Id": 4,
        "Parameter": "this_day_week",
        "MinRangeValue": 0.6,
        "MaxRangeValue": 0.8,
        "Score": 6
    },
    {
        "Id": 5,
        "Parameter": "this_day_week",
        "MinRangeValue": 0.8,
        "MaxRangeValue": 1,
        "Score": 10
    },
    {
        "Id": 6,
        "Parameter": "point_to_point",
        "MinRangeValue": 0,
        "MaxRangeValue": 0.2,
        "Score": 0
    },
    {
        "Id": 7,
        "Parameter": "point_to_point",
        "MinRangeValue": 0.2,
        "MaxRangeValue": 0.4,
        "Score": 2
    },
    {
        "Id": 8,
        "Parameter": "point_to_point",
        "MinRangeValue": 0.4,
        "MaxRangeValue": 0.6,
        "Score": 3
    },
    {
        "Id": 9,
        "Parameter": "point_to_point",
        "MinRangeValue": 0.6,
        "MaxRangeValue": 0.8,
        "Score": 4
    },
    {
        "Id": 10,
        "Parameter": "point_to_point",
        "MinRangeValue": 0.8,
        "MaxRangeValue": 1,
        "Score": 6
    },
    {
        "Id": 21,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0,
        "MaxRangeValue": 0.2,
        "Score": 0
    },
    {
        "Id": 22,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.2,
        "MaxRangeValue": 0.3,
        "Score": 1
    },
    {
        "Id": 23,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.3,
        "MaxRangeValue": 0.4,
        "Score": 2
    },
    {
        "Id": 24,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.4,
        "MaxRangeValue": 0.5,
        "Score": 3
    },
    {
        "Id": 25,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.5,
        "MaxRangeValue": 0.6,
        "Score": 4
    },
    {
        "Id": 26,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.6,
        "MaxRangeValue": 0.7,
        "Score": 6
    },
    {
        "Id": 27,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.7,
        "MaxRangeValue": 0.8,
        "Score": 8
    },
    {
        "Id": 28,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.8,
        "MaxRangeValue": 0.9,
        "Score": 10
    },
    {
        "Id": 29,
        "Parameter": "this_time_inDay",
        "MinRangeValue": 0.9,
        "MaxRangeValue": 1,
        "Score": 12
    },
    {
        "Id": 32,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 0,
        "MaxRangeValue": 1,
        "Score": 0
    },
    {
        "Id": 33,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 1,
        "MaxRangeValue": 2,
        "Score": 1
    },
    {
        "Id": 34,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 2,
        "MaxRangeValue": 3,
        "Score": 2
    },
    {
        "Id": 35,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 3,
        "MaxRangeValue": 4,
        "Score": 3
    },
    {
        "Id": 36,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 4,
        "MaxRangeValue": 5,
        "Score": 4
    },
    {
        "Id": 37,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 5,
        "MaxRangeValue": 6,
        "Score": 5
    },
    {
        "Id": 38,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 6,
        "MaxRangeValue": 7,
        "Score": 6
    },
    {
        "Id": 39,
        "Parameter": "Time_since_last_ride",
        "MinRangeValue": 7,
        "MaxRangeValue": 999,
        "Score": 0
    },
    {
        "Id": 40,
        "Parameter": "is_future_Ride",
        "MinRangeValue": 1,
        "MaxRangeValue": 1,
        "Score": 0
    },
    {
        "Id": 41,
        "Parameter": "is_future_Ride",
        "MinRangeValue": 0,
        "MaxRangeValue": 0,
        "Score": 5
    },
    {
        "Id": 42,
        "Parameter": "AVG_rides_week",
        "MinRangeValue": 0,
        "MaxRangeValue": 1,
        "Score": 0
    },
    {
        "Id": 43,
        "Parameter": "AVG_rides_week",
        "MinRangeValue": 1,
        "MaxRangeValue": 2,
        "Score": 2
    },
    {
        "Id": 44,
        "Parameter": "AVG_rides_week",
        "MinRangeValue": 2,
        "MaxRangeValue": 3,
        "Score": 5
    },
    {
        "Id": 45,
        "Parameter": "AVG_rides_week",
        "MinRangeValue": 3,
        "MaxRangeValue": 999,
        "Score": 8
    }
]
    

$(document).ready(function () {
    const apiGetUrl = 'WebService.asmx/GetAllConfigDetails';
    const apiSaveUrl = 'WebService.asmx/UpdateScoreConfig';

    $('#wait').show();

    $.ajax({
        url: apiGetUrl,
        method: "POST",
        success: function (data) {
            let res = data.getElementsByTagName('string')[0].innerHTML;
            const jsonRes = JSON.parse(res);
            OriginalDataFromServer = JSON.parse(JSON.stringify(jsonRes)); // deep clone
            renderParameterTables(jsonRes);
            $('#wait').hide();
        },
        error: function () {
            $('#wait').hide();
            alert("שגיאה בטעינת הנתונים מהשרת.");
        }
    });

    function renderParameterTables(data, afterRenderCallback=null) {
        //console.log(data);
        const params = {};
        data.forEach(item => {
            if (!params[item.Parameter]) {
                params[item.Parameter] = [];
            }
            params[item.Parameter].push(item);
        });

        $('#tables-container').empty();
        Object.keys(params).forEach(param => {
            const table = $(
                `<table>
                    <thead>
                        <tr><th class="paramTitle" colspan=\"2\">${EnglishToHebrewDictionary[param]}</th></tr>
                        <tr>
                            <th>ניקוד</th>
                            <th>${param == `Time_since_last_ride` ? `שבועות מהסעה האחרונה` : param == `AVG_rides_week` ? `ממוצע הסעות בשבוע` : param ==`is_future_Ride`?`יש הסעה עתידית?`:`אחוז מהסעות`}</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>`
            );
            //console.log(params[param]);
            if (param == 'is_future_Ride') {
                params[param].forEach(row => {
                    table.find('tbody').append(`
                    <tr data-id="${row.Id}">
                        <td class="score_td"><input onchange="changeTheColor(this)" class="score_in" type="number" value="${row.Score}" step="0.2"></td>
                        <td class="percentage_td">${row.MinRangeValue==1 ? `יש` : `אין`}</td>
                    </tr>
                    `);
                });
            }
            else {
                params[param].forEach(row => {
                    //console.log(row)
                    table.find('tbody').append(`
                    <tr data-id="${row.Id}">
                        <td class="score_td"><input onchange="changeTheColor(this)" class="score_in" type="number" value="${row.Score}" step="0.2"></td>
                        <td class="percentage_td">${row.Parameter == `Time_since_last_ride` || row.Parameter == `AVG_rides_week` ? row.MinRangeValue : row.MinRangeValue * 100 + `%` +` - `+ row.MaxRangeValue * 100 + `%` } ${row.MaxRangeValue==999?`+`:``}</td>
                    </tr>
                    `);
                });
            }


            $('#tables-container').append(table);
        });

        if (typeof afterRenderCallback === 'function') {
            afterRenderCallback(); 
        }
    }

    $('#save-btn').click(function () {
        
        //console.log(GlobalScoresObjectListChange.length);
        const isEmpty = Object.keys(GlobalScoresObjectListChange).length === 0;
        if (isEmpty) {
            Swal.fire({
                title: "🤷‍♀️לא השתנה כלום",
                icon: "warning",
                text:'לא השתנה שום ערך ולכן לא ייעשו שינויים 🤷‍♀️',
                confirmButtonText: 'אישור'
            })
            return false;
        }
        const updatedData = [];
        for (const [id, score] of Object.entries(GlobalScoresObjectListChange)) {
            updatedData.push({
                Id: parseInt(id),
                Parameter: '',
                MinRangeValue: 0,
                MaxRangeValue: 0,
                Score: parseFloat(score)
            });
        }
        //console.log(updatedData);

       

        $.ajax({
            url: apiSaveUrl,
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({ listToUpdate: updatedData }),
            success: function () {
                
                Swal.fire({
                    title: '👌השינויים נשמרו בהצלחה!',
                    icon: 'success',
                    confirmButtonText: 'אישור'
                }).then(() => {
                    location.reload();
                })
            },
            error: function (err) {
                console.log(err);
                Swal.fire({
                    title: 'שגיאה ❌',
                    text: `${err.statusText}`,
                    icon: 'error',
                    confirmButtonText: 'אישור'
                });
            }
        });
    });

    $('#defult-value-btn').click(() => {
        renderParameterTables(DefaultScoreValue, () => {
            markDifferencesFromOriginal(DefaultScoreValue);

            Swal.fire({
                title: "הערכים חזרו לערכי ברירת מחדל",
                text: "הערכים השתנו לערכי ברירת מחדל האם תרצה לשמור אותם? \n אם כן, לחץ על אישור \n אם ברצונך להמשיך לערוך אותם לחץ על ביטול \n אם ברצונך לחזור לערכים הקודמים רענן את הדף",
                icon: "info",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "שמור את הערכים",
                cancelButtonText: "להמשיך לערוך"
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#save-btn').click();
                }
            });
        });

       
    })



});

const changeTheColor = (input) => {
    input.parentNode.parentNode.style.backgroundColor = 'lightpink';
    GlobalScoresObjectListChange[input.parentNode.parentNode.getAttribute('data-id').toString()]=input.value;
   
}

const BacktoCandidate = () => {
    window.location.href = "viewCandidates.html";
}

function markDifferencesFromOriginal(defaults) {
    GlobalScoresObjectListChange = {}; // איפוס כדי להתחיל נקי

    defaults.forEach(def => {
        const original = OriginalDataFromServer.find(x => x.Id === def.Id);
        if (!original || original.Score !== def.Score) {
            const input = document.querySelector(`tr[data-id="${def.Id}"] input.score_in`);
            if (input) {
                input.value = def.Score;
                changeTheColor(input); // גם צובע וגם מכניס למערך
            }
        }
    });
}
