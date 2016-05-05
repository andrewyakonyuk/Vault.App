/// <binding ProjectOpened='watch' />
/*global require, console */
(function () {
    'use strict';

    // Node modules
    var fs = require('fs'),
        vm = require('vm'),
        merge = require('deeply'),
        chalk = require('chalk'),
        es = require('event-stream');

    // Gulp and plugins
    var gulp = require('gulp'),
        rjs = require('gulp-requirejs-bundler'),
        concat = require('gulp-concat'),
        clean = require('gulp-clean'),
        replace = require('gulp-replace'),
        uglify = require('gulp-uglify'),
        htmlreplace = require('gulp-html-replace'),
        minifyCSS = require('gulp-minify-css'),
        concatCss = require('gulp-concat-css'),
        less = require('gulp-less'),
        shell = require('gulp-shell'),
        //autoprefixer = require('gulp-autoprefixer'),
        path = require('path');

    // Config
    var requireJsRuntimeConfig = vm.runInNewContext(fs.readFileSync('wwwroot/require.config.js') + '; require;'),
        requireJsOptimizerConfig = merge(requireJsRuntimeConfig, {
            out: 'app-bundle.min.js',
            baseUrl: './wwwroot',
            name: 'app/startup',
            paths: {
                requireLib: 'bower_modules/requirejs/require'
            },
            include: [
                'requireLib'
            ],
            insertRequire: ['app/startup'],
            bundles: {
                // If you want parts of the site to load on demand, remove them from the 'include' list
                // above, and group them into bundles here.
                // 'bundle-name': [ 'some/module', 'another/module' ],
                // 'another-bundle-name': [ 'yet-another-module' ]
            }
        });

    // Discovers all AMD dependencies, concatenates together all required .js files, minifies them
    gulp.task('js', function () {
        return rjs(requireJsOptimizerConfig)
            .pipe(uglify({
                preserveComments: 'some'
            }))
            .pipe(gulp.dest('./wwwroot/app/'));
    });

    // Concatenates CSS files, rewrites relative paths to Font Awesome, copies Font Awesome fonts
    gulp.task('css', function () {
       return gulp.src('./wwwroot/styles/main.css')
            .pipe(replace(/url\((')?\.\.\/img\//g, 'url($1img/'))
           // .pipe(autoprefixer())
            .pipe(minifyCSS({}))
            .pipe(concat('main.min.css'))
            .pipe(gulp.dest('./wwwroot/styles'));
    });

    gulp.task('less', function () {
        return gulp.src('./wwwroot/styles/main.less')
          .pipe(less({
              paths: [path.join(__dirname, 'less', 'includes')]
          }))
          .pipe(gulp.dest('./wwwroot/styles'));
    });

    gulp.task('dnx', shell.task(['dnx-watch dev']));
    
    gulp.task('watch', ['less'], function() {
        gulp.watch('./wwwroot/styles/**/*.less', ['less']);  // Watch all the .less files, then run the less task
    });

    gulp.task('default', ['js','less', 'css'], function (callback) {
        callback();
        console.log('\nPlaced optimized files in ' + chalk.magenta('dist/\n'));
    });

})();
