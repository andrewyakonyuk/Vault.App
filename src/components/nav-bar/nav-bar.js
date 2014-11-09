define(['knockout', 'hasher', 'jquery', 'text!./nav-bar.html'], function (ko, hasher, $, template) {

    function NavBarViewModel(params) {

        // This viewmodel doesn't do anything except pass through the 'route' parameter to the view.
        // You could remove this viewmodel entirely, and define 'nav-bar' as a template-only component.
        // But in most apps, you'll want some viewmodel logic to determine what navigation options appear.

        this.route = params.route;
        this.searchText = ko.observable('');
    }

    NavBarViewModel.prototype.render = function () {
        'use strict';
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
