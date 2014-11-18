/*global define */

define(['app/app', 'knockout', 'text!./search-page.html'], function (app, ko, templateMarkup) {
    'use strict';

    function SearchPage(params) {
        var title = "Search: " + params.searchText;
        this.pageTitle = ko.observable(title);

        this.items = ko.observableArray([1, 2, 3, 4, 5, 6, 7, 8, 9, 0]);
    }

    SearchPage.prototype.dispose = function () {};

    return {
        viewModel: SearchPage,
        template: templateMarkup
    };
});
