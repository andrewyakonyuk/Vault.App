/*global define, console */
define(['knockout', 'jquery', 'packages/http', 'text!./calendar.html'], function (ko, $, http, template) {
    'use strict';

    function CalendarViewModel() {
        var self = this;
        this.events = ko.observableArray([]);
        http.callService('events', 'get').done(function(data){
            self.events(data);
        });
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
