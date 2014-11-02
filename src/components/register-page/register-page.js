define(['knockout', 'text!./register-page.html'], function(ko, templateMarkup) {

    function RegisterPage(){
        var self = this;

        this.name = ko.observable().extend({
           required: { message: 'Please enter your name.' }
        }).isModified(false);

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
            return !self.isValid() && (self.name.isModified() || self.login.isModified() || self.password.isModified());
        }, this);

        this.title = ko.observable('Sign up');
    };

    RegisterPage.prototype.submit = function(e){
        if (this.errors().length === 0) {
            //todo: submit registered data
        } else {
            this.errors.showAllMessages();
        }
        return false;
    }

  // This runs when the component is torn down. Put here any logic necessary to clean up,
  // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
  RegisterPage.prototype.dispose = function() { };

  return { viewModel: RegisterPage, template: templateMarkup };

});
