/*global define */

define(['knockout', 'packages/router', 'packages/auth', 'packages/i18n!nls/localizedStrings', 'text!./signin.html'], function (ko, router, auth, localizedStrings, template) {
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
            return self.errors().length && (self.login.isModified() || self.password.isModified());
        }, this);

        this.title = ko.observable("Sign in");
    }

    SignInViewModel.prototype.submit = function (e) {
        var self = this;
        if (!self.hasErrors()) {
            auth.signIn(self.login(), self.password(), self.rememberMe())
                .done(function(){
                    router.navigate('dashboard-page');
                }).fail(function(){
                    self.errors.showAllMessages();
            });
        } else {
            self.errors.showAllMessages();
        }
        return false;
    };

    return {
        viewModel: SignInViewModel,
        template: template
    };
});
