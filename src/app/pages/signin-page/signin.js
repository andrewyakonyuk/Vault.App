/*global define */

define(['knockout', 'jquery', 'hasher', 'app/app', 'packages/i18n!nls/localizedStrings', 'text!./signin.html'], function (ko, $, hasher, app, localizedStrings, template) {
    'use strict';

    function SignInViewModel() {

        var self = this;

        this.login = ko.observable().extend({
            email: {
                message: localizedStrings.validation.emailShouldBeValid
            },
            required: {
                message: localizedStrings.validation.loginRequired
            }
        }).isModified(false);

        this.password = ko.observable().extend({
            minLength: 6,
            required: {
                message: localizedStrings.validation.fieldisRequired
            }
        }).isModified(false);

        this.rememberMe = ko.observable(false);

        this.errors = ko.validation.group(self);

        this.hasErrors = ko.computed(function () {
            return !self.isValid() && (self.login.isModified() || self.password.isModified());
        }, this);

        this.title = ko.observable("Sign in");
    }

    SignInViewModel.prototype.submit = function (e) {
        if (this.errors().length === 0) {
            //todo: submit login data and check permissions
            app.authorized(true);
            hasher.setHash('flow/10');
        } else {
            this.errors.showAllMessages();
        }
        return false;
    };

    return {
        viewModel: SignInViewModel,
        template: template
    };
});
