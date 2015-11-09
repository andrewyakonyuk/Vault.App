/*global define*/

define(['knockout', 'packages/i18n!nls/localizedStrings', 'text!./breadcrumbs.html'], function (ko, localizedStrings, template) {
    'use strict';

    function BreadcrumbsViewModel(params) {
        if (params.active) {
            this.active = ko.observable({
                title: localizedStrings[params.active.title],
            });
        }
        if (params.root) {
            this.root = ko.observable({
                title: localizedStrings[params.root.title],
                url: '#' + params.root.url
            });
        }
    }

    BreadcrumbsViewModel.prototype.dispose = function () {

    };

    return {
        viewModel: BreadcrumbsViewModel,
        template: template
    }
})
