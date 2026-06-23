const MASTER = {
  getBaseUrl: () => {
    const env = MASTER.environmentDetected();
    if (env === "local")
      return "http://localhost:59819/Road%20to%20Recovery/pages/WebService.asmx/";
    if (env === "test")
      return "https://roadtorecovery.org.il/Gilad_test/Road%20to%20Recovery/pages/WebService.asmx/";
    return "https://roadtorecovery.org.il/prod/Road%20to%20Recovery/pages/WebService.asmx/";
  },
  environmentDetected: () => {
    if (location.href.includes("localhost") || location.protocol == "file:") {
      return "local";
    }
    if (location.href.includes("Gilad_test")) {
      return "test";
    }
    if (location.href.includes("prod")) {
      return "prod";
    }
    return "unknown";
  },
  ajax: (endpoint, data, onSuccess, onError) => {
    $.ajax({
      url: MASTER.getBaseUrl() + endpoint,
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify(data),
      success: onSuccess,
      error: onError,
    });
  },
  getCurrentUser: () => {
    try {
      const raw = sessionStorage.getItem("current-user");
      return raw ? JSON.parse(raw) : null;
    } catch (e) {
      return null;
    }
  },
  showToast: (message, type) => {
    type = type || "success";
    const icons = { success: "✓", error: "✕", warning: "⚠" };
    const container = document.getElementById("toast-container");
    if (!container) return;

    container.innerHTML = "";

    const toast = document.createElement("div");
    toast.className = "toast toast--" + type;
    toast.setAttribute("role", "alert");
    toast.innerHTML =
      '<span class="toast-icon" aria-hidden="true">' +
      icons[type] +
      "</span>" +
      "<span>" +
      message +
      "</span>";

    container.appendChild(toast);

    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        toast.classList.add("toast--visible");
      });
    });

    setTimeout(function () {
      toast.classList.remove("toast--visible");
      setTimeout(function () {
        toast.remove();
      }, 300);
    }, 2500);
  },
};
