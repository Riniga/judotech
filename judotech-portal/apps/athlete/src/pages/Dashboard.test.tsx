import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import Dashboard from "./Dashboard";

describe("Dashboard", () => {
  it("renders", () => {
    render(<Dashboard />);

    expect(screen.getByText("test")).toBeInTheDocument();
  });
});
