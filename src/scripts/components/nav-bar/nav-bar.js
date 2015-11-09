/*global define, console, Waves */

define(['knockout','jquery', 'text!./nav-bar.html', 'packages/mpmenu', 'packages/auth', 'bootstrap/dropdown'], function (ko, $, template, mpmenu, auth) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;
        this.searchText = ko.observable('');

        this.authorized = ko.computed(function () {
            return auth.isAuthorized();
        }, this);
    }

    NavBarViewModel.prototype.render = function () {
        mpmenu();
    };

    NavBarViewModel.prototype.signout = function () {
        auth.signOut();
    };

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
