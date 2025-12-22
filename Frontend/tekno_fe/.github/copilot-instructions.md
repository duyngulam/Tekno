<!-- Guidance for AI coding agents working on `tekno_fe` -->
# Copilot instructions — Tekno Frontend

Purpose: short, concrete rules so AI edits match existing app structure and workflows.

- Big picture
  - Next.js frontend (App Router) with `src/app/` and component modules under `src/components/`.
  - Business logic and API calls live in `src/services/`; shared helpers in `src/lib/`.
  - Global context and hooks in `src/context/` and `src/hook/` (e.g., `AuthContext.tsx`, `useAuth.tsx`).
  - Small global store present in root `store.ts` for lightweight state.

- Dev & build (from `package.json`)
  - `npm run dev` — `next dev --turbopack`
  - `npm run build` — `next build --turbopack`
  - `npm run start`, `npm run lint`

- Conventions & quick rules
  - Keep service wrappers in `src/services/` and reuse `src/lib/api.ts` for shared API utilities.
  - Prefer hooks in `src/hook/` for reusable UI/data logic (`useCart.ts`, `useFavor.ts`).
  - Tailwind/Turbopack specifics: many components rely on `tailwind.config.ts` and `postcss.config.mjs`—avoid changes that require wide Tailwind config edits unless necessary.
  - Icons & small UI atoms use `lucide-react` and `components/ui/` primitives.

- Examples and files to check
  - `src/context/AuthContext.tsx` — how auth tokens and profile are provided.
  - `src/services/products.ts` and `src/lib/api.ts` — API usage pattern.
  - `store.ts` — small global store usage (zustand-like patterns).

- Safety
  - Minimize changes to `next.config.ts` and `tailwind.config.ts` unless requested; they affect the whole app.
