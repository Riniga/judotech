/**
 * Shared Vitest defaults. A package spreads this into its own `test` block:
 *
 *   import { defineConfig } from "vitest/config";
 *   import { testBase } from "@judotech/config/vitest";
 *   export default defineConfig({ test: { ...testBase, environment: "jsdom" } });
 */
export const testBase = {
  passWithNoTests: true,
  clearMocks: true,
  restoreMocks: true,
};
