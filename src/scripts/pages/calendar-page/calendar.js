/*global define, console */
define(['knockout', 'text!./calendar.html'], function (ko, template) {
    'use strict';

    function CalendarViewModel() {

    }

    CalendarViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: CalendarViewModel,
        template: template
    };
});
