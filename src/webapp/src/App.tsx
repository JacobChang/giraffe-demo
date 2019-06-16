import React from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";
import "./App.css";
import { Home } from "./Home";
import { Channel } from "./Channel";

const App: React.FC = () => {
  return (
    <Router>
      <Route path="/" exact component={Home} />
      <Route path="/channels/:id" component={Channel} />
    </Router>
  );
};

export default App;
