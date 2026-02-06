checkCookie();

$(document).ready(function () {
    includeHTML();

    let { COPYWRITE } = GENERAL;
    $('#rights').html(COPYWRITE());

    var userName = GENERAL.USER.getUserDisplayName();
    $("#userName").html(userName);

    // Environment check
    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.toLowerCase().indexOf('test') != -1) {
        $("#na").css("background-color", "#ffde89");
    }

    // Update selected tables preview
    updateSelectedTablesPreview();
});

// Update the preview of selected tables
function updateSelectedTablesPreview() {
    const checkboxes = document.querySelectorAll('.checkbox-item input[type="checkbox"]:checked');
    const previewContainer = document.getElementById('selectedTablesPreview');

    if (checkboxes.length === 0) {
        previewContainer.innerHTML = '<em style="color: #999;">לא נבחרו טבלאות</em>';
        return;
    }

    let html = '';
    checkboxes.forEach(cb => {
        const label = cb.nextElementSibling.textContent;
        html += `<span class="selected-table-tag">${label}</span>`;
    });

    previewContainer.innerHTML = html;
}

// Clear all selections
function clearSelections() {
    document.querySelectorAll('.checkbox-item input[type="checkbox"]').forEach(cb => {
        cb.checked = false;
    });
    document.getElementById('queryInput').value = '';
    document.getElementById('resultsContainer').classList.remove('active');
    updateSelectedTablesPreview();
}

// Generate report - placeholder for your logic
function generateReport() {
    const selectedTables = [];
    document.querySelectorAll('.checkbox-item input[type="checkbox"]:checked').forEach(cb => {
        selectedTables.push(cb.value);
    });

    const query = document.getElementById('queryInput').value.trim();

    if (selectedTables.length === 0) {
        swal({
            title: "שים לב",
            text: "יש לבחור לפחות טבלה אחת",
            type: "warning"
        });
        return;
    }

    if (query === '') {
        swal({
            title: "שים לב",
            text: "יש להזין שאילתה",
            type: "warning"
        });
        return;
    }

    // TODO: Add your AI logic here
    console.log('Selected Tables:', selectedTables);
    console.log('Query:', query);

    // Show results container (placeholder)
    document.getElementById('resultsContainer').classList.add('active');
    document.getElementById('resultsContent').innerHTML = `
                <h4>התוצאות יופיעו כאן</h4>
                <p>טבלאות נבחרות: ${selectedTables.join(', ')}</p>
                <p>שאילתה: ${query}</p>
            `;
}