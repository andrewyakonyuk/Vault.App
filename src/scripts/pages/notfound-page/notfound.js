/*global define */
define(['knockout', 'text!./notfound.html'], function (ko, template) {
    'use strict';

    function NotFoundViewModel(params) {

    }

    return {
        viewModel: NotFoundViewModel,
        template: template
    };
});
