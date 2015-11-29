/*global define, console */
define(["jquery", "knockout", "packages/cookie", "packages/http"], function ($, ko, cookie, http) {
    'use strict';

    var auth = {
        signIn: function (login, password, rememberMe) {
            return http.callService('login', 'put', {login: login, password: password})
                .done(function (data) {
                    auth.ticket(data.ticket);
                })
                .fail(function (jqXHR, textStatus) {
                    console.log(textStatus);
                });
        },
        signOut: function () {
            auth.ticket('');
            $.Deferred()
                .resolve()
                .promise();
        },
        signUp: function (login, password, name) {
            $.Deferred()
                .resolve()
                .promise();
        },
        isAuthorized: ko.observable(cookie.hasItem('auth')),
        ticket: ko.observable(cookie.getItem('auth') || '')
    };

    auth.ticket.subscribe(function (newValue) {
        if (newValue) {
            cookie.setItem('auth', newValue);
        } else {
            cookie.removeItem('auth');
        }

        auth.isAuthorized(!!auth.ticket());
    });

    return auth;
});
