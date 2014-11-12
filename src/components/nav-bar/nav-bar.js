/*global define, console */

define(['app/app', 'knockout', 'hasher', 'jquery', 'text!./nav-bar.html','bootstrap', 'jasny-bootstrap'], function (app, ko, hasher, $, template) {
    'use strict';

    function NavBarViewModel(params) {
        this.route = params.route;
        this.searchText = ko.observable('');
        this.pageTitle = ko.computed(function () {
            console.log(app);
            return app.pageTitle();
        }, this);

        $('.navmenu').offcanvas();
    }

    NavBarViewModel.prototype.render = function () {
        console.log('navbar render');
    };

    NavBarViewModel.prototype.search = function () {
        if (this.searchText()) {
            var value = this.searchText();
            this.searchText('');
            hasher.setHash('search/' + value);
        }
        return false;
    };

    return {
        viewModel: NavBarViewModel,
        template: template
    };
});
