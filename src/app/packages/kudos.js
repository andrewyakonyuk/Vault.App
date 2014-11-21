/*global define */

define([], function () {
    'use strict';

    var Kudoable,
        __bind = function (fn, me) {
            return function () {
                return fn.apply(me, arguments);
            };
        },

        __hasClass = function (el, className) {
            if (el.classList)
                return el.classList.contains(className);
            else
                return new RegExp('(^| )' + className + '( |$)', 'gi').test(el.className);
        },

        __addClass = function (el, className) {
            if (el.classList)
                el.classList.add(className);
            else
                el.className += ' ' + className;
        },

        __removeClass = function (el, className) {
            if (el.classList)
                el.classList.remove(className);
            else
                el.className = el.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
        },

        __customTrigger = function (el, eventName, data) {
            if (window.CustomEvent) {
                var event = new CustomEvent(eventName, data);
            } else {
                var event = document.createEvent('CustomEvent');
                event.initCustomEvent(eventName, true, true, data);
            }

            el.dispatchEvent(event);
        };

    Kudoable = (function () {
        function Kudoable(element) {
            this.element = element;
            this.unkudo = __bind(this.unkudo, this);
            this.complete = __bind(this.complete, this);
            this.end = __bind(this.end, this);
            this.start = __bind(this.start, this);
            this.bindEvents();
            this.counter = this.element.querySelector('.count .num');
            this.element.setAttribute('data-kudoable', this);
        }

        Kudoable.prototype.bindEvents = function () {
            this.element.addEventListener('mouseenter', this.start, true);
            this.element.addEventListener('mouseleave', this.end, true);
            this.element.addEventListener('click', this.unkudo, true);
            this.element.addEventListener('touchstart', __bind(this.touchstart, this), true);
            this.element.addEventListener('touchend', __bind(this.end, this), true);
        };

        Kudoable.prototype.isKudoable = function () {
            return __hasClass(this.element, 'kudoable');
        };

        Kudoable.prototype.isKudod = function () {
            return __hasClass(this.element, 'complete');
        };

        Kudoable.prototype.touchstart = function (e) {
            if (this.isKudoable() && !this.isKudod()) {
                this.start(e);
            } else if (this.isKudoable() && this.isKudod()) {
                this.unkudo(e);
            }
        };

        Kudoable.prototype.start = function (e) {
            if (this.isKudoable() && !this.isKudod()) {
                __customTrigger(this.element, 'kudo:active');
                __addClass(this.element, 'active');
                return (this.timer = setTimeout(this.complete, 700));
            }
        };

        Kudoable.prototype.end = function () {
            if (this.isKudoable() && !this.isKudod()) {
                __customTrigger(this.element, 'kudo:inactive');
                __removeClass(this.element, 'active');
                if (this.timer !== null) {
                    return clearTimeout(this.timer);
                }
            }
        };

        Kudoable.prototype.complete = function () {
            this.end();
            this.incrementCount();
            __addClass(this.element, 'complete');
            __customTrigger(this.element, 'kudo:added');
        };

        Kudoable.prototype.unkudo = function (event) {
            event.preventDefault();
            if (this.isKudod()) {
                this.decrementCount();
                __removeClass(this.element, 'complete');
                __customTrigger(this.element, 'kudo:removed');
            }
        };

        Kudoable.prototype.setCount = function (count) {
            return this.counter.innerHTML = count;
        };

        Kudoable.prototype.currentCount = function () {
            return parseInt(this.counter.innerHTML);
        };

        Kudoable.prototype.incrementCount = function () {
            return this.setCount(this.currentCount() + 1);
        };

        Kudoable.prototype.decrementCount = function () {
            return this.setCount(this.currentCount() - 1);
        };

        return Kudoable;

    })();

    return Kudoable;
});