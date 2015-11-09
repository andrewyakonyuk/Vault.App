/*global define, console */
define(['knockout', 'text!./labels.html'], function (ko, template) {
    'use strict';

    function LabelsViewModel(params) {

    }

    LabelsViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: LabelsViewModel,
        template: template
    };
});
