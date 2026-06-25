/* Sample/placeholder data — no ride-list API is wired up yet. */
var SAMPLE_RIDES = [
  {
    time: "09:30",
    origin: "רחוב הרצל 12, תל אביב",
    destination: "בית חולים איכילוב",
    patient: "דוד כהן",
    status: "מאושרת",
  },
  {
    time: "13:00",
    origin: "רחוב ויצמן 5, רמת גן",
    destination: "מרכז רפואי שיבא, תל השומר",
    patient: "רחל לוי",
    status: "מאושרת",
  },
];

function getTimeGreeting() {
  var h = new Date().getHours();
  if (h >= 5 && h < 12) return "בוקר טוב";
  if (h >= 12 && h < 17) return "צהריים טובים";
  if (h >= 17 && h < 21) return "ערב טוב";
  return "לילה טוב";
}

function renderGreeting(user) {
  var name = (user && user.DisplayName) || "מתנדב/ת";
  var timeGreet = getTimeGreeting();
  $("#greeting").text(timeGreet + ", " + name + "!");
}

function renderStats(user) {
  var total = (user && user.NoOfDocumentedRides) || 0;
  var monthly = (user && user.NoOfRides) || 0;

  $("#statMonth").text(monthly);
  $("#statTotal").text(total);
}

function renderRides(rides) {
  var $list = $("#rideList");
  $list.empty();

  if (!rides || rides.length === 0) {
    $list.append(
      '<div class="empty-state">' +
        '<span class="empty-state__icon" aria-hidden="true">📋</span>' +
        "<p>אין נסיעות מתוכננות להיום.</p>" +
        '<button class="btn-primary" id="emptyFindRide">חיפוש נסיעה</button>' +
        "</div>",
    );
    return;
  }

  rides.forEach(function (ride) {
    var $card = $(
      '<article class="ride-card">' +
        '<div class="ride-card__top">' +
        '<span class="ride-card__time"></span>' +
        '<span class="ride-card__status"><span aria-hidden="true">✓</span><span class="status-text"></span></span>' +
        "</div>" +
        '<div class="ride-card__route"></div>' +
        '<div class="ride-card__patient"></div>' +
        "</article>",
    );
    $card.find(".ride-card__time").text(ride.time);
    $card.find(".status-text").text(ride.status);
    $card
      .find(".ride-card__route")
      .text(ride.origin + " ← " + ride.destination);
    $card.find(".ride-card__patient").text("מטופל/ת: " + ride.patient);
    $list.append($card);
  });
}

$(function () {
  MASTER.renderHeader("#appHeader", { title: "שלום!", subtitle: "מוכנים לעזור היום?" });

  var user = MASTER.getCurrentUser();
  if (!user) {
    window.location.replace("login.html");
    return;
  }
  renderGreeting(user);
  renderStats(user);
  renderRides(SAMPLE_RIDES);

  MASTER.IsProductionDatabase(function (isProd) {
    if (!isProd) {
      $(".header").css("background-color", "#f39c12");
    }
  });

  $("#actionFindRide, #navFindRide").on("click", function () {
    window.location.href = "find-ride.html";
  });
  $("#actionMyRides, #navMyRides").on("click", function () {
    window.location.href = "my-rides.html";
  });
  $("#actionPreferences").on("click", function () {
    window.location.href = "preferences.html";
  });
  $("#actionStats").on("click", function () {
    window.location.href = "stats.html";
  });
  $("#navSettings").on("click", function () {
    window.location.href = "settings.html";
  });
  $(document).on("click", "#emptyFindRide", function () {
    window.location.href = "find-ride.html";
  });
  $("#navHome").on("click", function () {
    MASTER.showToast("את/ה כבר במסך הבית", "warning");
  });
});
