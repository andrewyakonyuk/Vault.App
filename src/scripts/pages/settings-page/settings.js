/*global define, console */
define(['jquery', 'text!./settings.html'], function (ko, template) {
    'use strict';

    function SettingsViewModel() {

    }

    SettingsViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: SettingsViewModel,
        template: template
    };
});
