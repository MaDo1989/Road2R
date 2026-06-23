---
name: senior-friendly-ui
description: 'Build mobile-first web UIs optimized for users aged 55+. Use when creating accessible interfaces for older adults, senior-friendly apps, high-contrast layouts, touch-friendly designs, large-text UIs, or age-inclusive mobile screens. Produces a single self-contained HTML/CSS/JS file with a CSS token system — no external dependencies.'
argument-hint: 'Describe the screen or component to build (e.g. "appointment booking screen")'
---

# Senior-Friendly UI

Builds accessible, mobile-first HTML/CSS/JS JQUERY UIs optimized for users aged 55+.
Always outputs a **single self-contained file** — no external dependencies unless explicitly requested.
Uses **CSS custom properties** as the design token system.

---

## Procedure

1. Read the user's screen or component description.
2. Apply every rule in **Core Design Rules** without exception.
3. Pick the appropriate **Layout Pattern** for the screen type.
4. Apply **Visual Design** rules for all states (default, error, success, empty).
5. Run through the **Avoid** checklist — remove anything that fails it.
6. Emit the complete `<!DOCTYPE html>` file with the token system defined in `:root`.

---

## CSS Token System (always define in `:root`)

```css
:root {
  /* Typography */
  --font-body: 18px;
  --font-heading-sm: 24px;
  --font-heading-lg: 30px;
  --line-height-body: 1.6;
  --line-height-heading: 1.3;

  /* Spacing */
  --padding-card: 16px;
  --gutter: 20px;
  --touch-target: 56px;

  /* Colors — WCAG AA compliant (≥4.5:1 on white) */
  --color-primary: #0057b8;       /* #0057b8 on #fff = 5.9:1 */
  --color-primary-text: #ffffff;
  --color-surface: #ffffff;
  --color-background: #f5f5f5;
  --color-text: #1a1a1a;          /* #1a1a1a on #fff = 17.1:1 */
  --color-text-secondary: #595959;/* #595959 on #fff = 7.0:1  */
  --color-border: #767676;        /* #767676 on #fff = 4.54:1 */
  --color-error: #b91c1c;         /* #b91c1c on #fff = 5.9:1  */
  --color-success: #166534;       /* #166534 on #fff = 7.3:1  */
  --color-warning: #92400e;       /* #92400e on #fff = 5.9:1  */

  /* Shadows */
  --shadow-card: 0 2px 8px rgba(0,0,0,0.12);

  /* Animation */
  --duration-fast: 200ms;
  --duration-max: 300ms;
}
```

---

## Core Design Rules

| Rule | Requirement |
|------|-------------|
| Touch targets | **56px minimum height** for every interactive element (button, input, link, checkbox, radio) |
| Font size | **18px minimum** for body; headings **24–30px** |
| Contrast | Text must meet **WCAG AA** — 4.5:1 minimum ratio. Use only token colors above. |
| Touch-first | **No hover-only interactions** — all affordances must work on tap |
| Padding | **16px inside cards**, **20px page gutters** minimum |
| Line height | **1.6** for body text, **1.3** for headings |
| Animation | **≤ 300ms** for all transitions and animations |

---

## Layout Patterns

### Default screen layout
```
┌─────────────────────────────────┐
│  Page header (title + back)     │
├─────────────────────────────────┤
│                                 │
│  Single-column content area     │
│  (20px gutters)                 │
│                                 │
│  [Sticky CTA at bottom]         │
├─────────────────────────────────┤
│  Bottom nav (≤4 items, 56px+)   │
└─────────────────────────────────┘
```

- **Single column** is the default. Use max 2 columns only for card/grid layouts.
- **Bottom navigation bar** — thumb-reachable, max 4 items, icon + text label always.
- **Sheet modals** — slide up from the bottom, never centered popups.
- **Sticky CTA** — pin the primary action button at the bottom of scroll areas.
- **One primary action per screen** — use `--color-primary` for only one button per view.

---

## Visual Design

### Cards
- Flat white surface (`--color-surface`) with `--shadow-card`
- No glassmorphism, no gradients on text
- `--padding-card` (16px) inside, rounded corners (`border-radius: 12px`)

### Status communication
Always use **icon + color + text label** together — never color alone:
- Error: `✕` icon + `--color-error` + label text
- Success: `✓` icon + `--color-success` + label text
- Warning: `⚠` icon + `--color-warning` + label text

### Empty states
Must include a **visible action button** — not just a message.
```html
<!-- Example empty state -->
<div class="empty-state">
  <span class="empty-icon" aria-hidden="true">📋</span>
  <p>No appointments yet.</p>
  <button class="btn-primary">Book an Appointment</button>
</div>
```

### Toast feedback
- Auto-dismisses in **2.5 seconds**
- Contains: color + icon + text label
- Positioned bottom-center, above bottom nav
- Use for both error and success feedback

```js
function showToast(message, type = 'success') {
  // type: 'success' | 'error' | 'warning'
  const icons = { success: '✓', error: '✕', warning: '⚠' };
  const toast = document.createElement('div');
  toast.className = `toast toast--${type}`;
  toast.innerHTML = `<span aria-hidden="true">${icons[type]}</span> ${message}`;
  toast.setAttribute('role', 'alert');
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 2500);
}
```

---

## Avoid Checklist

Before emitting the final file, verify none of these are present:

- [ ] Icons without visible text labels
- [ ] Carousels or swipe-only gestures (no `overflow-x: scroll` galleries)
- [ ] Dense information tables (use stacked cards instead)
- [ ] Inputs with placeholder text only — **always show a `<label>` above the input**
- [ ] Hover-only states (`:hover` without equivalent `:focus`/`:active`)
- [ ] Animations or transitions longer than 300ms
- [ ] Text that fails 4.5:1 contrast ratio
- [ ] Touch targets shorter than 56px height
- [ ] Centered popup modals (use bottom sheets)
- [ ] More than 4 bottom nav items
- [ ] More than 2 columns in any layout

---

## Output Format

Emit a complete, valid file:

```
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>[Screen Title]</title>
  <style>
    /* 1. :root token definitions */
    /* 2. Reset + base styles */
    /* 3. Layout */
    /* 4. Components */
    /* 5. Utilities (toast, etc.) */
  </style>
</head>
<body>
  <!-- content -->
  <script>
    /* Interaction logic only — no framework */
  </script>
</body>
</html>
```

Structure CSS in that exact order. No `<link>` tags, no `<script src>` tags unless the user explicitly requests an external dependency.
