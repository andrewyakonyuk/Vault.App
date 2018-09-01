import React from "react";
import {
  Route,
  Redirect,
} from "react-router-dom";
import auth from "../auth";

class PrivateRoute extends React.Component{
    render () {
        const { component, ...rest } = this.props;
        const Component = component;
        return (<Route
        {...rest}
        render={props =>
            auth.isAuthenticated ? (
            <Component {...props} />
            ) : (
            <Redirect
                to={{
                pathname: "/login",
                state: { from: props.location }
                }}
            />
            )
        }
        />);
    }
}

export default PrivateRoute;