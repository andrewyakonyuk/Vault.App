/* Global define */

define(['pace',
        'jquery',
        'knockout',
        './router',
        'knockout-projections',
        'knockout-validation',
        './ko.bindinghandlers',
       'bootstrap',
       'bootstrap-material-design'],
       function (Pace, $, ko, router) {
    'use strict';

    window.ko = ko;

    Pace.start({
        restartOnPushState: false
    });

    if(window.addEventListener){
            window.addEventListener('hashchange', function(){
                if(Pace.options.restartOnPushState){
                    Pace.restart();
                }
            }, false);
        } else if (window.attachEvent){
            window.attachEvent('onhashchange', function(){
                if(Pace.options.restartOnPushState){
                    Pace.restart();
                }
            });
        }

    // Components can be packaged as AMD modules, such as the following:
    ko.components.register('nav-bar', { require: 'components/nav-bar/nav-bar' });
    ko.components.register('home-page', { require: 'components/home-page/home' });
    ko.components.register('signin-page', { require: 'components/signin-page/signin' });
    ko.components.register('flow-page', { require: 'components/flow-page/flow-page' });
    ko.components.register('flow-item-page', { require: 'components/flow-item-page/flow-item-page' });
    ko.components.register('register-page', { require: 'components/register-page/register-page' });

    // ... or for template-only components, you can just point to a .html file directly:
    ko.components.register('about-page', {
        template: { require: 'text!components/about-page/about.html' }
    });

    // [Scaffolded component registrations will be inserted here. To retain this feature, don't remove this comment.]

    // Start the application
    ko.applyBindings({ route: router.currentRoute });
});


