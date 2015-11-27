/*global define */

/*! Pushy - v0.9.2 - 2014-9-13
 * Pushy is a responsive off-canvas navigation menu using CSS transforms & transitions.
 * https://github.com/christophery/pushy/
 * by Christopher Yee */

define(['jquery'], function ($) {
    'use strict';

    var PushMenu = function () {
        var pushy = $('.mp-menu-left-sidebar'), //menu css class
            body = $('body'),
            container = $('.mp-container'), //container css class
            pushyClass = "mp-menu-left mp-menu-open", //menu position & menu open class
            pushyActiveClass = "mp-menu-active"; //css class to toggle site overlay

        function toggleMenuPushy() {
            body.toggleClass(pushyActiveClass); //toggle site overlay
            pushy.toggleClass(pushyClass);
            container.toggleClass("mp-container-push-left");
        }

        function toggleSearchPushy(){
            body.toggleClass(pushyActiveClass);
            $('.mp-menu-right-sidebar').toggleClass('mp-menu-open');
            container.toggleClass("mp-container-push-right");
        }

        //toggle menu
        $(document).on('click', '.mp-menu-toggle-left-btn', function(){
           toggleMenuPushy();
        });

        //close menu when clicking site overlay
        $(document).on('click', '.mp-site-overlay', function () {
            if(container.hasClass('mp-container-push-right')){
                 toggleSearchPushy();
            }
            else if(container.hasClass('mp-container-push-left')){
                toggleMenuPushy();
            }
        });

        $(document).on('click', '.mp-menu-toggle-right-btn', function(){
            toggleSearchPushy();
        });

         $(window).scroll(function () {
            if ($(window).scrollTop() > 10) {
                if(!$('.mp-menu-navbar').hasClass('mp-menu-navbar-scrolled')){
                    $('.mp-menu-navbar').addClass('mp-menu-navbar-scrolled');
                }
            }
             else{
                  $('.mp-menu-navbar').removeClass('mp-menu-navbar-scrolled')
             }
        });
    }

    return PushMenu;
});
