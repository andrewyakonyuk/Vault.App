/*global define */
define(['jquery', 'jquery-ui/widget'], function($) {
    'use strict'
    
   return $.widget('cards.audio', {
       _playRunner: null,
       _percentage: null,
       _songLength: 219, //todo: currently its a constant but shouldn`t
       _create: function() {
            this.element.addClass('ui-state-paused');
            $('<div>').addClass('runner')
                .appendTo($('<div>').addClass('progress-bar')
                    .appendTo(this.element));
            
            if(this.options.disabled) {
                this.element
                    .addClass('ui-state-disabled')
                    .attr('aria-disabled', "true");
            }
            
            this._on({
               'click .play-button': function(event){
                    this.element.toggleClass('ui-state-paused').toggleClass('ui-state-playing');
                    if (this._playRunner) {
                        clearInterval(this._playRunner);
                        this._playRunner = null;
                        this.element.find('.time').text(this._calculateTime(this._songLength, 100));
                    } else {
                        this._percentage = 0;
                        this._go();
                    }
                    
                    if(this.element.hasClass('ui-state-paused')){
                        this.element.find('.runner').css('width', '0%');
                    }
               },
               'click .progress-bar': function(event){
                    var posY = $(event.target).offset().left;
                    var clickY = event.pageX - posY;
                    var width = $(event.target).width();

                    this._percentage = clickY / width * 100;
               } 
            });
            
            clearInterval(this._playRunner);
       },
       _calculateTime: function (songLength, percentage) {
            var currentLength = songLength / 100 * percentage;
            var minutes = Math.floor(currentLength / 60);
            var seconds = Math.floor(currentLength - (minutes * 60));
            if (seconds <= 9) {
                return (minutes + ':0' + seconds);
            } else {
                return (minutes + ':' + seconds);
            }
        },
        _go: function () {
            var self = this;
            this._playRunner = setInterval(function() {
                //progress bar
                self._percentage += 0.15;
                if (self._percentage > 100) {
                    self._percentage = 0;
                }
                self.element.find('.runner')
                    .css('width', self._percentage + '%');

                self.element.find('.time')
                    .text(self._calculateTime(self._songLength, self._percentage));
            }, 250);
        } 
    });
});