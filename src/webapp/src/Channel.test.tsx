import React from "react";
import ReactDOM from "react-dom";
import { Channel } from "./Channel";

it("renders without crashing", () => {
  const div = document.createElement("div");
  ReactDOM.render(<Channel />, div);
  ReactDOM.unmountComponentAtNode(div);
});
