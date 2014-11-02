// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "bootstrap":            "bower_modules/components-bootstrap/js/bootstrap.min",
        "crossroads":           "bower_modules/crossroads/dist/crossroads.min",
        "hasher":               "bower_modules/hasher/dist/js/hasher.min",
        "jquery":               "bower_modules/jquery/dist/jquery.min",
        "knockout":             "bower_modules/knockout/dist/knockout",
        "knockout-projections": "bower_modules/knockout-projections/dist/knockout-projections.min",
        "knockout-validation":  "bower_modules/knockout-validation/Dist/knockout.validation.min",
        "signals":              "bower_modules/js-signals/dist/signals.min",
        "text":                 "bower_modules/requirejs-text/text"
    },
    shim: {
        "jquery": {
            exports: '$'
        },
        "bootstrap": { deps: ["jquery"] },
        "knockout-validation": {
            deps: ["knockout"]
        }
    }
};
