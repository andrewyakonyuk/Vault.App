/*global define */
define(['knockout', 'text!./musics.html', 'packages/http', 'packages/mediaplayer'], function (ko, template, http, mediaplayer) {
    'use strict';

    function MusicViewModel() {
      var self = this;
      this.medias = ko.observableArray([]);
      http.callService('musics', 'get').done(function(data){
        self.medias(data);
        $('.media-player').each(function(){
          mediaplayer($(this));
        });
      });
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
