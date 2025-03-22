GlobalScoresObjectListChange = {};
EnglishToHebrewDictionary = {
    this_day_week: 'יום בשבוע',
    point_to_point: 'מנקודה לנקודה',
    point_to_area: 'מנקודה לאזור',
    area_to_area: 'מאזור לאזור',
    this_time_inDay: 'החלק הזה ביום',
    Time_since_last_ride: 'זמן מהסעה אחרונה (בשבועות)',
    is_future_Ride: 'האם יש הסעה עתידית',
    AVG_rides_week: 'ממוצע הסעות בשבוע',

}
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
            renderParameterTables(jsonRes);
            $('#wait').hide();
        },
        error: function () {
            $('#wait').hide();
            alert("שגיאה בטעינת הנתונים מהשרת.");
        }
    });

    function renderParameterTables(data) {
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
                    console.log(row)
                    table.find('tbody').append(`
                    <tr data-id="${row.Id}">
                        <td class="score_td"><input onchange="changeTheColor(this)" class="score_in" type="number" value="${row.Score}" step="0.2"></td>
                        <td class="percentage_td">${row.Parameter == `Time_since_last_ride` || row.Parameter == `AVG_rides_week`  ? row.MinRangeValue : row.MinRangeValue*100+`%` } ${row.MaxRangeValue==999?`+`:``}</td>
                    </tr>
                    `);
                });
            }


            $('#tables-container').append(table);
        });
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
                });
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



});

const changeTheColor = (input) => {
    input.parentNode.parentNode.style.backgroundColor = 'lightpink';
    GlobalScoresObjectListChange[input.parentNode.parentNode.getAttribute('data-id').toString()]=input.value;
   
}

const BacktoCandidate = () => {
    window.location.href = "viewCandidates.html";
}