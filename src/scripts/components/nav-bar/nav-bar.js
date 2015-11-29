/*global define, console, Waves */

define(['knockout', 'jquery', 'text!./nav-bar.html', 'packages/mpmenu', 'packages/auth', 'jquery.nicescroll'], function (ko, $, template, mpmenu, auth) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;

        this.authorized = ko.computed(function () {
            return auth.isAuthorized();
        }, this);
    }

    NavBarViewModel.prototype.render = function () {
        mpmenu();
        $('.mp-menu-content').niceScroll();
    };

    NavBarViewModel.prototype.signout = function () {
        auth.signOut();
        return false;
    };

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
