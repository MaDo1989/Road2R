checkCookie();

// Store table structures globally
let tableStructures = {};

// Hebrew names for tables
const tableHebrewNames = {
    'Volunteer': 'מתנדבים',
    'Patient': 'חולים',
    'UnityRide': 'הסעות'
};

// Hebrew names for common columns
const columnHebrewNames = {
    'Id': 'מזהה',
    'DisplayName': 'שם מלא',
    'CellPhone': 'טלפון',
    'CellPhone2': 'טלפון 2',
    'IsActive': 'פעיל',
    'Gender': 'מגדר',
    'Remarks': 'הערות',
    'CityCityName': 'עיר',
    'Address': 'כתובת',
    'JoinDate': 'תאריך הצטרפות',
    'AvailableSeats': 'מקומות פנויים',
    'LastModified': 'עדכון אחרון',
    'VolunteerIdentity': 'תעודת זהות',
    'isDriving': 'האם נוהג',
    'joinYear': 'שנת הצטרפות',
    'NoOfDocumentedCalls': 'מספר שיחות',
    'NoOfDocumentedRides': 'מספר הסעות מתועדות',
    'LastUpdateBy': 'עודכן על ידי',
    'No_of_Rides': 'מספר הסעות',
    'IsBooster': 'בוסטר',
    'IsBabyChair': 'כסא תינוק',
    'IsAnonymous': 'האם אנונימי',
    'RidePatNum': 'מספר הסעה',
    'PatientName': 'שם החולה',
    'PatientCellPhone': 'טלפון החולה',
    'PatientId': 'מזהה חולה',
    'PatientGender': 'מגדר החולה',
    'PatientStatus': 'סטטוס החולה',
    'PatientBirthDate': 'תאריך לידה',
    'AmountOfEscorts': 'מספר מלווים',
    'AmountOfEquipments': 'מספר ציוד',
    'Origin': 'מוצא',
    'Destination': 'יעד',
    'pickupTime': 'שעת איסוף',
    'Coordinator': 'רכז',
    'Remark': 'הערה',
    'Status': 'סטטוס',
    'Area': 'אזור',
    'OnlyEscort': 'ליווי בלבד',
    'lastModified': 'עדכון אחרון',
    'CoordinatorID': 'מזהה רכז',
    'MainDriver': 'נהג ראשי',
    'DriverName': 'שם הנהג',
    'DriverCellPhone': 'טלפון הנהג',
    'IsNewDriver': 'האם נהג חדש'
};

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
    getTabelsStracture();
});

function getTabelsStracture() {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetStractureOfTables",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({
            tablesName: ['UnityRide', 'Volunteer', 'Patient']
        }),
        success: function (data) {
            tableStructures = JSON.parse(data.d);
            document.getElementById('ai-loader').classList.add('hidden');
            updateSelectedTablesPreview();
            console.log('Table structures loaded:', tableStructures);
        },
        error: function (err) {
            document.getElementById('ai-loader').classList.add('hidden');
            console.error('Error loading table structures:', err);
        }
    });
}

// Called when a table checkbox is toggled
function onTableSelectionChange(tableName, isChecked) {
    updateSelectedTablesPreview();
    updateColumnsSelectionUI();
}

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

// Update column selection UI based on selected tables
function updateColumnsSelectionUI() {
    const selectedTables = getSelectedTables();
    const container = document.getElementById('columnsSelectionContainer');
    const content = document.getElementById('columnsSelectionContent');

    if (selectedTables.length === 0) {
        container.classList.remove('active');
        content.innerHTML = '';
        updateTokenSavings();
        return;
    }

    container.classList.add('active');
    let html = '';

    selectedTables.forEach(tableName => {
        const columns = tableStructures[tableName] || [];
        const hebrewTableName = tableHebrewNames[tableName] || tableName;

        html += `
            <div class="table-columns-section" id="section_${tableName}">
                <div class="table-columns-header" onclick="toggleTableColumns('${tableName}')">
                    <h4>
                        <i class="fa fa-table"></i>
                        ${hebrewTableName}
                        <span class="selected-count-badge" id="count_${tableName}">0/${columns.length}</span>
                    </h4>
                    <div class="table-columns-actions" onclick="event.stopPropagation()">
                        <button class="btn-select-all" onclick="selectAllColumns('${tableName}', true)">
                            <i class="fa fa-check-square-o"></i> בחר הכל
                        </button>
                        <button class="btn-clear-all" onclick="selectAllColumns('${tableName}', false)">
                            <i class="fa fa-square-o"></i> נקה הכל
                        </button>
                    </div>
                    <i class="fa fa-chevron-down toggle-icon"></i>
                </div>
                <div class="table-columns-body" id="body_${tableName}">
                    <div class="columns-grid">
                        ${columns.map(col => {
            const colName = col.ColumnName;
            const hebrewName = columnHebrewNames[colName] || colName;
            const dataType = col.DataType;
            return `
                                <div class="column-item">
                                    <input type="checkbox" 
                                           id="col_${tableName}_${colName}" 
                                           value="${colName}"
                                           data-table="${tableName}"
                                           onchange="onColumnSelectionChange()">
                                    <label for="col_${tableName}_${colName}">${hebrewName}</label>
                                    <span class="data-type">${dataType}</span>
                                </div>
                            `;
        }).join('')}
                    </div>
                </div>
            </div>
        `;
    });

    content.innerHTML = html;
    updateTokenSavings();
}

// Toggle collapse/expand for table columns
function toggleTableColumns(tableName) {
    const header = document.querySelector(`#section_${tableName} .table-columns-header`);
    const body = document.getElementById(`body_${tableName}`);

    header.classList.toggle('collapsed');
    body.classList.toggle('collapsed');
}

// Select or clear all columns for a table
function selectAllColumns(tableName, select) {
    const checkboxes = document.querySelectorAll(`input[data-table="${tableName}"]`);
    checkboxes.forEach(cb => {
        cb.checked = select;
    });
    onColumnSelectionChange();
}

// Called when any column checkbox changes
function onColumnSelectionChange() {
    // Update count badges for each table
    const selectedTables = getSelectedTables();
    selectedTables.forEach(tableName => {
        const allCols = document.querySelectorAll(`input[data-table="${tableName}"]`);
        const selectedCols = document.querySelectorAll(`input[data-table="${tableName}"]:checked`);
        const countBadge = document.getElementById(`count_${tableName}`);
        if (countBadge) {
            countBadge.textContent = `${selectedCols.length}/${allCols.length}`;
        }
    });

    updateTokenSavings();
}

// Update token savings display
function updateTokenSavings() {
    const selectedTables = getSelectedTables();
    const savingsInfo = document.getElementById('tokenSavingsInfo');

    if (selectedTables.length === 0) {
        savingsInfo.style.display = 'none';
        return;
    }

    let totalCols = 0;
    let selectedCols = 0;

    selectedTables.forEach(tableName => {
        const allCols = document.querySelectorAll(`input[data-table="${tableName}"]`);
        const selected = document.querySelectorAll(`input[data-table="${tableName}"]:checked`);
        totalCols += allCols.length;
        selectedCols += selected.length;
    });

    if (totalCols === 0) {
        savingsInfo.style.display = 'none';
        return;
    }

    savingsInfo.style.display = 'flex';
    const savingsPercent = totalCols > 0 ? Math.round((1 - selectedCols / totalCols) * 100) : 0;

    document.getElementById('savingsPercent').textContent = savingsPercent + '%';
    document.getElementById('selectedColsCount').textContent = selectedCols;
    document.getElementById('totalColsCount').textContent = totalCols;
}

// Get list of selected tables
function getSelectedTables() {
    const tables = [];
    document.querySelectorAll('.checkbox-item input[type="checkbox"]:checked').forEach(cb => {
        tables.push(cb.value);
    });
    return tables;
}

// Get selected columns grouped by table (optimized structure for AI)
function getSelectedColumnsForAI() {
    const result = {};
    const selectedTables = getSelectedTables();

    selectedTables.forEach(tableName => {
        const selectedCols = document.querySelectorAll(`input[data-table="${tableName}"]:checked`);

        if (selectedCols.length > 0) {
            result[tableName] = [];
            selectedCols.forEach(cb => {
                const colName = cb.value;
                const colData = tableStructures[tableName]?.find(c => c.ColumnName === colName);
                if (colData) {
                    // Minimal structure to save tokens
                    result[tableName].push({
                        col: colName,
                        type: colData.DataType
                    });
                }
            });
        } else {
            // If no columns selected, include all (for backward compatibility)
            result[tableName] = tableStructures[tableName]?.map(c => ({
                col: c.ColumnName,
                type: c.DataType
            })) || [];
        }
    });

    return result;
}

// Clear all selections
function clearSelections() {
    document.querySelectorAll('.checkbox-item input[type="checkbox"]').forEach(cb => {
        cb.checked = false;
    });
    document.getElementById('queryInput').value = '';
    document.getElementById('resultsContainer').classList.remove('active');
    document.getElementById('columnsSelectionContainer').classList.remove('active');
    document.getElementById('columnsSelectionContent').innerHTML = '';
    updateSelectedTablesPreview();
    updateTokenSavings();
}

// Generate report
function generateReport() {
    const selectedTables = getSelectedTables();
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
    USER_PROMPT = query;
    // Get only the selected columns (optimized for tokens)
    const selectedColumns = getSelectedColumnsForAI();
    // This is what you'll send to the AI API - much smaller payload!
    const aiPayload = {
        tables: selectedColumns,
        query: query
    };
    TABEL_NAMES = Object.keys(aiPayload.tables).join(",");
    const request = {
        prompt: PromptBuilder(selectedColumns, query),
        thinkingLevel: "LOW",
        model: "gemini-3-flash-preview"
    }
    document.getElementById('ai-loader').classList.remove('hidden');
    $.ajax({
        url: 'https://querymaker.onrender.com/generate',
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify(request),
        success: generateReport_SCB,
        error: generateReport_ECB
    })

    // Show results container (placeholder)
    document.getElementById('resultsContainer').classList.add('active');

}

function generateReport_SCB(data) {
    console.log('success , ', data);
    const Request = {
        UserPhone: localStorage.getItem("user"),
        UserName: localStorage.getItem("userCell"),
        ExecutionTime: new Date().toISOString(),
        SqlQuery: data.data.SQL_Query,
        AiDescription: data.data.description,
        UserPrompt: USER_PROMPT,
        RelatedTables: TABEL_NAMES

    };
    //console.log(Request);
    finalExecution(Request);
}
function generateReport_ECB(err) {
    console.error('error , ', err);

}
function PromptBuilder(stracture, userPrompt) {
    let FinalPrompt = `Write an SQL query according to the following instructions:
    ${userPrompt}
    And use the following description to help you understand the structure of the data in the database:
    ${JSON.stringify(stracture)}
    `
    return FinalPrompt;
}

function finalExecution(request) {

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetQueryResults",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ exec: request }),
        success: function (data) {
            document.getElementById('ai-loader').classList.add('hidden');
            console.log('finalExecution raw results:', data.d);

            try {
                let parsedData = data.d;

                // If it's a string, parse it
                if (typeof parsedData === 'string') {
                    parsedData = JSON.parse(parsedData);
                }

                console.log('Parsed data type:', typeof parsedData, Array.isArray(parsedData));
                console.log('Parsed data:', parsedData);

                FINAL_RESULTS = parsedData;
                REPORT_DT = renderDataTable(FINAL_RESULTS, 'resultsContent');

            } catch (parseError) {
                console.error('Error parsing results:', parseError);
                document.getElementById('resultsContent').innerHTML = '<p style="color: red; text-align: center;">שגיאה בעיבוד התוצאות: ' + parseError.message + '</p>';
            }
        },
        error: function (err) {
            document.getElementById('ai-loader').classList.add('hidden');
            console.error('Error api GetQueryResults:', err);
            document.getElementById('resultsContent').innerHTML = '<p style="color: red; text-align: center;">שגיאה בקבלת התוצאות</p>';
        }
    });
}



function renderDataTable(data, containerId, options = {}) {
    const container = document.getElementById(containerId);

    if (!container) {
        console.error('Container not found:', containerId);
        return null;
    }

    // Handle null/undefined
    if (data === null || data === undefined) {
        container.innerHTML = '<p style="text-align: center;">אין נתונים להצגה</p>';
        return null;
    }

    // If data is a string, try to parse it
    if (typeof data === 'string') {
        try {
            data = JSON.parse(data);
        } catch (e) {
            console.error('Failed to parse data string:', e);
            container.innerHTML = '<p style="text-align: center; color: red;">שגיאה בפענוח הנתונים</p>';
            return null;
        }
    }

    // If data is an object but not an array, check for common wrapper properties
    if (data && typeof data === 'object' && !Array.isArray(data)) {
        if (Array.isArray(data.results)) {
            data = data.results;
        } else if (Array.isArray(data.data)) {
            data = data.data;
        } else if (Array.isArray(data.rows)) {
            data = data.rows;
        } else if (Array.isArray(data.items)) {
            data = data.items;
        } else if (Array.isArray(data.records)) {
            data = data.records;
        } else {
            data = [data];
        }
    }

    // Final validation - must be an array
    if (!Array.isArray(data)) {
        console.error('Data is not an array:', typeof data, data);
        container.innerHTML = '<p style="text-align: center; color: red;">פורמט נתונים לא תקין</p>';
        return null;
    }

    // Check for empty array
    if (data.length === 0) {
        container.innerHTML = '<p style="text-align: center;">אין נתונים להצגה</p>';
        return null;
    }

    // Extract columns from all objects (in case some rows have extra keys)
    const colSet = new Set();
    data.forEach(row => {
        if (row && typeof row === 'object') {
            Object.keys(row).forEach(k => colSet.add(k));
        }
    });
    const columns = [...colSet];

    if (columns.length === 0) {
        container.innerHTML = '<p style="text-align: center;">אין עמודות להצגה</p>';
        return null;
    }

    // Generate unique table ID
    const tableId = `dt_${containerId}_${Date.now()}`;

    // Detect date columns for sorting
    const columnDefs = columns.map((col, i) => {
        const isDate = data.some(r => r && r[col] && /^\d{4}-\d{2}-\d{2}T/.test(String(r[col])));
        if (isDate) {
            return {
                targets: i,
                render: function (val) {
                    if (!val) return '';
                    const d = new Date(val);
                    if (isNaN(d.getTime())) return val;
                    return d.toLocaleDateString('he-IL') + ' ' + d.toLocaleTimeString('he-IL', { hour: '2-digit', minute: '2-digit' });
                }
            };
        }
        return null;
    }).filter(Boolean);

    // Build DataTables column definitions for data-driven mode
    const dtColumns = columns.map(col => ({
        title: columnHebrewNames[col] || col,
        data: col,
        defaultContent: '',
        render: function (val) {
            if (val === null || val === undefined) return '';
            if (typeof val === 'string') {
                return val.replace(/</g, '&lt;').replace(/>/g, '&gt;');
            }
            return val;
        }
    }));

    // Merge date renderers into dtColumns
    columnDefs.forEach(def => {
        if (def && def.targets !== undefined) {
            dtColumns[def.targets].render = def.render;
        }
    });

    // Clear container and create a wrapper with overflow support
    container.innerHTML = `<div class="datatable-scroll-wrapper"><table id="${tableId}" class="display nowrap cell-border" style="width:100%"></table></div>`;

    // Check if DataTable is available
    if (typeof $.fn.DataTable === 'undefined') {
        console.warn('DataTables plugin not loaded - rendering fallback table');
        // Fallback: build a plain HTML table inside the scroll wrapper
        let html = `<div class="datatable-scroll-wrapper"><table id="${tableId}" class="ai-report-table"><thead><tr>`;
        columns.forEach(col => {
            html += `<th>${columnHebrewNames[col] || col}</th>`;
        });
        html += '</tr></thead><tbody>';
        data.forEach(row => {
            html += '<tr>';
            columns.forEach(col => {
                let val = '';
                if (row && row[col] !== undefined && row[col] !== null) {
                    val = row[col];
                    if (typeof val === 'string') {
                        val = val.replace(/</g, '&lt;').replace(/>/g, '&gt;');
                    }
                }
                html += `<td>${val}</td>`;
            });
            html += '</tr>';
        });
        html += '</tbody></table></div>';
        container.innerHTML = html;
        return null;
    }

    // Destroy existing DataTable on this container if any
    if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
        $(`#${tableId}`).DataTable().destroy();
    }

    // Use setTimeout to let the browser complete the layout pass after innerHTML
    setTimeout(function () {
        try {
            const dtOptions = {
                data: data,
                columns: dtColumns,
                pageLength: options.pageLength || 25,
                scrollX: true,
                autoWidth: false,
                language: options.language || {
                    search: "חיפוש:",
                    lengthMenu: "הצג _MENU_ רשומות",
                    info: "מציג _START_ עד _END_ מתוך _TOTAL_ רשומות",
                    paginate: { first: "ראשון", last: "אחרון", next: "הבא", previous: "הקודם" },
                    emptyTable: "אין נתונים",
                    zeroRecords: "לא נמצאו תוצאות"
                },
                initComplete: function () {
                    // Adjust columns after full init
                    this.api().columns.adjust();
                    console.log('DataTable initialized successfully');
                }
            };

            // Add buttons only if the Buttons extension is available
            if ($.fn.DataTable.Buttons) {
                dtOptions.dom = 'Bfrtip';
                dtOptions.buttons = [
                    { extend: 'excelHtml5', text: 'Excel', exportOptions: { orthogonal: 'export' } },
                    { extend: 'csvHtml5', text: 'CSV' },
                    { extend: 'print', text: 'הדפסה' },
                    { extend: 'colvis', text: 'עמודות' }
                ];
            }

            // Merge any additional options
            if (options.dtOptions) {
                Object.assign(dtOptions, options.dtOptions);
            }

            const dt = $(`#${tableId}`).DataTable(dtOptions);
            REPORT_DT = dt;

        } catch (dtError) {
            console.error('Error initializing DataTable:', dtError);
        }
    }, 50);

    return null;
}
