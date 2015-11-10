/*global define */

/*! Pushy - v0.9.2 - 2014-9-13
 * Pushy is a responsive off-canvas navigation menu using CSS transforms & transitions.
 * https://github.com/christophery/pushy/
 * by Christopher Yee */

define(['jquery'], function ($) {
    'use strict';

    var PushMenu = function () {

        var pushy = $('.mp-menu-left'), //menu css class
            body = $('body'),
            container = $('.mp-container'), //container css class
            push = $('.push'), //css class to add pushy capability
            siteOverlay = $('.mp-site-overlay'), //site overlay
            pushyClass = "mp-menu-left mp-menu-open", //menu position & menu open class
            pushyActiveClass = "mp-menu-active", //css class to toggle site overlay
            containerClass = "mp-container-push-left", //container open class
            menuLeftBtn = $('.mp-menu-toggle-left-btn'), //css classes to toggle the menu
            menuRightBtn = $('.mp-menu-toggle-right-btn'),
            menuSpeed = 200, //jQuery fallback menu speed
            menuWidth = pushy.width() + "px"; //jQuery fallback menu width

        function toggleMenuPushy() {
            body.toggleClass(pushyActiveClass); //toggle site overlay
            pushy.toggleClass(pushyClass);
            container.toggleClass(containerClass);
        }

        function toggleSearchPushy(){
            body.toggleClass(pushyActiveClass);
            $('.mp-menu-right').toggleClass('mp-menu-open');
            container.toggleClass("mp-container-push-right");
        }


        function openPushyFallback() {
            body.addClass(pushyActiveClass);
            pushy.animate({
                left: "0px"
            }, menuSpeed);
            container.animate({
                left: menuWidth
            }, menuSpeed);
            push.animate({
                left: menuWidth
            }, menuSpeed); //css class to add pushy capability
        }

        function closePushyFallback() {
            body.removeClass(pushyActiveClass);
            pushy.animate({
                left: "-" + menuWidth
            }, menuSpeed);
            container.animate({
                left: "0px"
            }, menuSpeed);
            push.animate({
                left: "0px"
            }, menuSpeed); //css class to add pushy capability
        }

        //checks if 3d transforms are supported removing the modernizr dependency
        var cssTransforms3d = (function csstransforms3d() {
            var el = document.createElement('p'),
                supported = false,
                transforms = {
                    'webkitTransform': '-webkit-transform',
                    'OTransform': '-o-transform',
                    'msTransform': '-ms-transform',
                    'MozTransform': '-moz-transform',
                    'transform': 'transform'
                };

            // Add it to the body to get the computed style
            document.body.insertBefore(el, null);

            for (var t in transforms) {
                if (el.style[t] !== undefined) {
                    el.style[t] = 'translate3d(1px,1px,1px)';
                    supported = window.getComputedStyle(el).getPropertyValue(transforms[t]);
                }
            }

            document.body.removeChild(el);

            return (supported !== undefined && supported.length > 0 && supported !== "none");
        })();

        if (cssTransforms3d) {
            //toggle menu
            menuLeftBtn.click(function () {
                toggleMenuPushy();
            });
            //close menu when clicking site overlay
            siteOverlay.click(function () {
                if(container.hasClass('mp-container-push-right')){
                     toggleSearchPushy();
                }
                else if(container.hasClass('mp-container-push-left')){
                    toggleMenuPushy();
                }
            });
            menuRightBtn.click(function(){
                toggleSearchPushy();
            });
        } else {
            //jQuery fallback
            pushy.css({
                left: "-" + menuWidth
            }); //hide menu by default
            container.css({
                "overflow-x": "hidden"
            }); //fixes IE scrollbar issue

            //keep track of menu state (open/close)
            var state = true;

            //toggle menu
            menuLeftBtn.click(function () {
                if (state) {
                    openPushyFallback();
                    state = false;
                } else {
                    closePushyFallback();
                    state = true;
                }
            });

            //close menu when clicking site overlay
            siteOverlay.click(function () {
                if (state) {
                    openPushyFallback();
                    state = false;
                } else {
                    closePushyFallback();
                    state = true;
                }
            });
        }

         $(window).scroll(function () {
            if ($(window).scrollTop() > $('.mp-navbar').height() - 7) {
                if(!$('.mp-navbar').hasClass('mp-navbar-scrolled')){
                    $('.mp-navbar').addClass('mp-navbar-scrolled');
                }
            }
             else{
                  $('.mp-navbar').removeClass('mp-navbar-scrolled')
             }
        });
    }

    return PushMenu;
});
