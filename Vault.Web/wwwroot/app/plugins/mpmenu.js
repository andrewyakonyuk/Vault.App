/*global define */

define(['jquery', 'jquery-ui/accordion'], function ($, Hammer) {
    'use strict';

    var PushMenu = function () {
        var $sidebar = $('.js-draw-menu-sidebar'),
            $body = $('body'),
            $container = $('.js-draw-container'), //container css class,
            $window = $(window);

        var pushLeft = function(){
            $body.addClass("draw-menu-active"); //toggle site overlay
            $sidebar.addClass("draw-menu-open");
            $container.addClass("draw-container-push");
        };

        $(document).on('click', '.js-draw-menu-btn', function(){
            requestAnimationFrame(pushLeft);
        });

        //close menu when clicking site overlay
        $(document).on('click', '.js-draw-overlay', function () {
            requestAnimationFrame(function(){
                $body.removeClass("draw-menu-active");
                $sidebar.removeClass("draw-menu-open");
                $container.removeClass('draw-container-push');
            });
        });

       $('.js-sidebar-list .js-expand').accordion({
           collapsible: true,
           icons: false
       });

       $window.resize(function () {
           $('.draw-menu-holder').height($window.height());
           $('.js-sidebar-list .js-expand').accordion("refresh");
       });

       $window.resize();
    };

    return PushMenu;
});
