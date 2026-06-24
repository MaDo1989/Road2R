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

/* ---- Header summary + tab counts ---- */
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

  if (allTrips.length === 0) {
    $("#ridesSummary").text("אין נסיעות להצגה");
  } else {
    $("#ridesSummary").text(
      upcoming + " נסיעות קרובות מתוך " + allTrips.length + " בסך הכל",
    );
  }
}

/* ---- Card builder ---- */
function buildTripCard(trip) {
  var isPast = !!trip.IsInThePast;
  var $card = $(
    '<article class="trip-card' +
      (isPast ? " trip-card--past" : "") +
      '">' +
      '<div class="trip-card__top">' +
      '<div class="trip-card__date">' +
      '<span class="trip-card__day"></span>' +
      "</div>" +
      '<span class="trip-card__time"></span>' +
      "</div>" +
      '<div class="trip-card__badges">' +
      '<span class="status-badge"><span aria-hidden="true"></span><span class="status-badge__text"></span></span>' +
      '<span class="shift-badge"></span>' +
      "</div>" +
      '<div class="trip-card__divider"></div>' +
      '<div class="trip-card__route">' +
      '<div class="route-point route-point--origin"><span class="route-point__icon" aria-hidden="true">📍</span><span class="route-point__text"></span></div>' +
      '<div class="route-arrow" aria-hidden="true">⬇</div>' +
      '<div class="route-point route-point--dest"><span class="route-point__icon" aria-hidden="true">🏥</span><span class="route-point__text"></span></div>' +
      "</div>" +
      '<div class="trip-card__meta">' +
      '<span class="meta-patient"><span aria-hidden="true">👤</span><span class="meta-patient__text"></span></span>' +
      '<span class="meta-area"><span aria-hidden="true">🌍</span><span class="meta-area__text"></span></span>' +
      "</div>" +
      "</article>",
  );

  $card.find(".trip-card__day").text(MASTER.formatHebrewDate(trip.PickupTime));
  $card.find(".trip-card__time").text(MASTER.formatTime(trip.PickupTime));

  var statusText = trip.Status || (isPast ? "בוצעה" : "מתוכננת");
  var badgeClass = statusBadgeClass(statusText);
  var statusIcons = {
    "status-badge--success": "✓",
    "status-badge--error": "✕",
    "status-badge--warning": "⚠",
    "status-badge--default": "•",
  };
  $card.find(".status-badge").addClass(badgeClass);
  $card.find(".status-badge span[aria-hidden]").text(statusIcons[badgeClass]);
  $card.find(".status-badge__text").text(statusText);

  $card
    .find(".shift-badge")
    .text(trip.IsAfterNoon ? "☀️ אחר הצהריים" : "🌅 בוקר");

  $card.find(".route-point--origin .route-point__text").text(trip.Origin || "—");
  $card
    .find(".route-point--dest .route-point__text")
    .text(trip.Destination || "—");

  $card.find(".meta-patient__text").text(trip.PatientName || "מטופל/ת");
  $card.find(".meta-area__text").text(trip.Area || "—");

  if (trip.AmountOfEscorts) {
    $card.find(".trip-card__meta").append(
      $('<span class="meta-escorts"></span>')
        .append('<span aria-hidden="true">🧑‍🤝‍🧑</span>')
        .append(
          $("<span></span>").text(trip.AmountOfEscorts + " מלווים"),
        ),
    );
  }

  if (trip.PatientCellPhone) {
    var $call = $('<a class="btn-call"></a>')
      .attr("href", "tel:" + trip.PatientCellPhone)
      .append('<span aria-hidden="true">📞</span>')
      .append($("<span></span>").text("התקשרות למטופל/ת"));
    $card.append($call);
  }

  return $card;
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
});
