/*global define, console */

define(['knockout', 'jquery', 'text!./menubar.html', 'packages/mpmenu', 'packages/auth'], function (ko, $, template, mpmenu, auth) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;

        this.authorized = ko.computed(function () {
            return auth.isAuthorized();
        }, this);
    }

    NavBarViewModel.prototype.render = function () {
        mpmenu();
    };

    NavBarViewModel.prototype.signout = function () {
        auth.signOut();
        return false;
    };

    NavBarViewModel.prototype.dispose = function() {
    };

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
