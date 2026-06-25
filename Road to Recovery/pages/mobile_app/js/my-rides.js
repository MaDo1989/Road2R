var allTrips = [];
var currentFilter = "upcoming";
var loadFailed = false;

/* ---- Status → badge class ---- */
function statusBadgeClass(status) {
  var s = (status || "").trim();
  if (s.indexOf("בוטל") !== -1) return "status-badge--error";
  if (s.indexOf("בוצע") !== -1 || s.indexOf("הסתיים") !== -1)
    return "status-badge--success";
  if (s.indexOf("ממתינ") !== -1 || s.indexOf("תלוי") !== -1)
    return "status-badge--warning";
  return "status-badge--default";
}

/* ---- Filtering / sorting ---- */
function filterTrips(filter) {
  var trips = allTrips.slice();

  if (filter === "upcoming") {
    trips = trips.filter(function (t) {
      return !t.IsInThePast;
    });
    trips.sort(function (a, b) {
      return new Date(a.PickupTime) - new Date(b.PickupTime);
    });
  } else if (filter === "past") {
    trips = trips.filter(function (t) {
      return !!t.IsInThePast;
    });
    trips.sort(function (a, b) {
      return new Date(b.PickupTime) - new Date(a.PickupTime);
    });
  } else {
    trips.sort(function (a, b) {
      return new Date(b.PickupTime) - new Date(a.PickupTime);
    });
  }

  return trips;
}

/* ---- Tab counts ---- */
function updateSummary() {
  var upcoming = allTrips.filter(function (t) {
    return !t.IsInThePast;
  }).length;
  var past = allTrips.filter(function (t) {
    return !!t.IsInThePast;
  }).length;

  $("#countUpcoming").text(upcoming || "");
  $("#countPast").text(past || "");
  $("#countAll").text(allTrips.length || "");
}

/* ---- Card builder ---- */
function buildTripCard(trip) {
  var isPast = !!trip.IsInThePast;
  var $card = $(
    '<article class="trip-card' +
      (isPast ? " trip-card--past" : "") +
      '" tabindex="0" role="button">' +
      '<div class="trip-card__head">' +
      '<span class="trip-card__when">' +
      '<span aria-hidden="true">📅</span><span class="trip-card__date"></span>' +
      '<span class="trip-card__when-time"><span aria-hidden="true">🕐</span><span class="trip-card__time-text"></span></span>' +
      "</span>" +
      '<span class="status-badge status-badge--sm"><span class="status-badge__dot" aria-hidden="true"></span><span class="status-badge__text"></span></span>' +
      "</div>" +
      '<h3 class="trip-card__route"></h3>' +
      '<div class="trip-card__row">' +
      '<span class="trip-card__patient"><span aria-hidden="true">👤</span><span class="trip-card__patient-text"></span></span>' +
      "</div>" +
      (isPast
        ? ""
        : '<button class="btn-cancel" type="button">בטל רישום</button>') +
      "</article>",
  );

  $card.data("trip", trip);

  $card.find(".trip-card__date").text(MASTER.formatHebrewDate(trip.PickupTime));
  $card.find(".trip-card__time-text").text(MASTER.formatTime(trip.PickupTime));

  var statusText = trip.Status || (isPast ? "בוצעה" : "מתוכננת");
  var badgeClass = statusBadgeClass(statusText);
  $card.find(".status-badge").addClass(badgeClass);
  $card.find(".status-badge__text").text(statusText);

  $card
    .find(".trip-card__route")
    .text((trip.Origin || "—") + " ← " + (trip.Destination || "—"));

  $card.find(".trip-card__patient-text").text(trip.PatientName || "מטופל/ת");

  return $card;
}

/* ---- Details modal ---- */
function openTripModal(trip) {
  $("#modalTime").text(MASTER.formatTime(trip.PickupTime));
  $("#modalDate").text(MASTER.formatHebrewDate(trip.PickupTime));
  $("#modalOrigin").text(trip.Origin || "—");
  $("#modalDest").text(trip.Destination || "—");
  $("#modalPatient").text(trip.PatientName || "מטופל/ת");
  $("#modalArea").text(trip.Area || "—");

  if (trip.AmountOfEscorts) {
    $("#modalEscorts").text(trip.AmountOfEscorts + " מלווה");
    $("#modalEscortsRow, #modalEscortsDivider").removeAttr("hidden");
  } else {
    $("#modalEscortsRow, #modalEscortsDivider").attr("hidden", "hidden");
  }

  if (trip.PatientCellPhone) {
    $("#modalPhone").text(trip.PatientCellPhone);
    $("#modalCallBtn").attr("href", "tel:" + trip.PatientCellPhone);
    $("#modalPhoneRow, #modalPhoneDivider").removeAttr("hidden");
  } else {
    $("#modalPhoneRow, #modalPhoneDivider").attr("hidden", "hidden");
  }

  $("#modalCancelBtn").toggle(!trip.IsInThePast);

  $("#tripModalOverlay").data("trip", trip).removeAttr("hidden");
}

function closeTripModal() {
  $("#tripModalOverlay").attr("hidden", "hidden");
}

function confirmCancelTrip(trip) {
  Swal.fire({
    title: "בטל רישום לנסיעה זו?",
    text: trip.Origin + " ← " + trip.Destination,
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "בטל רישום",
    cancelButtonText: "חזרה",
    confirmButtonColor: "#b91c1c",
  }).then(function (result) {
    if (result.isConfirmed) {
      closeTripModal();
      MASTER.showToast("הרישום בוטל", "success");
    }
  });
}

/* ---- Empty / error states ---- */
function buildEmptyState(filter) {
  var messages = {
    upcoming: "אין לך נסיעות קרובות מתוכננות.",
    past: "אין נסיעות עבר להצגה.",
    all: "עדיין לא שובצת לנסיעות.",
  };
  return $(
    '<div class="empty-state">' +
      '<span class="empty-state__icon" aria-hidden="true">📋</span>' +
      "<p>" +
      (messages[filter] || messages.all) +
      "</p>" +
      '<button class="btn-primary" id="emptyFindRide">חיפוש נסיעה</button>' +
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
function renderTrips() {
  var $list = $("#tripList");
  $list.empty();

  if (loadFailed) {
    $list.append(buildErrorState());
    return;
  }

  var trips = filterTrips(currentFilter);

  if (trips.length === 0) {
    $list.append(buildEmptyState(currentFilter));
    return;
  }

  trips.forEach(function (trip) {
    $list.append(buildTripCard(trip));
  });
}

function renderLoading() {
  var $list = $("#tripList");
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
function fetchTrips(volunteerId) {
  loadFailed = false;
  renderLoading();

  MASTER.ajax(
    "GetMyUnityRidesMobile",
    { volunteerId: volunteerId },
    function (wrapper) {
      allTrips = MASTER.parseResponse(wrapper) || [];
      updateSummary();
      renderTrips();
    },
    function (xhr, status, error) {
      MASTER.devLog("Error loading rides: " + error, "error");
      loadFailed = true;
      renderTrips();
      MASTER.showToast("שגיאה בטעינת הנסיעות. נסו שנית.", "error");
    },
  );
}

$(function () {
  MASTER.renderHeader("#appHeader", { title: "הנסיעות שלי" });

  var user = MASTER.getCurrentUser();
  if (!user) {
    window.location.replace("login.html");
    return;
  }

  fetchTrips(user.Id);

  MASTER.IsProductionDatabase(function (isProd) {
    if (!isProd) {
      $(".header").css("background-color", "#f39c12");
    }
  });

  $(".filter-tab").on("click", function () {
    setActiveTab($(this).data("filter"));
    renderTrips();
  });

  $("#navHome").on("click", function () {
    window.location.href = "main-menu.html";
  });
  $("#navFindRide").on("click", function () {
    window.location.href = "find-ride.html";
  });
  $("#navMyRides").on("click", function () {
    MASTER.showToast("את/ה כבר במסך הנסיעות שלי", "warning");
  });
  $("#navSettings").on("click", function () {
    window.location.href = "settings.html";
  });

  $(document).on("click", "#emptyFindRide", function () {
    window.location.href = "find-ride.html";
  });
  $(document).on("click", "#retryLoad", function () {
    fetchTrips(user.Id);
  });

  $(document).on("click", ".trip-card .btn-cancel", function (e) {
    e.stopImmediatePropagation();
    confirmCancelTrip($(this).closest(".trip-card").data("trip"));
  });
  $(document).on("click keypress", ".trip-card", function (e) {
    if (e.type === "keypress" && e.key !== "Enter") return;
    openTripModal($(this).data("trip"));
  });

  $("#modalCloseBtn").on("click", closeTripModal);
  $("#modalCancelBtn").on("click", function () {
    confirmCancelTrip($("#tripModalOverlay").data("trip"));
  });
  $("#tripModalOverlay").on("click", function (e) {
    if (e.target === this) closeTripModal();
  });
});
