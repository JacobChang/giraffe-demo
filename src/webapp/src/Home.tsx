import React from "react";
import "./Home.css";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import CircularProgress from "@material-ui/core/CircularProgress";
import { ajax } from "rxjs/ajax";
import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import Fab from "@material-ui/core/Fab";
import AddIcon from "@material-ui/icons/Add";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import { thisExpression } from "@babel/types";
import { Subscription } from "rxjs/internal/Subscription";
import { Link } from "react-router-dom";

interface Props {}

interface Channel {
  id: string;
  title: string;
  startTime: Date;
  duration: number;
}

type Status = "idle" | "loading" | "error" | "ready";

interface State {
  status: Status;
  channels: Channel[];
  openModal: boolean;
  channelForm: {
    title: string;
    duration: number;
  };
}

export class Home extends React.Component<Props, State> {
  refreshSubscription?: Subscription;
  createSubscription?: Subscription;

  constructor(props: Props) {
    super(props);

    this.state = {
      status: "idle",
      channels: [],
      openModal: false,
      channelForm: {
        title: "",
        duration: 30
      }
    };
  }

  componentDidMount() {
    this.refreshSubscription = ajax("/api/v1/channels").subscribe(
      res => {
        let channels = res.response.map((channel: Channel) => {
          return {
            ...channel,
            startTime: new Date(channel.startTime)
          };
        });
        this.setState({
          status: "ready",
          channels
        });
      },
      err => {
        this.setState({
          status: "error"
        });
      }
    );
  }

  componentWillUnmount() {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }

    if (this.createSubscription) {
      this.createSubscription.unsubscribe();
    }
  }

  handleOpen = () => {
    this.setState({
      openModal: true
    });
  };

  updateTitle = (evt: any) => {
    this.setState({
      channelForm: {
        ...this.state.channelForm,
        title: evt.target.value
      }
    });
  };

  updateDuration = (evt: any) => {
    this.setState({
      channelForm: {
        ...this.state.channelForm,
        duration: evt.target.value
      }
    });
  };

  handleSubmit = () => {
    let params = {
      ...this.state.channelForm
    };

    this.createSubscription = ajax
      .post("/api/v1/channels", JSON.stringify(params), {
        "Content-Type": "application"
      })
      .subscribe(
        res => {
          this.setState({
            channels: [res.response, ...this.state.channels]
          });
        },
        err => {},
        () => {
          this.setState({
            openModal: false
          });
        }
      );
  };

  handleClose = () => {
    this.setState({
      openModal: false
    });
  };

  render() {
    const { openModal } = this.state;

    return (
      <div>
        <AppBar position="static">
          <Toolbar>
            <Typography className="app__title" variant="h6">
              Hurry
            </Typography>
            <Link to="/">Login</Link>
          </Toolbar>
        </AppBar>
        <Container>
          {this.state.status === "loading" || this.state.status === "idle" ? (
            <Paper>
              <CircularProgress />
            </Paper>
          ) : null}
          {this.state.status === "error" ? <div /> : null}
          {this.state.status === "ready" ? (
            <div>
              {this.state.channels.map(channel => {
                return (
                  <Paper key={channel.id}>
                    <h4>{channel.title}</h4>
                    <Link to={`/channels/${channel.id}`}>Join</Link>
                  </Paper>
                );
              })}
            </div>
          ) : null}
        </Container>

        <Dialog
          open={openModal}
          onClose={this.handleClose}
          aria-labelledby="form-dialog-title"
          disableBackdropClick
          disableEscapeKeyDown
        >
          <DialogTitle id="form-dialog-title">New Channel</DialogTitle>
          <DialogContent>
            <DialogContentText>
              To create new channel, please enter the title and duration here.
            </DialogContentText>
            <TextField
              autoFocus
              margin="normal"
              id="title"
              label="Channel title"
              type="text"
              fullWidth
              value={this.state.channelForm.title}
              onChange={this.updateTitle}
            />
            <TextField
              margin="normal"
              id="duration"
              label="Channel duration(in minutes)"
              type="number"
              fullWidth
              value={this.state.channelForm.duration}
              onChange={this.updateDuration}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={this.handleClose}>Cancel</Button>
            <Button onClick={this.handleSubmit} color="primary">
              Create
            </Button>
          </DialogActions>
        </Dialog>

        <Fab color="primary" aria-label="Add" onClick={this.handleOpen}>
          <AddIcon />
        </Fab>
      </div>
    );
  }
}
