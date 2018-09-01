import React, { Component } from 'react';
import {
  BrowserRouter,
  Route,
  Link,
  Redirect,
  withRouter
} from "react-router-dom"
import logo from './logo.svg';
import './App.css';
import HomeView from "./views/HomeView";
import LoginView from "./views/LoginView";
import PrivateRoute from "./components/PrivateRoute";

class App extends Component {
  render() {
    return (
      <div className="App">
        <BrowserRouter>
          <div>
            <header className="App-header">
              <img src={logo} className="App-logo" alt="logo" />
              <h1 className="App-title">Welcome to React</h1>
              <Link to="/">Home</Link>
              |<Link to="/login">Login</Link>
            </header>
            <div>
              <PrivateRoute path="/" exact component={HomeView} />
              <Route path="/login" exact component={LoginView} />
            </div>
          </div>
        </BrowserRouter>
      </div>
    );
  }
}

export default App;
