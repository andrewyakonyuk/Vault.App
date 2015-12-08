/*global define */

/*! Pushy - v0.9.2 - 2014-9-13
 * Pushy is a responsive off-canvas navigation menu using CSS transforms & transitions.
 * https://github.com/christophery/pushy/
 * by Christopher Yee */

define(['jquery', 'hammerjs'], function ($, Hammer) {
    'use strict';

    var PushMenu = function () {
        var $pushyLeft = $('.mp-menu-left-sidebar'),
            $pushyRight = $('.mp-menu-right-sidebar'),
            $body = $('body'),
            $container = $('.mp-container'), //container css class,
            $window = $(window),
            $navbar = $('.mp-menu-navbar'),
            pushyClass = "mp-menu-open", //menu position & menu open class
            pushyActiveClass = "mp-menu-active", //css class to toggle site overlay
            overlayClass = "mp-site-overlay";

        var pushLeft = function(){
            $body.addClass(pushyActiveClass); //toggle site overlay
            $pushyLeft.addClass(pushyClass);
            $container.addClass("mp-container-push-left");
        };

        var pushRight = function(){
            $body.addClass(pushyActiveClass);
            $pushyRight.addClass(pushyClass);
            $container.addClass("mp-container-push-right");
        };

        $(document).on('click', '[data-target-bar="left"]', function(){
            requestAnimationFrame(pushLeft);
        });

        $(document).on('click', '[data-target-bar="right"]', function(){
            requestAnimationFrame(pushRight);
        });

        //close menu when clicking site overlay
        $(document).on('click', '.' + overlayClass, function () {
            requestAnimationFrame(function(){
                $body.removeClass(pushyActiveClass);
                $pushyLeft.removeClass(pushyClass);
                $pushyRight.removeClass(pushyClass);
                $container.removeClass('mp-container-push-left');
                $container.removeClass('mp-container-push-right');
            });
        });

        var hammer = new Hammer(document.getElementById('page') || document.getElementsByTagName('body')[0]);
        hammer.on("swiperight", function () {
            pushLeft();
        });

        hammer.on("swipeleft", function () {
            pushRight();
        });

       $window.scroll(function () {
          if ($window.scrollTop() > 10) {
              $navbar.addClass('mp-menu-navbar-scrolled');
          }
           else{
              $navbar.removeClass('mp-menu-navbar-scrolled');
           }
      });
    };

    return PushMenu;
});
