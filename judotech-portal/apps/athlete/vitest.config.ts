import { defineConfig, mergeConfig } from "vitest/config";
import { testBase } from "@judotech/config/vitest";
import viteConfig from "./vite.config";

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      ...testBase,
      environment: "jsdom",
      setupFiles: ["./src/test/setup.ts"],
      css: true,
    },
  }),
);
