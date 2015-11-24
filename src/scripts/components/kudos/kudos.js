/*global define */

define(['knockout', 'jquery', 'text!./kudos.html'], function (ko, $, template) {
    'use strict';

    function Kudos(params) {
        this.id = ko.observable(ko.utils.unwrapObservable(params.id));


    }

    Kudos.prototype = {
        render: function(elements, context){
            context.element = $('[data-kudo-id=\"' + context.id() + '\"]');
            context.unkudo = $.proxy(context.unkudo, context);
            context.complete = $.proxy(context.complete, context);
            context.end = $.proxy(context.end, context);
            context.start = $.proxy(context.start, context);
            context.bindEvents();
            context.counter = $('.count .num', context.element);
            context.element.data('kudoable', context);
        },
        isKudoable: function(){
             return this.element.hasClass('kudoable');
        },
        isKudod: function(){
            return this.element.hasClass('complete');
        },
        unkudo: function (e) {
            e.preventDefault();
            if (this.isKudod()) {
                this.decrementCount();
                this.element.removeClass('complete');
                this.element.trigger('kudo:removed');
            }
        },
        setCount: function (count) {
            return this.counter.html(count);
        },
        currentCount: function () {
            return parseInt(this.counter.html());
        },
        incrementCount: function () {
            return this.setCount(this.currentCount() + 1);
        },
        decrementCount: function () {
            return this.setCount(this.currentCount() - 1);
        },
        dispose: function () {
            $(this.element).off('mouseenter.kudos');
            $(this.element).off('mouseleave.kudos');
            $(this.element).off('click.kudos');
            $(this.element).off('touchstart.kudos');
            $(this.element).off('touchend.kudos');
        }
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

    return {
        viewModel: Kudos,
        template: template
    };
});
