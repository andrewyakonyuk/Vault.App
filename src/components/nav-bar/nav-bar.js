/*global define, console */

define(['app/app', 'knockout', 'hasher', 'jquery', 'text!./nav-bar.html', 'app/offcanvas', 'app/transition', 'app/dropdown'], function (app, ko, hasher, $, template) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;
        this.searchText = ko.observable('');
        this.pageTitle = ko.computed(function () {
            return app.pageTitle();
        }, this);

        this.authorized = ko.computed(function(){
            return app.authorized();
        }, this);
    }

    NavBarViewModel.prototype.render = function () {
        //nothing
    };

    NavBarViewModel.prototype.search = function () {
        if (this.searchText()) {
            var value = this.searchText();
            this.searchText('');
            hasher.setHash('search/' + value);
        }
        return false;
    };

    NavBarViewModel.prototype.signout = function(){
        app.authorized(false);
    }

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
