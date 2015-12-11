/*global define, console*/

define(['knockout', 'text!./collection.html'], function (ko, template) {
    'use strict';

    function CollectionViewModel(params) {
        console.log(params.id);
        this.items = ko.observableArray([1,2,3,4,5,6,7,8,9,0]);
        this.isViewportOpen = ko.observable(false);
    }

    CollectionViewModel.prototype = {
        afterRender: function () {

        },

        dispose: function () {

        },

        openViewport: function(){
          console.log('openViewport')
          this.isViewportOpen(true);
        },

        closeViewport: function(){
          this.isViewportOpen(false);
        }
    };

    return {
        viewModel: CollectionViewModel,
        template: template
    };
});
