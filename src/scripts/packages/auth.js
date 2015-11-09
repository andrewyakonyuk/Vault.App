define(["jquery", "knockout"], function($, ko){
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
        isAuthorized: ko.observable(false)
    };


    return auth;
});
