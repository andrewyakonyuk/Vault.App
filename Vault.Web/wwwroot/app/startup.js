define(function (require, exports, module) {
    'use strict';

    var $ = require('jquery');

    var Pace = require('pace');
    Pace.start({
        restartOnPushState: true
    });

    //require('jquery-pjax');

    //$(document).pjax('[data-pjax] a, a[data-pjax]', '.app-container-holder')

    require('widgets/audio');
    require('plugins/jquery.sticky');
    require('jquery-ui/menu');
    require('jquery-ui/tooltip');

    $('.menu').menu({
        position: { my: "right bottom", at: "right top" }
    }).removeClass('hide');
    $(document).tooltip({
        show: { delay: 800 }
    });

    var AppView = require('modules/views/app');
    var app = new AppView()
        .render();

    $(document).on('pjax:complete', function () {
        app.Board.render();
    });
});