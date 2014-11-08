define(['knockout', 'text!./signin.html', 'hasher', 'knockout-validation'], function(ko, template, hasher) {

    function SignInViewModel(){
        var self = this;

        this.login = ko.observable().extend({
          email: true,
          required: { message: 'Please supply your email address.' }
        }).isModified(false);

        this.password = ko.observable().extend({
          minLength: 6,
          required: true
        }).isModified(false);

        this.rememberMe = ko.observable(false);

        this.errors = ko.validation.group(self);

        this.hasErrors = ko.computed(function(){
            return !self.isValid() && (self.login.isModified() || self.password.isModified());
        }, this);

        this.title = ko.observable("Sign in");
    };

    SignInViewModel.prototype.submit = function(e){
        if (this.errors().length === 0) {
            //todo: submit login data and check permissions
            hasher.setHash('flow/10');
        } else {
            this.errors.showAllMessages();
        }
        return false;
    }

    return { viewModel: SignInViewModel, template: template };
});
