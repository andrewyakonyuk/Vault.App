/*global define */

define(["knockout", "text!./home.html"], function (ko, homeTemplate) {
    'use strict';

    function HomeViewModel(route) {
        this.title = ko.observable("Home");
    }

    return {
        viewModel: HomeViewModel,
        template: homeTemplate
    };

});
