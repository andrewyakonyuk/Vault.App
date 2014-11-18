/*global define */

define(['knockout', 'text!./list-page.html'], function (ko, templateMarkup) {
    'use strict';

    function FlowPage(params) {
        this.pageTitle = ko.observable("Flow");
    }

    FlowPage.prototype.dispose = function () {};

    return {
        viewModel: FlowPage,
        template: templateMarkup
    };
});
