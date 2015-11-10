define(["jquery", "knockout", "packages/cookie"], function($, ko, cookie){
    'use strict';

    var auth = {
        signIn: function(login, password, rememberMe){
            this.isAuthorized(true);
            return $.Deferred()
                .resolve({login: login})
                .promise();
        },
        signOut: function(){
            this.isAuthorized(false);
            $.Deferred()
                .resolve()
                .promise();
        },
        signUp: function(login, password, name){
             $.Deferred()
                .resolve()
                .promise();
        },
        isAuthorized: ko.observable(cookie.hasItem('auth'))
    };

    auth.isAuthorized.subscribe(function (newValue) {
        if (newValue) {
            cookie.setItem('auth', false)
        } else {
            cookie.removeItem('auth')
        }
    });

    return auth;
});
