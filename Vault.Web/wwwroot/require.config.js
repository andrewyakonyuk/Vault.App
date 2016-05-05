// require.js looks for the following global when initializing
var require = {
    baseUrl: "/",
    paths: {
        "jquery": "bower_modules/jquery/dist/jquery",
        "text": "bower_modules/requirejs-text/text",
        "underscore": "bower_modules/underscore/underscore",
        "backbone": "bower_modules/backbone/backbone",
        "pace": "bower_modules/pace/pace",
        "hammerjs": "bower_modules/hammerjs/hammer",
        "jquery-ui": "bower_modules/jquery-ui/ui",
        "jquery-validation": "bower_modules/jquery-validation/jquery.validate",
        "jquery-validation-unobtrusive": "bower_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive",
        "jquery-pjax": "bower_modules/jquery-pjax/jquery.pjax",

        "modules/models": "./app/modules/models",
        "modules/views": "./app/modules/views",
        "modules/collections": "./app/modules/collections",
        "plugins": "./app/plugins",
        "templates": "./app/templates",
        "widgets": "./app/widgets"
    },
    enforceDefine: false,
    waitSeconds: 0, //disable timeout
    shim: {
        "jquery-validation-unobtrusive": {
            deps: ["jquery-validation"]
        },
        "jquery-validation": {
            deps: ["jquery"]
        },
        "jquery-ui": {
            deps: ["jquery"]
        },
        "jquery-pjax": {
            deps: ["jquery"]
        }
    },
    locale: "en-us"
};