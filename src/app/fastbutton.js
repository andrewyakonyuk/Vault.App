/*global define, console */

define(['jquery'], function (jQuery) {
    'use strict';

    (function ($) {
        var Clickbuster, FastButton, clickDistance, clickbusterDistance, clickbusterTimeout, debug, eventHandler;

        clickbusterDistance = 25;

        clickbusterTimeout = 2500;

        clickDistance = 10;

        if (window.debug === null) {
            debug = function (arg) {
                return console.log(arg);
            };
        }

        FastButton = (function () {

            function FastButton(selector, handler) {
                var handlers, that;
                this.selector = selector;
                this.handler = handler;
                if (!("ontouchstart" in window)) {
                    return;
                }
                this.active = false;
                that = this;
                handlers = {
                    touchstart: function (event) {
                        return that.touchStart(event, this);
                    },
                    touchend: function (event) {
                        return that.touchEnd(event, this);
                    }
                };
                $(document).on(handlers, selector).on('touchmove', function () {
                    return that.touchMove(event);
                });
            }

            FastButton.prototype.touchStart = function (event, element) {
                var touch;
                touch = event.originalEvent.touches[0];
                this.active = true;
                this.startX = touch.clientX;
                this.startY = touch.clientY;
                return event.stopPropagation();
            };

            FastButton.prototype.touchMove = function (event) {
                var dx, dy, touch;
                if (!this.active) {
                    return;
                }
                touch = event.originalEvent.touches[0];
                dx = Math.abs(touch.clientX - this.startX);
                dy = Math.abs(touch.clientY - this.startY);
                if (dx > clickDistance || dy > clickDistance) {
                    return this.active = false;
                }
            };

            FastButton.prototype.touchEnd = function (event, element) {
                if (!this.active) {
                    return;
                }
                event.preventDefault();
                this.active = false;
                Clickbuster.preventGhostClick(this.startX, this.startY);
                return this.handler.call(element, event);
            };

            return FastButton;

        })();

        Clickbuster = (function () {

            function Clickbuster() {}

            Clickbuster.coordinates = [];

            Clickbuster.preventGhostClick = function (x, y) {
                Clickbuster.coordinates.push(x, y);
                return window.setTimeout(Clickbuster.pop, clickbusterTimeout);
            };

            Clickbuster.pop = function () {
                return Clickbuster.coordinates.splice(0, 2);
            };

            Clickbuster.onClick = function (event) {
                var coordinates, dx, dy, i, x, y;
                coordinates = Clickbuster.coordinates;
                i = 0;
                if (event.clientX == null) {
                    return true;
                }
                window.ev = event;
                while (i < coordinates.length) {
                    x = coordinates[i];
                    y = coordinates[i + 1];
                    dx = Math.abs(event.clientX - x);
                    dy = Math.abs(event.clientY - y);
                    i += 2;
                    if (dx < clickbusterDistance && dy < clickbusterDistance) {
                        return false;
                    }
                }
                return true;
            };

            return Clickbuster;

        })();

        eventHandler = function (handleObj) {
            var origHandler;
            origHandler = handleObj.handler;
            return handleObj.handler = function (event) {
                if (!Clickbuster.onClick(event)) {
                    return false;
                }
                return origHandler.apply(this, arguments);
            };
        };

        $.event.special.click = {
            add: eventHandler
        };

        $.event.special.submit = {
            add: eventHandler
        };

        $.fn.extend({
            fastButton: function (handler) {
                return $.fastButton(this.selector, handler);
            }
        });

        $.extend({
            fastButton: function (selector, handler) {
                return new FastButton(selector, handler);
            }
        });

        $.fastButton('.use-fastclick a[data-remote],\
   .use-fastclick .fastClick', function (ev) {
            $(this).trigger('click');
            return false;
        });

        $.fastButton('.use-fastclick a:not([data-remote]):not(.fastClick)', function (ev) {
            var $this, href, target;
            $this = $(this);
            target = $this.attr('target');
            href = $this.attr('href');
            if (target === void 0) {
                window.location = href;
            } else {
                window.open(href, target);
            }
            return false;
        });

        $.fastButton('.use-fastclick .submit,\
   .use-fastclick input[type="submit"],\
   .use-fastclick button[type="submit"]', function (ev) {
            $(this).closest('form').trigger('click');
            return false;
        });

        $.fastButton('.use-fastclick input[type="text"]', function (ev) {
            ev.preventDefault();
            $(this).trigger('focus');
            return false;
        });
    })(jQuery);
});
