var allRides = [];
var currentFilter = "all";
var loadFailed = false;
var currentUser = null;

var searchQuery = "";
var selectedDateKey = "all";
var prefsOnly = false;
var cachedPrefs = null;

/* ---- Filtering / sorting ---- */
function dateKeyOf(ride) {
  var d = MASTER.parseDate(ride.PickupTime);
  if (!d) return null;
  return (
    d.getFullYear() +
    "-" +
    String(d.getMonth() + 1).padStart(2, "0") +
    "-" +
    String(d.getDate()).padStart(2, "0")
  );
}

function rideMatchesQuery(ride, query) {
  var haystack = [
    patientDisplayName(ride),
    ride.Origin,
    ride.Destination,
    ride.Area,
  ]
    .join(" ")
    .toLowerCase();
  return haystack.indexOf(query) !== -1;
}

/* Rides matching every filter except the morning/afternoon tab — shared by
   the list itself and by the tab counts, so counts stay in sync with search/date/prefs. */
function baseFilteredRides() {
  var rides = allRides.slice();

  if (selectedDateKey !== "all") {
    rides = rides.filter(function (r) {
      return dateKeyOf(r) === selectedDateKey;
    });
  }

  if (prefsOnly) {
    rides = rides.filter(function (r) {
      return MASTER.rideMatchesPreferences(r, cachedPrefs);
    });
  }

  var query = searchQuery.trim().toLowerCase();
  if (query) {
    rides = rides.filter(function (r) {
      return rideMatchesQuery(r, query);
    });
  }

  return rides;
}

function filterRides(filter) {
  var rides = baseFilteredRides();

  if (filter === "morning") {
    rides = rides.filter(function (r) {
      return !r.IsAfterNoon;
    });
  } else if (filter === "afternoon") {
    rides = rides.filter(function (r) {
      return !!r.IsAfterNoon;
    });
  }

  rides.sort(function (a, b) {
    return (
      (MASTER.parseDate(a.PickupTime) || 0) -
      (MASTER.parseDate(b.PickupTime) || 0)
    );
  });

  return rides;
}

/* ---- Tab counts ---- */
function updateSummary() {
  var rides = baseFilteredRides();
  var morning = rides.filter(function (r) {
    return !r.IsAfterNoon;
  }).length;
  var afternoon = rides.filter(function (r) {
    return !!r.IsAfterNoon;
  }).length;

  $("#countAll").text(rides.length || "");
  $("#countMorning").text(morning || "");
  $("#countAfternoon").text(afternoon || "");
}

/* ---- Date chips ---- */
function shortDateLabel(d) {
  return MASTER.HEBREW_DAY_ABBR[d.getDay()] + " " + d.getDate() + "." + (d.getMonth() + 1);
}

function renderDateChips() {
  var $row = $("#dateChipRow").empty();
  var seen = {};
  var dates = [];

  allRides.forEach(function (r) {
    var d = MASTER.parseDate(r.PickupTime);
    var key = dateKeyOf(r);
    if (!d || !key || seen[key]) return;
    seen[key] = true;
    dates.push({ key: key, date: d });
  });

  dates.sort(function (a, b) {
    return a.date - b.date;
  });

  $row.append(
    $('<button type="button" class="date-chip is-selected" aria-pressed="true" data-key="all">הכל</button>'),
  );

  dates.forEach(function (entry) {
    $row.append(
      $('<button type="button" class="date-chip" aria-pressed="false"></button>')
        .attr("data-key", entry.key)
        .text(shortDateLabel(entry.date)),
    );
  });
}

/* ---- Patient display helpers ---- */
function patientDisplayName(ride) {
  return ride.IsAnonymous ? "מטופל/ת אנונימי/ת" : ride.PatientName || "מטופל/ת";
}

function patientPhone(ride) {
  return ride.IsAnonymous ? "" : ride.PatientCellPhone || "";
}

/* ---- Card builder ---- */
function buildRideCard(ride) {
  var $card = $(
    '<article class="trip-card" tabindex="0" role="button">' +
      '<div class="trip-card__head">' +
      '<span class="trip-card__when">' +
      '<span aria-hidden="true">📅</span><span class="trip-card__date"></span>' +
      '<span class="trip-card__when-time"><span aria-hidden="true">🕐</span><span class="trip-card__time-text"></span><span class="trip-card__afternoon-label" hidden></span></span>' +
      "</span>" +
      '<span class="status-badge status-badge--sm status-badge--default"><span class="status-badge__dot" aria-hidden="true"></span><span class="status-badge__text"></span></span>' +
      "</div>" +
      '<h3 class="trip-card__route"></h3>' +
      '<div class="trip-card__row">' +
      '<span class="trip-card__patient"><span aria-hidden="true">👤</span><span class="trip-card__patient-text"></span></span>' +
      '<span class="trip-card__area"><span aria-hidden="true">📍</span><span class="trip-card__area-text"></span></span>' +
      "</div>" +
      '<div class="equipment-chips trip-card__equipment"></div>' +
      '<button class="btn-register" type="button">הירשם לנסיעה</button>' +
      "</article>",
  );

  $card.data("ride", ride);

  var timeInfo = MASTER.getRideTimeDisplay(ride.PickupTime, ride.IsAfterNoon);
  $card.find(".trip-card__date").text(MASTER.formatHebrewDate(ride.PickupTime));
  $card.find(".trip-card__time-text").text(timeInfo.time);
  $card
    .find(".trip-card__afternoon-label")
    .text(timeInfo.showLabel ? timeInfo.label : "")
    .attr("hidden", timeInfo.showLabel ? null : "hidden");

  $card.find(".status-badge__text").text(ride.Status || "פתוחה");

  $card
    .find(".trip-card__route")
    .text((ride.Origin || "—") + " ← " + (ride.Destination || "—"));

  $card.find(".trip-card__patient-text").text(patientDisplayName(ride));
  $card.find(".trip-card__area-text").text(ride.Area || "—");

  var $chips = $card.find(".trip-card__equipment");
  (ride.Equipments || []).forEach(function (eq) {
    $chips.append(
      $('<span class="equipment-chip"></span>')
        .append('<span aria-hidden="true">🩺</span>')
        .append($("<span></span>").text(eq.Name)),
    );
  });

  return $card;
}

/* ---- Details modal ---- */
function openRideModal(ride) {
  var modalTimeInfo = MASTER.getRideTimeDisplay(ride.PickupTime, ride.IsAfterNoon);
  $("#modalTime").text(modalTimeInfo.time);
  $("#modalAfternoonLabel")
    .text(modalTimeInfo.showLabel ? modalTimeInfo.label : "")
    .attr("hidden", modalTimeInfo.showLabel ? null : "hidden");
  $("#modalDate").text(MASTER.formatHebrewDate(ride.PickupTime));
  $("#modalOrigin").text(ride.Origin || "—");
  $("#modalDest").text(ride.Destination || "—");
  $("#modalPatient").text(patientDisplayName(ride));
  $("#modalArea").text(ride.Area || "—");

  if (ride.AmountOfEscorts) {
    $("#modalEscorts").text(ride.AmountOfEscorts + " מלווה");
    $("#modalEscortsRow, #modalEscortsDivider").removeAttr("hidden");
  } else {
    $("#modalEscortsRow, #modalEscortsDivider").attr("hidden", "hidden");
  }

  var $chips = $("#modalEquipmentChips").empty();
  if (ride.Equipments && ride.Equipments.length) {
    ride.Equipments.forEach(function (eq) {
      $chips.append(
        $('<span class="equipment-chip"></span>')
          .append('<span aria-hidden="true">🩺</span>')
          .append($("<span></span>").text(eq.Name)),
      );
    });
    $("#modalEquipmentRow, #modalEquipmentDivider").removeAttr("hidden");
  } else {
    $("#modalEquipmentRow, #modalEquipmentDivider").attr("hidden", "hidden");
  }

  var phone = patientPhone(ride);
  if (phone) {
    $("#modalPhone").text(phone);
    $("#modalCallBtn").attr("href", "tel:" + phone);
    $("#modalPhoneRow, #modalPhoneDivider").removeAttr("hidden");
  } else {
    $("#modalPhoneRow, #modalPhoneDivider").attr("hidden", "hidden");
  }

  $("#rideModalOverlay").data("ride", ride).removeAttr("hidden");
}

function closeRideModal() {
  $("#rideModalOverlay").attr("hidden", "hidden");
}

function confirmRegisterRide(ride) {
  Swal.fire({
    title: "להירשם לנסיעה זו?",
    text: ride.Origin + " ← " + ride.Destination,
    icon: "question",
    showCancelButton: true,
    confirmButtonText: "הירשם",
    cancelButtonText: "חזרה",
    confirmButtonColor: "#005f85",
  }).then(function (result) {
    if (!result.isConfirmed) return;

    MASTER.ajax(
      "AssignUpdateDriverToUnityRide",
      {
        UnityRideId: ride.RidePatNum,
        DriverId: currentUser.Id,
        isDelete: false,
        userName: currentUser.DisplayName,
      },
      function (wrapper) {
        var updatedRide = MASTER.parseResponse(wrapper);
        if (updatedRide && updatedRide.RidePatNum === -5) {
          MASTER.showToast("הנסיעה הזו שובצה לנהג אחר. נסו לרענן.", "error");
          closeRideModal();
          fetchRides();
          return;
        }

        closeRideModal();
        allRides = allRides.filter(function (r) {
          return r.RidePatNum !== ride.RidePatNum;
        });
        updateSummary();
        renderRides();
        MASTER.showToast("נרשמת בהצלחה לנסיעה", "success");
        MASTER.refreshCurrentUser(function (user) {
          currentUser = user;
        });
      },
      function (xhr, status, error) {
        MASTER.devLog("Error in AssignUpdateDriverToUnityRide: " + error, "error");
        MASTER.showToast("שגיאה בהרשמה לנסיעה. נסו שנית.", "error");
      },
    );
  });
}

/* ---- Empty / error states ---- */
function hasActiveExtraFilters() {
  return !!searchQuery.trim() || selectedDateKey !== "all" || prefsOnly;
}

function buildEmptyState(filter) {
  if (hasActiveExtraFilters()) {
    return $(
      '<div class="empty-state">' +
        '<span class="empty-state__icon" aria-hidden="true">🔍</span>' +
        "<p>לא נמצאו נסיעות התואמות את הסינון שנבחר.</p>" +
        '<button class="btn-secondary empty-state__retry" id="clearFilters">איפוס סינון</button>' +
        "</div>",
    );
  }

  var messages = {
    all: "אין כרגע נסיעות פתוחות להרשמה.",
    morning: "אין נסיעות פתוחות בבוקר כרגע.",
    afternoon: "אין נסיעות פתוחות באחה\"צ כרגע.",
  };
  return $(
    '<div class="empty-state">' +
      '<span class="empty-state__icon" aria-hidden="true">🔍</span>' +
      "<p>" +
      (messages[filter] || messages.all) +
      "</p>" +
      '<button class="btn-primary empty-state__retry" id="retryLoad">ניסיון נוסף</button>' +
      "</div>",
  );
}

function buildErrorState() {
  return $(
    '<div class="empty-state">' +
      '<span class="empty-state__icon" aria-hidden="true">⚠️</span>' +
      "<p>שגיאה בטעינת הנסיעות. בדקו את החיבור ונסו שנית.</p>" +
      '<button class="btn-primary empty-state__retry" id="retryLoad">ניסיון נוסף</button>' +
      "</div>",
  );
}

function buildSkeleton() {
  var $wrap = $('<div class="trip-list"></div>');
  for (var i = 0; i < 3; i++) {
    $wrap.append(
      $(
        '<div class="skeleton-card">' +
          '<div class="skeleton-line skeleton-line--tall"></div>' +
          '<div class="skeleton-line"></div>' +
          '<div class="skeleton-line skeleton-line--short"></div>' +
          "</div>",
      ),
    );
  }
  return $wrap.children();
}

/* ---- Render ---- */
function renderRides() {
  var $list = $("#rideList");
  $list.empty();

  if (loadFailed) {
    $list.append(buildErrorState());
    return;
  }

  var rides = filterRides(currentFilter);

  if (rides.length === 0) {
    $list.append(buildEmptyState(currentFilter));
    return;
  }

  rides.forEach(function (ride) {
    $list.append(buildRideCard(ride));
  });
}

function renderLoading() {
  var $list = $("#rideList");
  $list.empty();
  $list.append(buildSkeleton());
}

function setActiveTab(filter) {
  currentFilter = filter;
  $(".filter-tab").each(function () {
    var isActive = $(this).data("filter") === filter;
    $(this).toggleClass("is-active", isActive);
    $(this).attr("aria-selected", isActive ? "true" : "false");
  });
}

/* ---- Data ---- */
function fetchRides() {
  loadFailed = false;
  renderLoading();

  selectedDateKey = "all";

  MASTER.ajax(
    "GetAllUnityRidesMobile",
    {},
    function (wrapper) {
      allRides = MASTER.parseResponse(wrapper) || [];
      console.log("Fetched rides:", allRides);
      renderDateChips();
      updateSummary();
      renderRides();
    },
    function (xhr, status, error) {
      MASTER.devLog("Error loading open rides: " + error, "error");
      loadFailed = true;
      renderRides();
      MASTER.showToast("שגיאה בטעינת הנסיעות. נסו שנית.", "error");
    },
  );
}

$(function () {
  MASTER.renderHeader("#appHeader", { title: "חיפוש נסיעה" });

  currentUser = MASTER.getCurrentUser();
  if (!currentUser) {
    window.location.replace("login.html");
    return;
  }

  cachedPrefs = MASTER.getCachedPreferences();
  fetchRides();

  MASTER.IsProductionDatabase(function (isProd) {
    if (!isProd) {
      $(".header").css("background-color", "#f39c12");
    }
  });

  $(".filter-tab").on("click", function () {
    setActiveTab($(this).data("filter"));
    renderRides();
  });

  $("#searchInput").on("input", function () {
    searchQuery = $(this).val();
    $("#searchClearBtn").attr("hidden", searchQuery ? null : "hidden");
    updateSummary();
    renderRides();
  });

  $("#searchClearBtn").on("click", function () {
    searchQuery = "";
    $("#searchInput").val("");
    $(this).attr("hidden", "hidden");
    updateSummary();
    renderRides();
  });

  $(document).on("click", "#dateChipRow .date-chip", function () {
    selectedDateKey = $(this).data("key");
    $("#dateChipRow .date-chip")
      .removeClass("is-selected")
      .attr("aria-pressed", "false");
    $(this).addClass("is-selected").attr("aria-pressed", "true");
    updateSummary();
    renderRides();
  });

  $("#prefOnlyToggle").on("click", function () {
    var hasPrefs =
      cachedPrefs &&
      ((cachedPrefs.PreferredDays || []).length ||
        (cachedPrefs.PreferredAreas || []).length);
    if (!prefsOnly && !hasPrefs) {
      MASTER.showToast(
        "טרם הוגדרו העדפות. ניתן להגדיר בעמוד ההעדפות שלי",
        "warning",
      );
    }
    prefsOnly = !prefsOnly;
    $(this)
      .toggleClass("is-active", prefsOnly)
      .attr("aria-pressed", prefsOnly ? "true" : "false");
    updateSummary();
    renderRides();
  });

  $("#navHome").on("click", function () {
    window.location.href = "main-menu.html";
  });
  $("#navFindRide").on("click", function () {
    MASTER.showToast("את/ה כבר במסך חיפוש נסיעה", "warning");
  });
  $("#navMyRides").on("click", function () {
    window.location.href = "my-rides.html";
  });
  $("#navPreferences").on("click", function () {
    window.location.href = "preferences.html";
  });

  $(document).on("click", "#retryLoad", function () {
    fetchRides();
  });

  $(document).on("click", "#clearFilters", function () {
    searchQuery = "";
    selectedDateKey = "all";
    prefsOnly = false;
    $("#searchInput").val("");
    $("#searchClearBtn").attr("hidden", "hidden");
    $("#prefOnlyToggle").removeClass("is-active").attr("aria-pressed", "false");
    $("#dateChipRow .date-chip")
      .removeClass("is-selected")
      .attr("aria-pressed", "false");
    $('#dateChipRow .date-chip[data-key="all"]')
      .addClass("is-selected")
      .attr("aria-pressed", "true");
    updateSummary();
    renderRides();
  });

  $(document).on("click", ".trip-card .btn-register", function (e) {
    e.stopImmediatePropagation();
    confirmRegisterRide($(this).closest(".trip-card").data("ride"));
  });
  $(document).on("click keypress", ".trip-card", function (e) {
    if (e.type === "keypress" && e.key !== "Enter") return;
    openRideModal($(this).data("ride"));
  });

  $("#modalCloseBtn").on("click", closeRideModal);
  $("#modalRegisterBtn").on("click", function () {
    confirmRegisterRide($("#rideModalOverlay").data("ride"));
  });
  $("#rideModalOverlay").on("click", function (e) {
    if (e.target === this) closeRideModal();
  });
});
