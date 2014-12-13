/*global define, console, Waves */

define(['knockout', 'hasher', 'jquery', 'text!./nav-bar.html', 'packages/mpmenu', 'bootstrap/dropdown'], function (ko, hasher, $, template, mpmenu) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;
        this.searchText = ko.observable('');

        this.authorized = ko.computed(function () {
            return true;
        }, this);
    }

    NavBarViewModel.prototype.render = function () {
        mpmenu();
    };

    NavBarViewModel.prototype.search = function () {
        if (this.searchText()) {
            var value = this.searchText();
            this.searchText('');
            hasher.setHash('search/' + value);
        }
        return false;
    };

    NavBarViewModel.prototype.signout = function () {
        app.authorized(false);
    };

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
