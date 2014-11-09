define(['jquery', 'underscore', './mobile-check', './emitter', 'text!./templates/comment.html', 'text!./templates/section.html'],
    function (jQuery, _, mobileCheck, Emitter, CommentTemplate, SectionTemplate) {

        /**
         * Creates a new SideComments instance.
         * @param {Object} el               The selector for the element for
         *                                  which side comments need to be initialized
         * @param {Object} currentUser      An object defining the current user. Used
         *                                  for posting new comments and deciding
         *                                  whether existing ones can be deleted
         *                                  or not.
         * @param {Array} existingComments An array of existing comments, in
         *                                 the proper structure.
         *
         * TODO: **GIVE EXAMPLE OF STRUCTURE HERE***
         */
        function SideComments(el, currentUser, existingComments) {
            this.$el = $(el);
            this.$body = $('body');
            this.eventPipe = new Emitter;

            this.currentUser = _.clone(currentUser) || null;
            this.existingComments = _.clone(existingComments) || [];
            this.sections = [];
            this.activeSection = null;

            // Event bindings
            this.eventPipe.on('showComments', _.bind(this.showComments, this));
            this.eventPipe.on('hideComments', _.bind(this.hideComments, this));
            this.eventPipe.on('sectionSelected', _.bind(this.sectionSelected, this));
            this.eventPipe.on('sectionDeselected', _.bind(this.sectionDeselected, this));
            this.eventPipe.on('commentPosted', _.bind(this.commentPosted, this));
            this.eventPipe.on('commentDeleted', _.bind(this.commentDeleted, this));
            this.eventPipe.on('addCommentAttempted', _.bind(this.addCommentAttempted, this));
            this.$body.on('click', _.bind(this.bodyClick, this));
            this.initialize(this.existingComments);
        }

        // Mix in Emitter
        Emitter(SideComments.prototype);

        /**
         * Adds the comments beside each commentable section.
         */
        SideComments.prototype.initialize = function (existingComments) {
            _.each(this.$el.find('.commentable-section'), function (section) {
                var $section = $(section);
                var sectionId = $section.data('section-id').toString();
                var sectionComments = _.find(this.existingComments, {
                    sectionId: sectionId
                });

                this.sections.push(new Section(this.eventPipe, $section, this.currentUser, sectionComments));
            }, this);
        };

        /**
         * Shows the side comments.
         */
        SideComments.prototype.showComments = function () {
            this.$el.addClass('side-comments-open');
        };

        /**
         * Hide the comments.
         */
        SideComments.prototype.hideComments = function () {
            if (this.activeSection) {
                this.activeSection.deselect();
                this.activeSection = null;
            }

            this.$el.removeClass('side-comments-open');
        };

        /**
         * Callback after a section has been selected.
         * @param  {Object} section The Section object to be selected.
         */
        SideComments.prototype.sectionSelected = function (section) {
            this.showComments();

            if (this.activeSection) {
                this.activeSection.deselect();
            }

            this.activeSection = section;
        };

        /**
         * Callback after a section has been deselected.
         * @param  {Object} section The Section object to be selected.
         */
        SideComments.prototype.sectionDeselected = function (section) {
            this.hideComments();
            this.activeSection = null;
        };

        /**
         * Fired when the commentPosted event is triggered.
         * @param  {Object} comment  The comment object to be posted.
         */
        SideComments.prototype.commentPosted = function (comment) {
            this.emit('commentPosted', comment);
        };

        /**
         * Fired when the commentDeleted event is triggered.
         * @param  {Object} comment  The commentId of the deleted comment.
         */
        SideComments.prototype.commentDeleted = function (comment) {
            this.emit('commentDeleted', comment);
        };

        /**
         * Fire an event to to signal that a comment as attempted to be added without
         * a currentUser.
         */
        SideComments.prototype.addCommentAttempted = function () {
            this.emit('addCommentAttempted');
        };

        /**
         * Inserts the given comment into the right section.
         * @param  {Object} comment A comment to be inserted.
         */
        SideComments.prototype.insertComment = function (comment) {
            var section = _.find(this.sections, {
                id: comment.sectionId
            });
            section.insertComment(comment);
        };

        /**
         * Removes the given comment from the right section.
         * @param sectionId The ID of the section where the comment exists.
         * @param commentId The ID of the comment to be removed.
         */
        SideComments.prototype.removeComment = function (sectionId, commentId) {
            var section = _.find(this.sections, {
                id: sectionId
            });
            section.removeComment(commentId);
        };

        /**
         * Delete the comment specified by the given sectionID and commentID.
         * @param sectionId The section the comment belongs to.
         * @param commentId The comment's ID
         */
        SideComments.prototype.deleteComment = function (sectionId, commentId) {
            var section = _.find(this.sections, {
                id: sectionId
            });
            section.deleteComment(commentId);
        };

        /**
         * Checks if comments are visible or not.
         * @return {Boolean} Whether or not the comments are visible.
         */
        SideComments.prototype.commentsAreVisible = function () {
            return this.$el.hasClass('side-comments-open');
        };

        /**
         * Callback for body clicks. We hide the comments if someone clicks outside of the comments section.
         * @param  {Object} event The event object.
         */
        SideComments.prototype.bodyClick = function (event) {
            var $target = $(event.target);

            // We do a check on $('body') existing here because if the $target has
            // no parent body then it's because it belongs to a deleted comment and
            // we should NOT hide the SideComments.
            if ($target.closest('.side-comment').length < 1 && $target.closest('body').length > 0) {
                if (this.activeSection) {
                    this.activeSection.deselect();
                }
                this.hideComments();
            }
        };

        /**
         * Set the currentUser and update the UI as necessary.
         * @param {Object} currentUser The currentUser to be used.
         */
        SideComments.prototype.setCurrentUser = function (currentUser) {
            this.hideComments();
            this.currentUser = currentUser;
            _.each(this.sections, function (section) {
                section.currentUser = this.currentUser;
                section.render();
            });
        };

        /**
         * Remove the currentUser and update the UI as necessary.
         */
        SideComments.prototype.removeCurrentUser = function () {
            this.hideComments();
            this.currentUser = null;
            _.each(this.sections, function (section) {
                section.currentUser = null;
                section.render();
            });
        };

        /**
         * Destroys the instance of SideComments, including unbinding from DOM events.
         */
        SideComments.prototype.destroy = function () {
            this.hideComments();
            this.$el.off();
        };

        /**
         * Creates a new Section object, which is responsible for managing a
         * single comment section.
         * @param {Object} eventPipe The Emitter object used for passing around events.
         * @param {Array} comments   The array of comments for this section. Optional.
         */
        function Section(eventPipe, $el, currentUser, comments) {
            this.eventPipe = eventPipe;
            this.$el = $el;
            this.comments = comments ? comments.comments : [];
            this.currentUser = currentUser || null;
            this.id = $el.data('section-id');
            this.render();
            this.initEventHandlers();

            //make textarea to grow its height while you are entering more lines of text
            new boilerplate.autogrow(this.$el.find('.comment-box').get(0), 14);
        }

        Section.prototype.initEventHandlers = function(){
            var self = this;

            _.each(jQuery('.side-comment .marker', self.$el), function(element){
                new boilerplate.fastButton(element, _.bind(self.markerClick, self));
            });

            _.each(jQuery('.side-comment .add-comment', self.$el), function(element){
                new boilerplate.fastButton(element, _.bind(self.addCommentClick, self));
            });

            _.each(jQuery('.side-comment .post', self.$el), function(element){
                new boilerplate.fastButton(element, _.bind(self.postCommentClick, self));
            });

            _.each(jQuery('.side-comment .cancel', self.$el), function(element){
                new boilerplate.fastButton(element, _.bind(self.cancelCommentClick, self));
            });

            _.each(jQuery('.side-comment .delete', self.$el), function(element){
                new boilerplate.fastButton(element, _.bind(self.deleteCommentClick, self));
            });
        };

        /**
         * Click callback event on markers.
         * @param  {Object} event The event object.
         */
        Section.prototype.markerClick = function (event) {
            console.log('marker click');
            event.preventDefault();
            this.select();
        };

        /**
         * Callback for the comment button click event.
         * @param {Object} event The event object.
         */
        Section.prototype.addCommentClick = function (event) {
            event.preventDefault();
            if (this.currentUser) {
                this.showCommentForm();
            } else {
                this.eventPipe.emit('addCommentAttempted');
            }
        };

        /**
         * Show the comment form for this section.
         */
        Section.prototype.showCommentForm = function () {
            if (this.comments.length > 0) {
                this.$el.find('.add-comment').addClass('hide');
                this.$el.find('.comment-form').addClass('active');
            }

            this.focusCommentBox();
        };

        /**
         * Hides the comment form for this section.
         */
        Section.prototype.hideCommentForm = function () {
            if (this.comments.length > 0) {
                this.$el.find('.add-comment').removeClass('hide');
                this.$el.find('.comment-form').removeClass('active');
            }

            this.$el.find('.comment-box').empty();
        };

        /**
         * Focus on the comment box in the comment form.
         */
        Section.prototype.focusCommentBox = function () {
            // NOTE: !!HACK!! Using a timeout here because the autofocus causes a weird
            // "jump" in the form. It renders wider than it should be on screens under 768px
            // and then jumps to a smaller size.
            setTimeout(_.bind(function () {
                this.$el.find('.comment-box').get(0).focus();
            }, this), 300);
        };

        /**
         * Cancel comment callback.
         * @param  {Object} event The event object.
         */
        Section.prototype.cancelCommentClick = function (event) {
            event.preventDefault();
            this.cancelComment();
        };

        /**
         * Cancel adding of a comment.
         */
        Section.prototype.cancelComment = function () {
            if (this.comments.length > 0) {
                this.hideCommentForm();
            } else {
                this.deselect();
                this.eventPipe.emit('hideComments');
            }
        };

        /**
         * Post comment callback.
         * @param  {Object} event The event object.
         */
        Section.prototype.postCommentClick = function (event) {
            event.preventDefault();
            this.postComment();
        };

        /**
         * Post a comment to this section.
         */
        Section.prototype.postComment = function () {
            var $commentBox = this.$el.find('.comment-box');
            var commentBody = $commentBox.val();
            if(!commentBody)
                return;
            var comment = {
                sectionId: this.id,
                comment: commentBody,
                authorAvatarUrl: this.currentUser.avatarUrl,
                authorName: this.currentUser.name,
                authorId: this.currentUser.id,
                authorUrl: this.currentUser.authorUrl || null
            };
            $commentBox.val(''); // Clear the comment.
            this.eventPipe.emit('commentPosted', comment);
        };

        /**
         * Insert a comment into this sections comment list.
         * @param  {Object} comment A comment object.
         */
        Section.prototype.insertComment = function (comment) {
            this.comments.push(comment);
            var newCommentHtml = _.template(CommentTemplate)({
                comment: comment,
                currentUser: this.currentUser
            });
            this.$el.find('.comments').append(newCommentHtml);
            this.$el.find('.side-comment').addClass('has-comments');
            this.updateCommentCount();
            this.hideCommentForm();
        };

        /**
         * Increments the comment count for a given section.
         */
        Section.prototype.updateCommentCount = function () {
            this.$el.find('.marker span').text(this.comments.length);
        };

        /**
         * Event handler for delete comment clicks.
         * @param  {Object} event The event object.
         */
        Section.prototype.deleteCommentClick = function (event) {
            event.preventDefault();
            var commentId = $(event.target).closest('li').data('comment-id');
            this.deleteComment(commentId);
        };

        /**
         * Finds the comment and emits an event with the comment to be deleted.
         */
        Section.prototype.deleteComment = function (commentId) {
            var comment = _.find(this.comments, {
                id: commentId
            });
            if(comment){
                comment.sectionId = this.id;
                this.eventPipe.emit('commentDeleted', comment);
            }
        };

        /**
         * Removes the comment from the list of comments and the comment array.
         * @param commentId The ID of the comment to be removed from this section.
         */
        Section.prototype.removeComment = function (commentId) {
            this.comments = _.reject(this.comments, {
                id: commentId
            });
            this.$el.find('.side-comment .comments li[data-comment-id="' + commentId + '"]').remove();
            this.updateCommentCount();
            if (this.comments.length < 1) {
                this.$el.find('.side-comment').removeClass('has-comments');
            }
        };

        /**
         * Mark this section as selected. Delsect if this section is already selected.
         */
        Section.prototype.select = function () {
            if (this.isSelected()) {
                this.deselect();
                this.eventPipe.emit('sectionDeselected', this);
            } else {
                this.$el.find('.side-comment').addClass('active');

                if (this.comments.length === 0 && this.currentUser) {
                    this.focusCommentBox();
                }

                this.eventPipe.emit('sectionSelected', this);
            }
        };

        /**
         * Deselect this section.
         */
        Section.prototype.deselect = function () {
            this.$el.find('.side-comment').removeClass('active');
            this.hideCommentForm();
        };

        Section.prototype.isSelected = function () {
            return this.$el.find('.side-comment').hasClass('active');
        };

        /**
         * Get the class to be used on the side comment section wrapper.
         * @return {String} The class names to use.
         */
        Section.prototype.sectionClasses = function () {
            var classes = '';

            if (this.comments.length > 0) {
                classes = classes + ' has-comments';
            }
            if (!this.currentUser) {
                classes = classes + ' no-current-user'
            }

            return classes;
        };

        /**
         * Render this section into the DOM.
         */
        Section.prototype.render = function () {
            this.$el.find('.side-comment').remove();
            $(_.template(SectionTemplate)({
                commentTemplate: CommentTemplate,
                comments: this.comments,
                sectionClasses: this.sectionClasses(),
                currentUser: this.currentUser
            })).appendTo(this.$el);
        };

        /**
         * Desttroy this Section object. Generally meaning unbind events.
         */
        Section.prototype.destroy = function () {
            this.$el.off();
        }

        return {
            SideComments: SideComments,
            Section: Section
        };

    });
