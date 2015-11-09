/*global define, console */
define(['knockout', 'text!./musics.html'], function (ko, template) {
    'use strict';

    function MusicViewModel() {

    }

    MusicViewModel.prototype = {
        dispose: function () {

        },

        afterRender: function () {

        }
    };

    return {
        viewModel: MusicViewModel,
        template: template
    };
});
