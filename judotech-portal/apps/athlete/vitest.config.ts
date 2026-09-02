import { defineConfig, mergeConfig } from "vitest/config";
import { testBase } from "@judotech/config/vitest";
import viteConfig from "./vite.config";

// Component tests and a jsdom setup file are added in MVP-001 Phase 6.
export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      ...testBase,
    },
  }),
);
