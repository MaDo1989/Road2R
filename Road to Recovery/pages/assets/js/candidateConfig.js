const parameters = [
    { name: 'C_NoOfDocumentedRides', description: 'מספר הנסיעות המתועדות', newbis: 0.03, regulars: 0.303, super: 0.303 },
    { name: 'C_SeniorityInYears', description: 'ותק בשנים', newbis: 0.01, regulars: 0.15, super: 0.15 },
    { name: 'C_LastRideInDays', description: 'מספר ימים מאז הנסיעה האחרונה', newbis: 0.002, regulars: 0.02, super: 0.02 },
    { name: 'C_NextRideInDays', description: 'מספר ימים עד הנסיעה הבאה', newbis: 0.002, regulars: 0.2, super: 0.2 },
    { name: 'C_NumOfRidesLast2Month', description: 'מספר הנסיעות בחודשיים האחרונים', newbis: 0.033, regulars: 2.33, super: 2.33 },
    { name: 'C_AmountOfRidesInThisPath', description: 'כמות הנסיעות במסלול זה', newbis: 0.5, regulars: 6.5, super: 8.5 },
    { name: 'C_AmountOfRidesInOppositePath', description: 'כמות הנסיעות במסלול ההפוך', newbis: 0.025, regulars: 2.0025, super: 3.0025 },
    { name: 'C_AmountOfRides_OriginToArea', description: 'כמות הנסיעות מנקודת המוצא לאזור', newbis: 0.033, regulars: 1.4033, super: 2.4033 },
    { name: 'C_AmountOfRidesAtThisTime', description: 'כמות הנסיעות בשעה זו', newbis: 0.1, regulars: 0.91, super: 0.91 },
    { name: 'C_AmountOfRidesAtThisDayWeek', description: 'כמות הנסיעות ביום זה בשבוע', newbis: 0.1, regulars: 0.31, super: 0.31 },
    { name: 'C_AmountOfRidesFromRegionToDest', description: 'כמות הנסיעות מהאזור ליעד', newbis: 0.1, regulars: 0.1, super: 0.1 },
    { name: 'C_SumOfKM', description: 'סך הקילומטרים', newbis: 100, regulars: 100, super: 0.015 }
];

function createParameterTable() {
    const tbody = document.querySelector('#parametersTable tbody');
    tbody.innerHTML = '';

    parameters.forEach(param => {
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
    alert('הקונפיגורציה נשמרה בהצלחה!');
}

createParameterTable();




const BacktoCandidate = () => {
    window.location.href = "viewCandidates.html";
}