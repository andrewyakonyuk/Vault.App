/*global define */
define(['jquery'], function($){
   'use strict'
   
    $.fn.sticky = function(){
        this.each(function(){
            var $this = $(this);
            var $window = $(window);
            var didScroll;
            var lastScrollTop = 0;
            var delta = 10;
            var navbarHeight = $this.outerHeight();

            $this.addClass('ui-sticky');

            $(window).scroll(function(event){
                didScroll = true;
            });

            setInterval(function() {
                if (didScroll) {
                    hasScrolled();
                    didScroll = false;
                }
            }, 250);

            function hasScrolled() {
                var st = $window.scrollTop();
                
                // Make sure they scroll more than delta
                if(Math.abs(lastScrollTop - st) <= delta)
                    return;
                
                // If they scrolled down and are past the navbar, add class .nav-up.
                // This is necessary so you never see what is "behind" the navbar.
                if (st > lastScrollTop && st > navbarHeight){
                    // Scroll Down
                    $this.removeClass('ui-state-move-down').addClass('ui-state-move-up');
                } else {
                    // Scroll Up
                    if(st + $(window).height() < $(document).height()) {
                        $this.removeClass('ui-state-move-up').addClass('ui-state-move-down');
                    }
                }
                lastScrollTop = st;
            }
        });
        
        return this;  
    };
});