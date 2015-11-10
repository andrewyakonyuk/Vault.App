/*global define, console */

define(['knockout', 'packages/auth', 'packages/router', 'text!./register-page.html'], function (ko, auth, router, templateMarkup) {
    'use strict';

    function RegisterPage() {
        var self = this;

        this.name = ko.observable().extend({
            required: {
                message: 'Please enter your name.'
            }
        }).isModified(false);

        this.login = ko.observable().extend({
            email: true,
            required: {
                message: 'Please supply your email address.'
            }
        }).isModified(false);

        this.password = ko.observable().extend({
            minLength: 6,
            required: true
        }).isModified(false);

        this.rememberMe = ko.observable(false);

        this.errors = ko.validation.group(self);

        this.hasErrors = ko.computed(function () {
            return self.errors().length && (self.name.isModified() || self.login.isModified() || self.password.isModified());
        }, this);

        this.title = ko.observable('Sign up');
    }

    RegisterPage.prototype.submit = function (e) {
        if (!this.hasErrors()) {
            auth.signUp(this.login(), this.password(), this.name()).done(function(){
                router.navigate('dashboard-page');
            });
        } else {
            this.errors.showAllMessages();
        }
        return false;
    };

    RegisterPage.prototype.dispose = function () {};

    return {
        viewModel: RegisterPage,
        template: templateMarkup
    };

});
