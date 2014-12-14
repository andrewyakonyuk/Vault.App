/*global define, console */
define(['jquery', 'text!./dashboard.html'], function (ko, template) {
    'use strict';

    function DashboardViewModel() {

    }

    DashboardViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: DashboardViewModel,
        template: template
    };
});
