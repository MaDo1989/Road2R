var DAYS_HE = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];
var SHIFTS = [
  { key: "בוקר", label: "בוקר 🌞" },
  { key: "אחהצ", label: 'אחה"צ 🌅' },
];

var selectedDays = {}; // { "ראשון": Set("בוקר", ...), ... }
var selectedAreas = {}; // { HebrewName: true }
var allAreas = [];
var availableSeats = 0;
var MIN_SEATS = 0;
var MAX_SEATS = 8;

/* ---- Profile card ---- */
function renderProfileCard(user) {
  var initial = (user.DisplayName || user.FirstNameH || "").trim().charAt(0);
  $("#profileAvatar").text(initial);
  $("#profileName").text(user.DisplayName || "");

  $("#profileBadge").attr("hidden", user.IsActive ? null : "hidden");

  var rideCount =
    user.NoOfRides != null ? user.NoOfRides : user.NoOfDocumentedRides;
  var parts = [];
  if (rideCount != null) parts.push(rideCount + " הסעות");
  if (user.JoinYear != null) parts.push("פעיל מ-" + user.JoinYear);
  $("#profileStats").text(parts.join(" • "));

  availableSeats = user.AvailableSeats != null ? user.AvailableSeats : 0;
  renderSeats();
}

/* ---- Seats stepper ---- */
function renderSeats() {
  $("#seatsValue").text(availableSeats);
  $("#seatsMinus").prop("disabled", availableSeats <= MIN_SEATS);
  $("#seatsPlus").prop("disabled", availableSeats >= MAX_SEATS);
}

function changeSeats(delta) {
  var next = availableSeats + delta;
  if (next < MIN_SEATS || next > MAX_SEATS) return;
  availableSeats = next;
  renderSeats();
}

/* ---- Tabs ---- */
function setActivePanel(panel) {
  $(".pref-tab").each(function () {
    var isActive = $(this).data("panel") === panel;
    $(this).toggleClass("is-active", isActive);
    $(this).attr("aria-selected", isActive ? "true" : "false");
  });
  $(".pref-panel").attr("hidden", "hidden");
  $("#panel" + panel.charAt(0).toUpperCase() + panel.slice(1)).removeAttr(
    "hidden",
  );
}

/* ---- Days / shifts ---- */
function isShiftSelected(day, shiftKey) {
  return !!(selectedDays[day] && selectedDays[day][shiftKey]);
}

function toggleShift(day, shiftKey) {
  selectedDays[day] = selectedDays[day] || {};
  var isSelected = !!selectedDays[day][shiftKey];

  if (isSelected) {
    delete selectedDays[day][shiftKey];
  } else {
    if (shiftKey === "כל") {
      selectedDays[day] = {};
    } else {
      delete selectedDays[day]["כל"];
    }
    selectedDays[day][shiftKey] = true;
  }
}

function buildDayRow(day) {
  var $row = $(
    '<div class="day-row">' +
      '<span class="day-row__label"></span>' +
      '<div class="day-row__shifts"></div>' +
      "</div>",
  );
  $row.find(".day-row__label").text(day);

  var bothHalvesSelected =
    isShiftSelected(day, "בוקר") && isShiftSelected(day, "אחהצ");

  var $shifts = $row.find(".day-row__shifts");
  SHIFTS.forEach(function (shift) {
    // "כל היום" is redundant once both half-day shifts are picked individually.
    if (shift.key === "כל" && bothHalvesSelected) return;

    var $chip = $('<button type="button" class="shift-chip"></button>')
      .text(shift.label)
      .attr("data-day", day)
      .attr("data-shift", shift.key)
      .toggleClass("is-selected", isShiftSelected(day, shift.key));
    $shifts.append($chip);
  });

  return $row;
}

function renderDayList() {
  var $list = $("#dayList");
  $list.empty();
  DAYS_HE.forEach(function (day) {
    $list.append(buildDayRow(day));
  });
}

/* ---- Areas ---- */
function buildAreaChip(area) {
  var isSelected = !!selectedAreas[area.HebrewName];
  return $(
    '<button type="button" class="area-chip">' +
      '<span class="area-chip__check" aria-hidden="true">✓</span>' +
      '<span class="area-chip__label"></span>' +
      "</button>",
  )
    .attr("data-area", area.HebrewName)
    .attr("aria-pressed", isSelected ? "true" : "false")
    .toggleClass("is-selected", isSelected)
    .find(".area-chip__label")
    .text(area.HebrewName)
    .end();
}

function renderAreaChips() {
  var $wrap = $("#areaChips");
  $wrap.empty();

  if (allAreas.length === 0) {
    $wrap.append('<p class="section-hint">לא נמצאו אזורים להצגה.</p>');
    return;
  }

  allAreas.forEach(function (area) {
    $wrap.append(buildAreaChip(area));
  });
}

/* ---- Data loading ---- */
function applyExistingPreferences(prefs) {
  (prefs.PreferredDays || []).forEach(function (entry) {
    var day = entry.PreferedDayDayInWeek;
    var shift = entry.Shift;
    if (!day || !shift) return;
    selectedDays[day] = selectedDays[day] || {};
    selectedDays[day][shift] = true;
  });

  (prefs.PreferredAreas || []).forEach(function (entry) {
    if (entry.PreferredArea) selectedAreas[entry.PreferredArea] = true;
  });
}

function loadPreferences(volunteerId) {
  MASTER.ajax(
    "GetVolunteerPreferencesMobile",
    { volunteerId: volunteerId },
    function (wrapper) {
      var prefs = MASTER.parseResponse(wrapper) || {};
      applyExistingPreferences(prefs);
      renderDayList();
      renderAreaChips();
    },
    function (xhr, status, error) {
      MASTER.devLog("Error loading preferences: " + error, "error");
      renderDayList();
      MASTER.showToast("שגיאה בטעינת ההעדפות. נסו שנית.", "error");
    },
  );
}

function loadAreas() {
  MASTER.ajax(
    "getAreas",
    {},
    function (wrapper) {
      var areas = MASTER.parseResponse(wrapper) || [];
      allAreas = areas.filter(function (area) {
        return !!area.IsRoute;
      });
      renderAreaChips();
    },
    function (xhr, status, error) {
      MASTER.devLog("Error loading areas: " + error, "error");
      MASTER.showToast("שגיאה בטעינת האזורים. נסו שנית.", "error");
    },
  );
}

$(function () {
  MASTER.renderHeader("#appHeader", { title: "העדפות שלי" });

  var user = MASTER.getCurrentUser();
  if (!user) {
    window.location.replace("login.html");
    return;
  }

  renderProfileCard(user);
  loadPreferences(user.Id);
  loadAreas();

  MASTER.IsProductionDatabase(function (isProd) {
    if (!isProd) $(".header").css("background-color", "#f39c12");
  });

  $(document).on("click", ".shift-chip", function () {
    var day = $(this).data("day");
    var shift = $(this).data("shift");
    toggleShift(day, shift);
    renderDayList();
  });

  $(document).on("click", ".area-chip", function () {
    var area = $(this).data("area");
    selectedAreas[area] = !selectedAreas[area];
    if (!selectedAreas[area]) delete selectedAreas[area];
    renderAreaChips();
  });

  $(".pref-tab").on("click", function () {
    setActivePanel($(this).data("panel"));
  });

  $("#seatsMinus").on("click", function () {
    changeSeats(-1);
  });
  $("#seatsPlus").on("click", function () {
    changeSeats(1);
  });

  $("#saveProfileBtn, #saveDaysBtn, #saveAreasBtn").on("click", function () {
    MASTER.showToast("שמירת העדפות תהיה זמינה בקרוב", "warning");
  });

  $("#logoutBtn").on("click", function () {
    Swal.fire({
      title: "להתנתק מהחשבון?",
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "התנתקות",
      cancelButtonText: "חזרה",
      confirmButtonColor: "#b91c1c",
    }).then(function (result) {
      if (result.isConfirmed) {
        sessionStorage.removeItem("current-user");
        window.location.replace("login.html");
      }
    });
  });

  $("#navHome").on("click", function () {
    window.location.href = "main-menu.html";
  });
  $("#navFindRide").on("click", function () {
    window.location.href = "find-ride.html";
  });
  $("#navMyRides").on("click", function () {
    window.location.href = "my-rides.html";
  });
  $("#navPreferences").on("click", function () {
    MASTER.showToast("את/ה כבר במסך ההעדפות", "warning");
  });
});
