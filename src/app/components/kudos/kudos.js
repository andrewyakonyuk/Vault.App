/*global define */

define(['knockout', 'jquery', 'text!./kudos.html'], function (ko, $, template) {
    'use strict';

    var __bind = function (fn, me) {
        return function () {
            return fn.apply(me, arguments);
        };
    };

    function Kudos(params) {
        this.id = ko.observable(ko.utils.unwrapObservable(params.id));
    }

    Kudos.prototype.render = function (elements, bindingContext) {
        bindingContext.element = $('[data-kudo-id=\"' + bindingContext.id() + '\"]');
        bindingContext.unkudo = __bind(bindingContext.unkudo, bindingContext);
        bindingContext.complete = __bind(bindingContext.complete, bindingContext);
        bindingContext.end = __bind(bindingContext.end, bindingContext);
        bindingContext.start = __bind(bindingContext.start, bindingContext);
        bindingContext.bindEvents();
        bindingContext.counter = $('.count .num', bindingContext.element);
        bindingContext.element.data('kudoable', bindingContext);
    };

    Kudos.prototype.bindEvents = function () {
        $(this.element).on('mouseenter.kudos', this.element, $.proxy(this.start, this));
        $(this.element).on('mouseleave.kudos', this.element, $.proxy(this.end, this));
        $(this.element).on('click.kudos', this.element, $.proxy(this.unkudo, this));
        $(this.element).on('touchstart.kudos', this.element, $.proxy(this.touchstart, this));
        $(this.element).on('touchend.kudos', this.element, $.proxy(this.end, this));
    };

    Kudos.prototype.isKudoable = function () {
       return this.element.hasClass('kudoable');
    };

    Kudos.prototype.isKudod = function () {
        return this.element.hasClass('complete');
    };

    Kudos.prototype.touchstart = function (e) {
        if (this.isKudoable() && !this.isKudod()) {
            this.start(e);
        } else if (this.isKudod()) {
            this.unkudo(e);
        }
    };

    Kudos.prototype.start = function (e) {
        if (this.isKudoable() && !this.isKudod()) {
            this.element.trigger('kudo:active');
            this.element.addClass('active');
            (this.timer = setTimeout(this.complete, 700));
        }
    };

    Kudos.prototype.end = function () {
        if (this.isKudoable() && !this.isKudod()) {
            this.element.trigger('kudo:inactive');
            this.element.removeClass('active');
            if (this.timer !== null) {
                clearTimeout(this.timer);
            }
        }
    };

    Kudos.prototype.complete = function () {
        this.end();
        this.incrementCount();
        this.element.addClass('complete');
        this.element.trigger('kudo:added');
    };

    Kudos.prototype.unkudo = function (event) {
        event.preventDefault();
        if (this.isKudod()) {
            this.decrementCount();
            this.element.removeClass('complete');
            this.element.trigger('kudo:removed');
        }
    };

    Kudos.prototype.setCount = function (count) {
        return this.counter.html(count);
    };

    Kudos.prototype.currentCount = function () {
        return parseInt(this.counter.html());
    };

    Kudos.prototype.incrementCount = function () {
        return this.setCount(this.currentCount() + 1);
    };

    Kudos.prototype.decrementCount = function () {
        return this.setCount(this.currentCount() - 1);
    };

    Kudos.prototype.dispose = function () {
        $(this.element).off('mouseenter.kudos');
        $(this.element).off('mouseleave.kudos');
        $(this.element).off('click.kudos');
        $(this.element).off('touchstart.kudos');
        $(this.element).off('touchend.kudos');
    };

    return {
        viewModel: Kudos,
        template: template
    };
});
