---
name: r2r-client
description: "Road to Recovery mobile app client-side conventions. Use when adding pages, features, JS logic, or styles to this project. Covers file structure, MASTER utilities, API calls, CSS tokens, and page patterns."
argument-hint: "What page or feature are you working on?"
---

# R2R Client-Side Conventions

## Project Stack

- **HTML + jQuery + SweetAlert2** (no framework)
- **RTL Hebrew UI**, senior-friendly touch targets
- ASMX web service backend (see `API_documentation.md`)

---

## File Structure Rule

Every page follows this pattern — **never cross-contaminate**:

| Concern                            | File                        |
| ---------------------------------- | --------------------------- |
| Shared JS logic / utilities        | `js/master.js`              |
| Shared design tokens & base styles | `pages/css/master.css`      |
| Page HTML                          | `pages/<page-name>.html`    |
| Page JS                            | `js/<page-name>.js`         |
| Page CSS                           | `pages/css/<page-name>.css` |

> **Rule:** If a piece of logic or style is used by more than one page → it belongs in `master.js` / `master.css`. If it's specific to one page → it goes in that page's own file.

---

## HTML Page Boilerplate

```html
<!doctype html>
<html lang="he" dir="rtl">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="theme-color" content="#005f85" />
    <title><!-- Page Title --> — Road to Recovery</title>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="../js/master.js"></script>
    <link rel="stylesheet" href="css/master.css" />
    <link rel="stylesheet" href="css/<page-name>.css" />
  </head>
  <body>
    <div class="page page-with-nav">
      <header class="header" id="appHeader"></header>
      <main class="content"><!-- page content --></main>
      <nav class="bottom-nav"><!-- nav buttons --></nav>
    </div>
    <div id="toast-container"></div>
    <script src="../js/<page-name>.js"></script>
  </body>
</html>
```

---

## MASTER Utilities — Always Use These

### Page Init Pattern (every page)

```js
$(function () {
  MASTER.renderHeader("#appHeader", { title: "כותרת הדף" });

  var user = MASTER.getCurrentUser();
  if (!user) {
    window.location.replace("login.html");
    return;
  }

  // page logic here

  MASTER.IsProductionDatabase(function (isProd) {
    if (!isProd) $(".header").css("background-color", "#f39c12"); // orange = test DB
  });
});
```

### API Calls

```js
MASTER.ajax(
  "EndpointName", // ASMX method name
  { param: value }, // request payload
  function (wrapper) {
    var data = MASTER.parseResponse(wrapper); // unwraps .d or []
  },
  function (xhr, status, err) {
    MASTER.devLog("Error: " + err, "error");
    MASTER.showToast("שגיאה בטעינת הנסיעות. נסו שנית.", "error");
  },
);
```

### Toasts

```js
MASTER.showToast("הפעולה הצליחה", "success"); // success | error | warning
```

### Logging (dev only)

```js
MASTER.devLog("message", "error"); // error | warning | important
```

### Date / Time Helpers

```js
MASTER.parseDate(dateString); // → Date or null
MASTER.formatHebrewDate(dateString); // → "יום ראשון, 01/06/2025"
MASTER.getRideTimeDisplay(pickupTime, isAfterNoon); // → { time, label, showLabel }
```

### User Session

```js
var user = MASTER.getCurrentUser(); // from sessionStorage — null if not logged in
```

---

## CSS — Use Tokens, Never Hard-Code Values

All tokens are defined in `master.css`. Always use them:

```css
/* Colors */
--color-primary:
  #005f85 --color-background: #e8f2f7 --color-text: #1a1a1a
    --color-error: #b91c1c --color-success: #166534 /* Spacing & Sizing */
    --padding-card,
  --gutter, --touch-target: 56px, --radius-card,
  --radius-btn /* Typography */ --font-body: 18px, --font-heading-sm: 24px,
  --font-heading-lg: 32px;
```

> **Rule:** If you need a new reusable token (color, spacing, shadow), add it to `master.css :root`. If it's only used on one page, define it as a local CSS variable in that page's CSS file.

---

## Adding a New Page — Checklist

1. Create `pages/<name>.html` using the boilerplate above
2. Create `js/<name>.js` with the standard init pattern
3. Create `pages/css/<name>.css` (page-specific styles only)
4. Add the page's nav item to the `bottom-nav` on all other pages
5. If new shared logic is needed → add to `master.js` / `master.css`

---

## Key Conventions

- **Auth guard on every page** — redirect to `login.html` if no user
- **Non-prod indicator** — orange header when not on production DB
- **Error states** — always show a retry button; use `MASTER.showToast` for transient feedback
- **Accessibility** — `role`, `aria-*`, `tabindex` on interactive elements; minimum 56px touch targets
- **No inline styles** — use CSS classes and tokens; exception is the orange header override
