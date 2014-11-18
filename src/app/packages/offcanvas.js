/*global define */

define(['jquery'], function ($) {
    'use strict';

    +function ($) {

        var requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame || window.msRequestAnimationFrame,
            cancelAnimationFrame = window.cancelAnimationFrame || window.mozCancelAnimationFrame;

        if (requestAnimationFrame === null) {
            requestAnimationFrame = function (fn) {
                return setTimeout(fn, 50);
            };
            cancelAnimationFrame = function (id) {
                return clearTimeout(id);
            };
        }

        // OFFCANVAS PUBLIC CLASS DEFINITION
        // =================================

        var OffCanvas = function (element, options) {
            this.$element = $(element);
            this.options = $.extend({}, OffCanvas.DEFAULTS, options);
            this.state = null;
            this.placement = null;

            if (this.options.recalc) {
                this.calcClone();
                $(window).on('resize', $.proxy(this.recalc, this));
            }

            if (this.options.autohide && !this.options.modal) {
                if ('ontouchstart' in window) {
                    $(document).on('touchstart', $.proxy(this.autohide, this));
                    $(document).on('touchend', $.proxy(this.autohide, this));
                }
                $(document).on('click', $.proxy(this.autohide, this));
            }

            if (this.options.toggle) {
                this.toggle();
            }

            if (this.options.disablescrolling) {
                this.options.disableScrolling = this.options.disablescrolling;
                delete this.options.disablescrolling;
            }
        };

        OffCanvas.DEFAULTS = {
            toggle: true,
            placement: 'auto',
            autohide: true,
            recalc: true,
            disableScrolling: true,
            modal: false
        };

        OffCanvas.prototype.offset = function () {
            switch (this.placement) {
                case 'left':
                case 'right':
                    return this.$element.outerWidth();
                case 'top':
                case 'bottom':
                    return this.$element.outerHeight();
            }
        };

        OffCanvas.prototype.calcPlacement = function () {
            if (this.options.placement !== 'auto') {
                this.placement = this.options.placement;
                return;
            }

            if (!this.$element.hasClass('in')) {
                this.$element.css('visiblity', 'hidden !important').addClass('in');
            }

            var horizontal = $(window).width() / this.$element.width(),
                vertical = $(window).height() / this.$element.height(),
                element = this.$element;

            function ab(a, b) {
                if (element.css(b) === 'auto') {
                    return a;
                }
                if (element.css(a) === 'auto') {
                    return b;
                }

                var size_a = parseInt(element.css(a), 10),
                    size_b = parseInt(element.css(b), 10);

                return size_a > size_b ? b : a;
            }

            this.placement = horizontal >= vertical ? ab('left', 'right') : ab('top', 'bottom');

            if (this.$element.css('visibility') === 'hidden !important') {
                this.$element.removeClass('in').css('visiblity', '');
            }
        };

        OffCanvas.prototype.opposite = function (placement) {
            switch (placement) {
                case 'top':
                    return 'bottom';
                case 'left':
                    return 'right';
                case 'bottom':
                    return 'top';
                case 'right':
                    return 'left';
            }
        };

        OffCanvas.prototype.getCanvasElements = function () {
            // Return a set containing the canvas plus all fixed elements
            var canvas = this.options.canvas ? $(this.options.canvas) : this.$element,
                fixed_elements = canvas.find('*').filter(function () {
                    return $(this).css('position') === 'fixed';
                }).not(this.options.exclude);

            return canvas.add(fixed_elements);
        };

        OffCanvas.prototype.slide = function (elements, offset, callback) {
            // Use jQuery animation if CSS transitions aren't supported
            if (!$.support.transition) {
                var anim = {};
                anim[this.placement] = "+=" + offset;
                return elements.animate(anim, 350, callback);
            }

            var placement = this.placement,
                opposite = this.opposite(placement);

            elements.each(function () {
                if ($(this).css(placement) !== 'auto') {
                    $(this).css(placement, (parseInt($(this).css(placement), 10) || 0) + offset);
                }

                if ($(this).css(opposite) !== 'auto') {
                    $(this).css(opposite, (parseInt($(this).css(opposite), 10) || 0) - offset);
                }
            });

            this.$element
                .one($.support.transition.end, callback);
            if (!$.support.transition) {
                this.$element.emulateTransitionEnd(5000);
            }
        };

        OffCanvas.prototype.disableScrolling = function () {
            var bodyWidth = $('body').width(),
                prop = 'padding-right',
                padding;

            if ($('body').data('offcanvas-style') === undefined) {
                $('body').data('offcanvas-style', $('body').attr('style') || '');
            }

            $('body').css('overflow', 'hidden');

            if ($('body').width() > bodyWidth) {
                padding = parseInt($('body').css(prop), 10) + $('body').width() - bodyWidth;

                requestAnimationFrame(function () {
                    $('body').css(prop, padding);
                }, 1);
            }
            //disable scrolling on mobiles (they ignore overflow:hidden)
            $('body').on('touchmove.bs', function (e) {
                if (!$(event.target).closest('.offcanvas').length) {
                    e.preventDefault();
                }
            });
        };

        OffCanvas.prototype.enableScrolling = function () {
            $('body').off('touchmove.bs');
        };

        OffCanvas.prototype.show = function () {
            if (this.state) {
                return;
            }

            var startEvent = $.Event('show.bs.offcanvas');
            this.$element.trigger(startEvent);
            if (startEvent.isDefaultPrevented()) {
                return;
            }

            $(document.body).append('<div class="offcanvas-cover"></div>');

            this.state = 'slide-in';
            this.calcPlacement();

            var elements = this.getCanvasElements(),
                placement = this.placement,
                opposite = this.opposite(placement),
                offset = this.offset(),
                complete = function () {
                    if (this.state != 'slide-in') {
                        return;
                    }

                    this.state = 'slid';

                    elements.removeClass('canvas-sliding').addClass('canvas-slid');
                    this.$element.trigger('shown.bs.offcanvas');
                };

            if (elements.index(this.$element) !== -1) {
                $(this.$element).data('offcanvas-style', $(this.$element).attr('style') || '');
                this.$element.css(placement, -1 * offset);
                this.$element.css(placement); // Workaround: Need to get the CSS property for it to be applied before the next line of code
            }

            elements.addClass('canvas-sliding').each(function () {
                if ($(this).data('offcanvas-style') === undefined) {
                    $(this).data('offcanvas-style', $(this).attr('style') || '');
                }
                if ($(this).css('position') === 'static') {
                    $(this).css('position', 'relative');
                }
                if (($(this).css(placement) === 'auto' || $(this).css(placement) === '0px') &&
                    ($(this).css(opposite) === 'auto' || $(this).css(opposite) === '0px')) {
                    $(this).css(placement, 0);
                }
            });

            if (this.options.disableScrolling) {
                this.disableScrolling();
            }
            if (this.options.modal) {
                this.toggleBackdrop();
            }

            requestAnimationFrame($.proxy(function () {
                this.$element.addClass('in')
                this.slide(elements, offset, $.proxy(complete, this))
            }, this), 1)
        };

        OffCanvas.prototype.hide = function (fast) {
            if (this.state !== 'slid') return;

            var startEvent = $.Event('hide.bs.offcanvas'),
                elements = $('.canvas-slid'),
                placement = this.placement,
                offset = -1 * this.offset();
            this.$element.trigger(startEvent);
            if (startEvent.isDefaultPrevented()) return;

            this.state = 'slide-out';

            var complete = function () {
                if (this.state != 'slide-out') return;

                this.state = null;
                this.placement = null;

                this.$element.removeClass('in');

                elements.removeClass('canvas-sliding')
                elements.add(this.$element).add('body').each(function () {
                    $(this).attr('style', $(this).data('offcanvas-style')).removeData('offcanvas-style');
                })

                this.$element.trigger('hidden.bs.offcanvas');

                $('.offcanvas-cover', document.body).remove();
            }

            if (this.options.disableScrolling) this.enableScrolling();
            if (this.options.modal) this.toggleBackdrop();

            elements.removeClass('canvas-slid').addClass('canvas-sliding');

            requestAnimationFrame($.proxy(function () {
                this.slide(elements, offset, $.proxy(complete, this));
            }, this), 1)

        };

        OffCanvas.prototype.toggle = function () {
            if (this.state === 'slide-in' || this.state === 'slide-out') return;
            this[this.state === 'slid' ? 'hide' : 'show']();
        };

        OffCanvas.prototype.toggleBackdrop = function (callback) {
            callback = callback || $.noop;
            if (this.state == 'slide-in') {
                var doAnimate = $.support.transition;

                this.$backdrop = $('<div class="modal-backdrop fade" />')
                    .insertAfter(this.$element);

                if (doAnimate) this.$backdrop[0].offsetWidth; // force reflow

                this.$backdrop.addClass('in');
                this.$backdrop.on('click.bs', $.proxy(this.autohide, this));

                doAnimate ?
                    this.$backdrop
                    .one($.support.transition.end, callback) :
                callback()
            } else if (this.state == 'slide-out' && this.$backdrop) {
                this.$backdrop.removeClass('in');
                $('body').off('touchmove.bs');
                var self = this;
                if ($.support.transition) {
                    this.$backdrop
                        .one($.support.transition.end, function () {
                            self.$backdrop.remove();
                            callback()
                            self.$backdrop = null;
                        });
                } else {
                    this.$backdrop.remove();
                    this.$backdrop = null;
                    callback();
                }
            } else if (callback) {
                callback();
            }
        };

        OffCanvas.prototype.calcClone = function () {
            this.$calcClone = this.$element.clone()
                .html('')
                .addClass('offcanvas-clone').removeClass('in')
                .appendTo($('body'));
        };

        OffCanvas.prototype.recalc = function () {
            if (this.$calcClone.css('display') === 'none' || (this.state !== 'slid' && this.state !== 'slide-in')) return;

            this.state = null;
            this.placement = null;
            var elements = this.getCanvasElements();

            this.$element.removeClass('in');

            elements.removeClass('canvas-slid');
            elements.add(this.$element).add('body').each(function () {
                $(this).attr('style', $(this).data('offcanvas-style')).removeData('offcanvas-style');
            });
        };

        OffCanvas.prototype.autohide = function (e) {
            if ($(e.target).closest(this.$element).length === 0) {
                this.hide();
            }
        };

        // OFFCANVAS PLUGIN DEFINITION
        // ==========================

        var old = $.fn.offcanvas;

        $.fn.offcanvas = function (option) {
            return this.each(function () {
                var $this = $(this);
                var data = $this.data('bs.offcanvas');
                var options = $.extend({}, OffCanvas.DEFAULTS, $this.data(), typeof option === 'object' && option);

                if (!data) $this.data('bs.offcanvas', (data = new OffCanvas(this, options)))
                if (typeof option === 'string') data[option]()
            })
        }

        $.fn.offcanvas.Constructor = OffCanvas


        // OFFCANVAS NO CONFLICT
        // ====================

        $.fn.offcanvas.noConflict = function () {
            $.fn.offcanvas = old
            return this
        }


        // OFFCANVAS DATA-API
        // =================

        $(document).on('click.bs.offcanvas.data-api', '[data-toggle=offcanvas]', function (e) {
            var $this = $(this),
                href;
            var target = $this.attr('data-target') || e.preventDefault() || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, ''); //strip for ie7
            var $canvas = $(target);
            var data = $canvas.data('bs.offcanvas');
            var option = data ? 'toggle' : $this.data();

            if (data) data.toggle();
            else $canvas.offcanvas(option);
        })

    }($);
});
