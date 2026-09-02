import base from "@judotech/config/eslint";

/**
 * `@judotech/ui` extends the shared config with two relaxations:
 *
 * - `react-refresh/only-export-components` is off: this is a component library,
 *   not a Vite app with fast refresh, and several modules legitimately export a
 *   provider plus its hook.
 * - `react-hooks/set-state-in-effect` is a warning, not an error, while the
 *   bundled admin-template components (TD-020 / TD-021 in
 *   docs/architecture/technical-debt.md) are still being adopted.
 */
export default [
  ...base,
  {
    files: ["src/**/*.{ts,tsx}"],
    rules: {
      "react-refresh/only-export-components": "off",
      "react-hooks/set-state-in-effect": "warn",
    },
  },
];
