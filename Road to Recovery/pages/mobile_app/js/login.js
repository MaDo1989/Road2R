const form = document.getElementById("loginForm");
const phoneInput = document.getElementById("phone");
const loginBtn = document.getElementById("loginBtn");

/* ---- Form submit ---- */
form.addEventListener("submit", function (e) {
  e.preventDefault();

  const phone = phoneInput.value.trim().replace(/[-\s()]/g, "");

  if (!phone) {
    showFieldError("phone", "יש להזין מספר טלפון");
    phoneInput.focus();
    return;
  }

  if (!/^0[5][0-9]{8}$/.test(phone)) {
    showFieldError("phone", "יש להזין מספר טלפון נייד תקין (למשל: 0501234567)");
    phoneInput.focus();
    return;
  }

  clearFieldError("phone");
  setLoading(true);

  loginRequest(phone);
});

/* ---- API call ---- */
function loginRequest(phone) {
  MASTER.ajax(
    "LoginMobileApp",
    { userPhone: phone },
    function (wrapper) {
      const data = JSON.parse(wrapper.d);
      console.log("Login response:", data);

      if (data.ResponseStatus === 200) {
        sessionStorage.setItem("current-user", JSON.stringify(data));
        if (data.Id) MASTER.fetchAndCachePreferences(data.Id);
        Swal.fire({
          icon: "success",
          title: "ברוך הבא!",
          text: "שלום " + data.DisplayName + "! התחברת בהצלחה.",
          confirmButtonText: "המשך",
          confirmButtonColor: "#005f85",
          timer: 2500,
          timerProgressBar: true,
        }).then(function () {
          window.location.href = "main-menu.html";
        });
      } else {
        const msg = getErrorMessage(data.ResponseStatus, data.Message);
        showToast(msg, "error");
      }
      setLoading(false);
    },
    function (xhr, status, error) {
      console.error("Login error:", error);
      showToast("שגיאה בחיבור לשרת. נסו שנית.", "error");
      setLoading(false);
    },
  );
}

/* ---- Error messages ---- */
function getErrorMessage(status, serverMsg) {
  switch (status) {
    case 400:
      return "יש להזין מספר טלפון";
    case 404:
      return "המספר אינו רשום במערכת. אנא פנו למנהל.";
    case 500:
      return "שגיאת שרת. נסו שנית מאוחר יותר.";
    default:
      return serverMsg || "שגיאה לא ידועה";
  }
}

/* ---- Loading state ---- */
function setLoading(loading) {
  loginBtn.disabled = loading;
  const btnText = loginBtn.querySelector(".btn-text");
  const btnSpinner = loginBtn.querySelector(".btn-spinner");

  if (loading) {
    btnText.textContent = "מתחבר...";
    btnSpinner.style.display = "block";
  } else {
    btnText.textContent = "כניסה";
    btnSpinner.style.display = "none";
  }
}

/* ---- Field error helpers ---- */
function showFieldError(fieldId, message) {
  clearFieldError(fieldId);

  const input = document.getElementById(fieldId);
  input.classList.add("input--error");
  input.setAttribute("aria-invalid", "true");

  const errorEl = document.createElement("span");
  errorEl.className = "field-error field-error--center";
  errorEl.id = fieldId + "-error";
  errorEl.setAttribute("role", "alert");
  errorEl.innerHTML = '<span aria-hidden="true">✕</span> ' + message;
  input.parentElement.appendChild(errorEl);

  input.setAttribute("aria-describedby", fieldId + "-error");
}

function clearFieldError(fieldId) {
  const input = document.getElementById(fieldId);
  input.classList.remove("input--error");
  input.removeAttribute("aria-invalid");

  const existing = document.getElementById(fieldId + "-error");
  if (existing) existing.remove();

  input.setAttribute("aria-describedby", fieldId + "-hint");
}

/* ---- Toast ---- */
function showToast(message, type) {
  type = type || "success";
  var icons = { success: "✓", error: "✕", warning: "⚠" };
  var container = document.getElementById("toast-container");

  container.innerHTML = "";

  var toast = document.createElement("div");
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
}

console.log("running on ", MASTER.environmentDetected());
