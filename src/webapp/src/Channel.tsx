import React from "react";
import "./Channel.css";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import BackIcon from "@material-ui/icons/KeyboardArrowLeft";
import { Link } from "react-router-dom";
import { HubConnectionBuilder } from "@aspnet/signalr";

export class Channel extends React.Component {
  componentDidMount() {
    let connection = new HubConnectionBuilder().withUrl("/chathub").build();
    connection.on("send", data => {
      console.log(data);
    });
    connection.start().then(() => connection.invoke("send", "Hello"));
  }

  render() {
    return (
      <div>
        <AppBar position="static">
          <Toolbar>
            <Link to="/">
              <BackIcon />
            </Link>
          </Toolbar>
        </AppBar>
      </div>
    );
  }
}
