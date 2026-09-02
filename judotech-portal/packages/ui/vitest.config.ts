import { defineConfig } from "vitest/config";
import { testBase } from "@judotech/config/vitest";

export default defineConfig({
  test: {
    ...testBase,
    environment: "jsdom",
    setupFiles: ["./vitest.setup.ts"],
  },
});
