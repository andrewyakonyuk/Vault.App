/*global define, console */
/*jslint nomen: true*/

define(["knockout",
        'jquery',
        "underscore",
        "pace",
        "hammer",
        "text!./item-page.html",
        'packages/side-comments/side-comments',
       'packages/kudos'], function (ko, $, _, Pace, Hammer, templateMarkup, sc, kudoable) {
    'use strict';

    function FlowItemViewModel(route) {
        var self = this,
            user = {
                id: 1,
                avatarUrl: "http://f.cl.ly/items/0s1a0q1y2Z2k2I193k1y/default-user.png",
                name: "You"
            },
            initializedComments = 0;

        Pace.restart();

        this.Hammer = new Hammer(document.getElementById('flowItemContainer'));
        this.Hammer.on("swipe", function () {
            history.go(-1);
        })

        this.title = ko.observable("Item");

        this.showRestorePopup = ko.observable(false);

        //todo: load article from api
        this.article = {
            title: "SideComments.js in Action",
            sections: [{
                    id: 1,
                    content: "Each paragraph tag has the \"commentable-section\" class, making it a section which can be commented on after you've initialized a new SideComments object and pointed it at the parent element, which is \"#commentable-container\" for this demo.",
                    comments: [
                        {
                            id: 1,
                            authorAvatarUrl: "http://f.cl.ly/items/1W303Y360b260u3v1P0T/jon_snow_small.png",
                            authorName: "Jon Sno",
                            comment: "I'm Ned Stark's bastard. Related: I know nothing."
                    },
                        {
                            id: 2,
                            "authorAvatarUrl": "http://f.cl.ly/items/2o1a3d2f051L0V0q1p19/donald_draper.png",
                            "authorName": "Donald Draper",
                            "comment": "I need a scotch."
                    }]
            },
                {
                    id: 2,
                    content: "Hover over each section and you'll notice a little \"comment bubble\" pop up. Clicking on the markers on the right will show the SideComments. Sections without any comments only show their marker on hover.",
                    comments: []
                },
                {
                    id: 3,
                    content: "This is the default theme that comes with SideComments.js. You can easily theme SideComments to your liking by not including \"default-theme.css\" and just styling it all yourself.",
                    comments: [{
                        id: 3,
                        authorAvatarUrl: "http://f.cl.ly/items/0l1j230k080S0N1P0M3e/clay-davis.png",
                        authorName: "Senator Clay Davis",
                        comment: "These Side Comments are incredible. Sssshhhiiiiieeeee."
                    }]
                }
                ]
        };

        var existingComments = _.toArray(_.map(this.article.sections, function (item) {
            return {
                "sectionId": String(item.id),
                "comments": _.toArray(item.comments)
            };
        }));

        this.lastDeletedComment = null;

        this.initComments = function () {

            if (initializedComments === existingComments.length - 1) {

                initializedComments = 0;

                self.sideComments = new sc.SideComments('#commentable-area', user, existingComments);

                self.sideComments.on('commentPosted', function (comment) {
                    //TODO: for testing purpose
                    comment.id = self.sideComments.existingComments.length + 1;
                    self.sideComments.insertComment(comment);
                    $.ajax({
                        url: '/comments',
                        type: 'POST',
                        data: comment,
                        success: function (savedComment) {
                            // Once the comment is saved, you can insert the comment into the comment stream with "insertComment(comment)".
                            // self.sideComments.insertComment(savedComment);
                        }
                    });
                });

                // Listen to "commentDeleted" and send a request to your backend to delete the comment.
                // More about this event in the "docs" section.
                self.sideComments.on('commentDeleted', function (comment) {
                    if (comment) {
                        self.sideComments.removeComment(comment.sectionId, comment.id);
                        self.lastDeletedComment = comment;
                        self.showRestorePopup(true);
                        setTimeout(function () {
                            self.showRestorePopup(false);
                        }, 10000);
                    }
                    $.ajax({
                        url: '/comments/' + comment.id,
                        type: 'DELETE',
                        success: function (success) {
                            // Do something.
                        }
                    });
                });
            }
            initializedComments += 1;
        };

        // initialize kudos
        
        Array.prototype.forEach.call(document.querySelectorAll("figure.kudoable"), function (item, i) {
            new kudoable(item);
        });

        //kudoable(docu)
        //$("figure.kudoable").kudoable();


        //// when kudoing
        //$("figure.kudo").bind("kudo:active", function (e) {
        //    console.log("kudoing active");
        //});

        //// when not kudoing
        //$("figure.kudo").bind("kudo:inactive", function (e) {
        //    console.log("kudoing inactive");
        //});

        //// after kudo'd
        //$("figure.kudo").bind("kudo:added", function (e) {
        //    var element = $(this);
        //    // ajax'y stuff or whatever you want
        //    console.log("Kodo'd:", element.data('id'), ":)");
        //});

        //// after removing a kudo
        //$("figure.kudo").bind("kudo:removed", function (e) {
        //    var element = $(this);
        //    // ajax'y stuff or whatever you want
        //    console.log("Un-Kudo'd:", element.data('id'), ":(");
        //});
    }

    FlowItemViewModel.prototype.initUI = function () {
        console.log('init ui');
        $(window).scroll(function () {
            if ($(window).scrollTop() === $(document).height() - $(window).height()) {
                // ajax call get data from server and append to the div
                console.log('load more data');
            }
        });
    };

    FlowItemViewModel.prototype.restoreComment = function () {
        if (this.lastDeletedComment) {
            this.sideComments.insertComment(this.lastDeletedComment);
        }
        this.showRestorePopup(false);
    };

    FlowItemViewModel.prototype.closeRestorePopup = function (e) {
        this.showRestorePopup(false);
    };

    FlowItemViewModel.prototype.dispose = function () {
        if (this.sideComments) {
            this.sideComments.destroy();
            this.sideComments = null;
        }
        if (this.Hammer) {
            this.Hammer.destroy();
        }
    };

    return {
        viewModel: FlowItemViewModel,
        template: templateMarkup
    };

});
