const CACHE_NAME = "r2r-v1";

// App shell — all pages and shared assets
const APP_SHELL = [
  "pages/login.html",
  "pages/main-menu.html",
  "pages/find-ride.html",
  "pages/my-rides.html",
  "pages/preferences.html",
  "js/master.js",
  "js/login.js",
  "js/main-menu.js",
  "js/find-ride.js",
  "js/my-rides.js",
  "js/preferences.js",
  "pages/css/master.css",
  "pages/css/login.css",
  "pages/css/main-menu.css",
  "pages/css/find-ride.css",
  "pages/css/my-rides.css",
  "pages/css/preferences.css",
  "assets/logo-no-bg-sm.png",
  "manifest.json",
];

// Pre-cache the app shell on install
self.addEventListener("install", (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(APP_SHELL)),
  );
  self.skipWaiting();
});

// Clean up old caches on activate
self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) =>
        Promise.all(
          keys.filter((k) => k !== CACHE_NAME).map((k) => caches.delete(k)),
        ),
      ),
  );
  self.clients.claim();
});

// Fetch strategy:
//   - API calls (WebService.asmx) → network only, never cache
//   - Everything else → cache-first, fall back to network
self.addEventListener("fetch", (event) => {
  if (event.request.url.includes("WebService.asmx")) return;

  event.respondWith(
    caches
      .match(event.request)
      .then((cached) => cached || fetch(event.request)),
  );
});
