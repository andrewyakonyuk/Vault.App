 /**
* iOS Startup Image helper
 */
/*global define */

define([], function () {
    'use strict';

    var viewportmeta = document.querySelector && document.querySelector('meta[name="viewport"]'),
        startupImage = function () {
            var portrait,
                landscape,
                pixelRatio,
                head,
                link1,
                link2;

            pixelRatio = window.devicePixelRatio;
            head = document.getElementsByTagName('head')[0];

            if (navigator.platform === 'iPad') {
                portrait = pixelRatio === 2 ? 'img/startup/startup-tablet-portrait-retina.png' : 'img/startup/startup-tablet-portrait.png';
                landscape = pixelRatio === 2 ? 'img/startup/startup-tablet-landscape-retina.png' : 'img/startup/startup-tablet-landscape.png';

                link1 = document.createElement('link');
                link1.setAttribute('rel', 'apple-touch-startup-image');
                link1.setAttribute('media', 'screen and (orientation: portrait)');
                link1.setAttribute('href', portrait);
                head.appendChild(link1);

                link2 = document.createElement('link');
                link2.setAttribute('rel', 'apple-touch-startup-image');
                link2.setAttribute('media', 'screen and (orientation: landscape)');
                link2.setAttribute('href', landscape);
                head.appendChild(link2);
            } else {
                portrait = pixelRatio === 2 ? "img/startup/startup-retina.png" : "img/startup/startup.png";
                portrait = screen.height === 568 ? "img/startup/startup-retina-4in.png" : portrait;
                link1 = document.createElement('link');
                link1.setAttribute('rel', 'apple-touch-startup-image');
                link1.setAttribute('href', portrait);
                head.appendChild(link1);
            }

            //hack to fix letterboxed full screen web apps on 4" iPhone / iPod
            if ((navigator.platform === 'iPhone' || 'iPod') && (screen.height === 568)) {
                if (viewportmeta) {
                    viewportmeta.content = viewportmeta.content
                        .replace(/\bwidth\s*=\s*320\b/, 'width=320.1')
                        .replace(/\bwidth\s*=\s*device-width\b/, '');
                }
            }
        };

    return startupImage;
});
