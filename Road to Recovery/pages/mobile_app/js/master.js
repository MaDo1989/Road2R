const MASTER = {
  getVersion: () => "1.0.1",
  getBaseUrl: () => {
    const env = MASTER.environmentDetected();
    if (env === "local")
      return (
        location.protocol +
        "//" +
        location.host +
        "/Road%20to%20Recovery/pages/WebService.asmx/"
      );
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
  /* Re-runs LoginMobileApp for the logged-in user and refreshes sessionStorage —
   * used after ride assignment changes so stats like NoOfRides stay current. */
  refreshCurrentUser: (onDone) => {
    const user = MASTER.getCurrentUser();
    if (!user || !user.CellPhone) {
      if (typeof onDone === "function") onDone(user);
      return;
    }
    MASTER.ajax(
      "LoginMobileApp",
      { userPhone: user.CellPhone },
      (wrapper) => {
        const data = MASTER.parseResponse(wrapper);
        if (data && data.ResponseStatus === 200) {
          sessionStorage.setItem("current-user", JSON.stringify(data));
        }
        if (typeof onDone === "function") onDone(data || user);
      },
      (xhr, status, error) => {
        MASTER.devLog("Error refreshing current user: " + error, "error");
        if (typeof onDone === "function") onDone(user);
      },
    );
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
  devLog: (message, state, customColor) => {
    const states = {
      error: {
        label: "ERROR",
        bg: "#c0392b",
        color: "#fff",
        method: "error",
      },
      warning: {
        label: "WARNING",
        bg: "#e67e22",
        color: "#fff",
        method: "warn",
      },
      important: {
        label: "IMPORTANT",
        bg: "#2980b9",
        color: "#fff",
        method: "log",
      },
    };

    const cfg = Object.assign({}, states[state] || states["important"]);
    if (customColor) cfg.bg = customColor;
    const badge =
      `background:${cfg.bg};color:${cfg.color};` +
      `font-size:14px;font-weight:bold;padding:4px 10px;border-radius:4px 0 0 4px;`;
    const text =
      `background:#1a1a1a;color:${cfg.bg};` +
      `font-size:14px;font-weight:bold;padding:4px 10px;border-radius:0 4px 4px 0;`;

    console[cfg.method](`%c ${cfg.label} %c ${message} `, badge, text);
  },
};
const isProductionDatabase = (onResult) => {
  MASTER.ajax(
    "isProductionDatabase",
    {},
    function (response) {
      const isProd = response.d == true;
      if (isProd) {
        MASTER.devLog("Connected to PRODUCTION database.", "important");
      } else {
        MASTER.devLog(
          "Connected to NON-PRODUCTION database.",
          "important",
          "orange",
        );
      }
      if (typeof onResult === "function") onResult(isProd);
    },
    function (xhr, status, error) {
      MASTER.devLog("Error checking database environment: " + error, "error");
      if (typeof onResult === "function") onResult(false);
    },
  );
};
MASTER.IsProductionDatabase = isProductionDatabase;

/* Shared header component — logo + title (+ optional subtitle). */
const renderHeader = (selector, opts) => {
  opts = opts || {};
  const titleId = opts.titleId || "greeting";
  const subId = opts.subId || "greetingSub";
  const logoSrc = opts.logoSrc || "../assets/logo-no-bg-sm.png";
  const subHtml =
    opts.subtitle != null
      ? `<p class="header__sub" id="${subId}">${opts.subtitle}</p>`
      : "";

  $(selector)
    .addClass("header")
    .html(
      '<div class="header__top-row">' +
        '<div class="header__logo-wrap">' +
        `<img src="${logoSrc}" alt="Road to Recovery" class="header__logo" />` +
        "</div>" +
        '<div class="header__title-group">' +
        `<h1 class="header__greeting" id="${titleId}">${opts.title || ""}</h1>` +
        subHtml +
        "</div>" +
        `<span class="header__version">v${MASTER.getVersion()}</span>` +
        "</div>",
    );
};
MASTER.renderHeader = renderHeader;

const HEBREW_DAY_ABBR = ["א'", "ב'", "ג'", "ד'", "ה'", "ו'", "ש'"];

/* ASMX endpoints wrap the JSON payload as a serialised string in `.d` */
const parseResponse = (wrapper) => {
  try {
    const raw = wrapper && wrapper.d !== undefined ? wrapper.d : wrapper;
    return typeof raw === "string" ? JSON.parse(raw) : raw;
  } catch (e) {
    MASTER.devLog("Error parsing API response: " + e, "error");
    return null;
  }
};
MASTER.parseResponse = parseResponse;

/*
 * The API serialises dates as ASP.NET AJAX's "/Date(ms)/" format, where `ms`
 * is an absolute epoch timestamp (UTC). `new Date("/Date(ms)/")` is NOT
 * understood by modern JS engines and silently yields an Invalid Date — this
 * is what caused times/sorting to break once the app stopped talking to a
 * mocked/local payload that happened to use a plain ISO string. Because the
 * embedded value is an absolute epoch instant, parsing it correctly removes
 * any dependency on the server's timezone/location: the browser always
 * renders it in the viewer's local time, consistently across environments.
 */
const parseDate = (value) => {
  if (value === null || value === undefined || value === "") return null;
  if (value instanceof Date) return isNaN(value.getTime()) ? null : value;

  if (typeof value === "number") {
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

  if (typeof value === "string") {
    const aspNetMatch = value.match(/^\/Date\((-?\d+)\)\/$/);
    if (aspNetMatch) {
      const d = new Date(Number(aspNetMatch[1]));
      return isNaN(d.getTime()) ? null : d;
    }
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

  return null;
};
MASTER.parseDate = parseDate;

const formatTime = (dateStr) => {
  const d = parseDate(dateStr);
  if (!d) return "";
  const useUtc = MASTER.environmentDetected() !== "local";
  const h = (useUtc ? d.getUTCHours() : d.getHours())
    .toString()
    .padStart(2, "0");
  const m = (useUtc ? d.getUTCMinutes() : d.getMinutes())
    .toString()
    .padStart(2, "0");
  return h + ":" + m;
};
MASTER.formatTime = formatTime;

const AFTERNOON_LABEL = 'אחה"צ';

/*
 * Afternoon rides logged with the placeholder minute :14 carry no real
 * pickup time — show only the אחה"צ label for those. Afternoon rides with a
 * real time show both the time and a small אחה"צ label next to it.
 */
const getRideTimeDisplay = (pickupTime, isAfterNoon) => {
  const d = parseDate(pickupTime);
  const useUtc = MASTER.environmentDetected() !== "local";
  const isPlaceholderTime =
    !!isAfterNoon &&
    !!d &&
    (useUtc ? d.getUTCMinutes() : d.getMinutes()) === 14;

  return {
    time: isPlaceholderTime ? "" : formatTime(pickupTime),
    showLabel: !!isAfterNoon,
    label: AFTERNOON_LABEL,
  };
};
MASTER.getRideTimeDisplay = getRideTimeDisplay;

const PREFERENCES_STORAGE_KEY = "volunteer-preferences";

/* Cached { PreferredDays, PreferredAreas } from GetVolunteerPreferencesMobile — read by find-ride's "preferences only" filter. */
const getCachedPreferences = () => {
  try {
    const raw = sessionStorage.getItem(PREFERENCES_STORAGE_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch (e) {
    return null;
  }
};
MASTER.getCachedPreferences = getCachedPreferences;

const cachePreferences = (prefs) => {
  sessionStorage.setItem(
    PREFERENCES_STORAGE_KEY,
    JSON.stringify(prefs || { PreferredDays: [], PreferredAreas: [] }),
  );
};
MASTER.cachePreferences = cachePreferences;

/* Fetches a volunteer's preferences from the server and refreshes the cache — called once after login, and again whenever the preferences page reloads them. */
const fetchAndCachePreferences = (volunteerId, onDone) => {
  MASTER.ajax(
    "GetVolunteerPreferencesMobile",
    { volunteerId: volunteerId },
    (wrapper) => {
      const prefs = MASTER.parseResponse(wrapper) || {
        PreferredDays: [],
        PreferredAreas: [],
      };
      cachePreferences(prefs);
      if (typeof onDone === "function") onDone(prefs);
    },
    (xhr, status, error) => {
      MASTER.devLog("Error fetching volunteer preferences: " + error, "error");
      if (typeof onDone === "function") onDone(getCachedPreferences());
    },
  );
};
MASTER.fetchAndCachePreferences = fetchAndCachePreferences;

/*
 * Whether a ride matches the volunteer's saved preferences. A dimension
 * (days/shifts or areas) with no saved preference is treated as "any" and
 * doesn't constrain the match — preferences are additive, not exclusionary.
 * An unknown (never-fetched) preference set is treated the same as "no
 * preference saved" so the filter degrades to "show everything" rather than
 * hiding rides because of a missing cache entry.
 */
const rideMatchesPreferences = (ride, prefs) => {
  const days = (prefs && prefs.PreferredDays) || [];
  const areas = (prefs && prefs.PreferredAreas) || [];

  if (days.length) {
    const d = parseDate(ride.PickupTime);
    const dayName = d ? HEBREW_WEEKDAYS[d.getDay()] : null;
    const shiftMatches = days.some((entry) => {
      if (entry.PreferedDayDayInWeek !== dayName) return false;
      if (entry.Shift === "כל") return true;
      return entry.Shift === "אחהצ" ? !!ride.IsAfterNoon : !ride.IsAfterNoon;
    });
    if (!shiftMatches) return false;
  }

  if (areas.length) {
    const areaMatches = areas.some(
      (entry) => entry.PreferredArea === ride.Area,
    );
    if (!areaMatches) return false;
  }

  return true;
};
MASTER.rideMatchesPreferences = rideMatchesPreferences;

const HEBREW_WEEKDAYS = [
  "ראשון",
  "שני",
  "שלישי",
  "רביעי",
  "חמישי",
  "שישי",
  "שבת",
];
MASTER.HEBREW_WEEKDAYS = HEBREW_WEEKDAYS;
MASTER.HEBREW_DAY_ABBR = HEBREW_DAY_ABBR;

const formatHebrewDate = (dateStr) => {
  const d = parseDate(dateStr);
  if (!d) return "";
  return (
    "יום " +
    HEBREW_DAY_ABBR[d.getDay()] +
    ", " +
    d.getDate() +
    "." +
    (d.getMonth() + 1) +
    "." +
    d.getFullYear()
  );
};
MASTER.formatHebrewDate = formatHebrewDate;

// PWA — register service worker (lives one level above /pages/)
if ("serviceWorker" in navigator) {
  const swPath = location.pathname.includes("/pages/") ? "../sw.js" : "sw.js";
  navigator.serviceWorker.register(swPath).catch((err) => {
    MASTER.devLog("Service worker registration failed: " + err, "warning");
  });
}
