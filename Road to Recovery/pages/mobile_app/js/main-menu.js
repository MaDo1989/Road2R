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

$(function () {
  MASTER.renderHeader("#appHeader", {
    title: "שלום!",
    subtitle: "מוכנים לעזור היום?",
  });

  var user = MASTER.getCurrentUser();
  if (!user) {
    window.location.replace("login.html");
    return;
  }
  renderGreeting(user);
  renderStats(user);

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
  $("#actionPreferences, #navPreferences").on("click", function () {
    window.location.href = "preferences.html";
  });
  $("#actionStats").on("click", function () {
    window.location.href = "stats.html";
  });
  $("#navHome").on("click", function () {
    MASTER.showToast("את/ה כבר במסך הבית", "warning");
  });
});
