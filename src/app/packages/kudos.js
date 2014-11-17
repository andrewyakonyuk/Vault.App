/*global define */

define(['jquery'], function ($) {
    'use strict';

    var Kudoable,
        __bind = function (fn, me) {
            return function () {
                return fn.apply(me, arguments);
            };
        };

    Kudoable = (function () {
        function Kudoable(element) {
            this.element = element;
            this.unkudo = __bind(this.unkudo, this);
            this.complete = __bind(this.complete, this);
            this.end = __bind(this.end, this);
            this.start = __bind(this.start, this);
            this.bindEvents();
            this.counter = $('.count .num', this.element);
            this.element.data('kudoable', this);
        }

        Kudoable.prototype.bindEvents = function () {
            this.element.mouseenter(this.start);
            this.element.mouseleave(this.end);
            this.element.click(this.unkudo);
            $(this.element).on('touchstart', this.element, $.proxy(this.touchstart, this));
            return $(this.element).on('touchend', this.element, $.proxy(this.end, this));
        };

        Kudoable.prototype.isKudoable = function () {
            return this.element.hasClass('kudoable');
        };

        Kudoable.prototype.isKudod = function () {
            return this.element.hasClass('complete');
        };

        Kudoable.prototype.touchstart = function (e) {
            if (this.isKudoable() && !this.isKudod()) {
                this.start(e);
            } else if (this.isKudod()) {
                this.unkudo(e);
            }
        };

        Kudoable.prototype.start = function (e) {
            if (this.isKudoable() && !this.isKudod()) {
                this.element.trigger('kudo:active');
                this.element.addClass('active');
                return (this.timer = setTimeout(this.complete, 700));
            }
        };

        Kudoable.prototype.end = function () {
            if (this.isKudoable() && !this.isKudod()) {
                this.element.trigger('kudo:inactive');
                this.element.removeClass('active');
                if (this.timer !== null) {
                    return clearTimeout(this.timer);
                }
            }
        };

        Kudoable.prototype.complete = function () {
            this.end();
            this.incrementCount();
            this.element.addClass('complete');
            return this.element.trigger('kudo:added');
        };

        Kudoable.prototype.unkudo = function (event) {
            event.preventDefault();
            if (this.isKudod()) {
                this.decrementCount();
                this.element.removeClass('complete');
                return this.element.trigger('kudo:removed');
            }
        };

        Kudoable.prototype.setCount = function (count) {
            return this.counter.html(count);
        };

        Kudoable.prototype.currentCount = function () {
            return parseInt(this.counter.html());
        };

        Kudoable.prototype.incrementCount = function () {
            return this.setCount(this.currentCount() + 1);
        };

        Kudoable.prototype.decrementCount = function () {
            return this.setCount(this.currentCount() - 1);
        };

        return Kudoable;

    })();

    return ($.fn.kudoable = function () {
        return this.each(function () {
            return new Kudoable($(this));
        });
    });
});