/*global define, console */
define(['knockout', 'jquery', 'text!./searchbar.html', 'packages/mpmenu', 'packages/awesomplete'], function (ko, $, template, mpmenu) {
    'use strict';

    function SearchViewModel(params) {
        this.route = params.route;
    }

    SearchViewModel.prototype.render = function(){
        mpmenu();
    };

    return {
      viewModel: SearchViewModel,
      template: template
    };

});
