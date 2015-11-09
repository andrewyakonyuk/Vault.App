/*global define, console */
define(['knockout', 'text!./maps.html'], function (ko, template) {
    'use strict';

    function MapViewModel() {

    }

    MapViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: MapViewModel,
        template: template
    };
});
