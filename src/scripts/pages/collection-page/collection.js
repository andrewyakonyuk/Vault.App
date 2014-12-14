/*global define, console*/

define(['knockout', 'text!./collection.html'], function (ko, template) {
    'use strict';

    function CollectionViewModel(params) {
        console.log(params.id);
    }

    CollectionViewModel.prototype = {
        afterRender: function () {

        },

        dispose: function () {

        }
    };

    return {
        viewModel: CollectionViewModel,
        template: template
    };
});
