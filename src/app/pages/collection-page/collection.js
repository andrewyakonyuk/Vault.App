/*global define*/

define(['knockout', 'text!./collection.html'], function (ko, template) {
    'use strict';

    function CollectionViewModel(params) {
        console.log(params.id);
    }

    CollectionViewModel.prototype.afterRender = function () {

    };

    CollectionViewModel.prototype.dispose = function () {

    };

    return {
        viewModel: CollectionViewModel,
        template: template
    };
})