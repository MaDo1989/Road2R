//const parameters = [
//    { name: 'C_NoOfDocumentedRides', description: 'מספר הנסיעות המתועדות', newbis: 3.0, regulars: 0.5, super: 0.5 },
//    { name: 'C_SeniorityInYears', description: 'ותק בשנים', newbis: 0.1, regulars: 0.25, super: 0.25 },
//    { name: 'C_LastRideInDays', description: 'מספר ימים מאז הנסיעה האחרונה', newbis: 0.02, regulars: 0.1, super: 0.1 },
//    { name: 'C_NextRideInDays', description: 'מספר ימים עד הנסיעה הבאה', newbis: 0.02, regulars: 0.1, super: 0.1 },
//    { name: 'C_NumOfRidesLast2Month', description: 'מספר הנסיעות בחודשיים האחרונים', newbis: 3.5, regulars: 0.5, super: 0.5 },
//    { name: 'C_AmountOfRidesInThisPath', description: 'כמות הנסיעות במסלול זה', newbis: 5.0, regulars: 7.5, super: 7.5 },
//    { name: 'C_AmountOfRidesInOppositePath', description: 'כמות הנסיעות במסלול ההפוך', newbis: 4.0, regulars: 3.0, super: 3.0 },
//    { name: 'C_AmountOfRides_OriginToArea', description: 'כמות הנסיעות מנקודת המוצא לאזור', newbis: 3.5, regulars: 3.0, super: 3.0 },
//    { name: 'C_AmountOfRidesAtThisTime', description: 'כמות הנסיעות בשעה זו', newbis: 4.5, regulars: 5.0, super: 5.0 },
//    { name: 'C_AmountOfRidesAtThisDayWeek', description: 'כמות הנסיעות ביום זה בשבוע', newbis: 5.0, regulars: 2.5, super: 2.5 },
//    { name: 'C_AmountOfRidesFromRegionToDest', description: 'כמות הנסיעות מהאזור ליעד', newbis: 3.5, regulars: 3.0, super: 3.0 },
//    { name: 'C_SumOfKM', description: 'סך הקילומטרים', newbis: 100.0, regulars: 100.0, super: 100.0 }
//];


const DefulatParams = [
    { name: 'C_NoOfDocumentedRides', description: 'מספר הנסיעות המתועדות', newbis: 3.0, regulars: 0.5, super: 0.5 },
    { name: 'C_SeniorityInYears', description: 'ותק בשנים', newbis: 0.1, regulars: 0.25, super: 0.25 },
    { name: 'C_LastRideInDays', description: 'מספר ימים מאז הנסיעה האחרונה', newbis: 0.02, regulars: 0.1, super: 0.1 },
    { name: 'C_NextRideInDays', description: 'מספר ימים עד הנסיעה הבאה', newbis: 0.02, regulars: 0.1, super: 0.1 },
    { name: 'C_NumOfRidesLast2Month', description: 'מספר הנסיעות בחודשיים האחרונים', newbis: 3.5, regulars: 0.5, super: 0.5 },
    { name: 'C_AmountOfRidesInThisPath', description: 'כמות הנסיעות במסלול זה', newbis: 5.0, regulars: 7.5, super: 7.5 },
    { name: 'C_AmountOfRidesInOppositePath', description: 'כמות הנסיעות במסלול ההפוך', newbis: 4.0, regulars: 3.0, super: 3.0 },
    { name: 'C_AmountOfRides_OriginToArea', description: 'כמות הנסיעות מנקודת המוצא לאזור', newbis: 3.5, regulars: 3.0, super: 3.0 },
    { name: 'C_AmountOfRidesAtThisTime', description: 'כמות הנסיעות בשעה זו', newbis: 4.5, regulars: 5.0, super: 5.0 },
    { name: 'C_AmountOfRidesAtThisDayWeek', description: 'כמות הנסיעות ביום זה בשבוע', newbis: 5.0, regulars: 2.5, super: 2.5 },
    { name: 'C_AmountOfRidesFromRegionToDest', description: 'כמות הנסיעות מהאזור ליעד', newbis: 3.5, regulars: 3.0, super: 3.0 },
    { name: 'C_SumOfKM', description: 'סך הקילומטרים', newbis: 100.0, regulars: 100.0, super: 100.0 }
];



$(document).ready(() => {
    $('#wait').show();
    $.ajax({
        url: 'WebService.asmx/GetWeightsOfCandidateV2',  // החלף זאת בנתיב המדויק לשירות שלך
        type: 'POST',
        success: function (response) {
            $('#wait').hide();
            console.log(response);
            let res = response.getElementsByTagName('string')[0].innerHTML
            const weights = JSON.parse(res);
            console.log(weights);
            parameters = convertDataStructure(weights);
            console.log(parameters);
            createParameterTable(parameters);
          

        
        },
        error: function (xhr, status, error) {
            $('#wait').hide();
            console.error("error in GetWeightsOfCandidateV2 api:", error);
            Swal.fire({
                title: "שגיאה בAPI ",
                text: "יש שגיאה בקבלת המשקולות",
                icon: "error"
            });
            //var errorMessage = xhr.responseText;
            //console.error("error in UpdateCandidateWeights api details", errorMessage);
        }
    });

})

function createParameterTable(parms) {
    const tbody = document.querySelector('#parametersTable tbody');
    tbody.innerHTML = '';

    parms.forEach(param => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${param.name}</td>
                    <td>${param.description}</td>
                    <td><input type="number" id="${param.name}_newbis" value="${param.newbis}" step="0.001"></td>
                    <td><input type="number" id="${param.name}_regulars" value="${param.regulars}" step="0.001"></td>
                    <td><input type="number" id="${param.name}_super" value="${param.super}" step="0.001"></td>
                `;
        tbody.appendChild(tr);
    });
}

function saveConfiguration() {
    const config = {
        newbis: {},
        regulars: {},
        super: {}
    };
    parameters.forEach(param => {
        config.newbis[param.name] = parseFloat(document.getElementById(`${param.name}_newbis`).value);
        config.regulars[param.name] = parseFloat(document.getElementById(`${param.name}_regulars`).value);
        config.super[param.name] = parseFloat(document.getElementById(`${param.name}_super`).value);
    });
    console.log('קונפיגורציה נשמרה:', config);
    let arrays = convertToArrays(config)
    $.ajax({
        url: 'WebService.asmx/UpdateCandidateWeights',  // החלף זאת בנתיב המדויק לשירות שלך
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            newbis_W: arrays.newbis,  // דוגמה לערכים, החלף באלו שאתה רוצה לשלוח
            regular_W: arrays.regulars,
            super_W: arrays.super
        }),
        success: function (response) {
            if (response.d === "success") {
                Swal.fire({
                    title: "נשמר",
                    text: "המשקולות עודכנו בהצלחה.",
                    icon: "success"
                });
            } else {
                console.error("error in UpdateCandidateWeights api  :", response);
            }
        },
        error: function (xhr, status, error) {
            console.error("error in UpdateCandidateWeights api:", error);
            Swal.fire({
                title: "שגיאה בAPI ",
                text: "יש שגיאה בעדכון המשקולות",
                icon: "error"
            });
            //var errorMessage = xhr.responseText;
            //console.error("error in UpdateCandidateWeights api details", errorMessage);
        }
    });
}




const defultValues = () => {
    createParameterTable(DefulatParams);
}

const BacktoCandidate = () => {
    window.location.href = "viewCandidates.html";
}






function convertDataStructure(inputData) {
    const names = [
        'C_NoOfDocumentedRides', 'C_SeniorityInYears', 'C_LastRideInDays', 'C_NextRideInDays',
        'C_NumOfRidesLast2Month', 'C_AmountOfRidesInThisPath', 'C_AmountOfRidesInOppositePath',
        'C_AmountOfRides_OriginToArea', 'C_AmountOfRidesAtThisTime', 'C_AmountOfRidesAtThisDayWeek',
        'C_AmountOfRidesFromRegionToDest', 'C_SumOfKM'
    ];

    const descriptions = [
        'מספר הנסיעות המתועדות', 'ותק בשנים', 'מספר ימים מאז הנסיעה האחרונה', 'מספר ימים עד הנסיעה הבאה',
        'מספר הנסיעות בחודשיים האחרונים', 'כמות הנסיעות במסלול זה', 'כמות הנסיעות במסלול ההפוך',
        'כמות הנסיעות מנקודת המוצא לאזור', 'כמות הנסיעות בשעה זו', 'כמות הנסיעות ביום זה בשבוע',
        'כמות הנסיעות מהאזור ליעד', 'סך הקילומטרים'
    ];

    return names.map((name, index) => ({
        name: name,
        description: descriptions[index],
        newbis: inputData.Item1[index],
        regulars: inputData.Item2[index],
        super: inputData.Item3[index]
    }));
}

function convertToArrays(inputData) {
    const categories = ['newbis', 'regulars', 'super'];
    const result = {};

    categories.forEach(category => {
        result[category] = Object.values(inputData[category]);
    });

    return result;
}