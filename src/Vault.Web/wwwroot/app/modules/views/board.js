define(['jquery', 'backbone', 'jquery-ui/widgets/autocomplete', 'jquery-ui/widgets/menu'], function ($, Backbone) {
    var templateSettings = {
        evaluate: /{{=([\s\S]+?)}}/g,
        interpolate: /{{([\s\S]+?)}}/g,
        escape: /{{-([\s\S]+?)}}/g
    };

    var BoardView = Backbone.View.extend({
        template: {
            AudioCard: _.template($('#audio-card-template').html(), templateSettings),
            ArticleCard: _.template($('#article-card-template').html(), templateSettings),
            EventCard: _.template($('#event-card-template').html(), templateSettings),
            PlaceCard: _.template($('#place-card-template').html(), templateSettings)
        },

        initialize: function (options) {
            this.options = options || {};
        },

        render: function () {
            var $board = this;
            $(function () {
                $('.media-player').audio();
                $('.app-header, .draw-menu-navbar').sticky();
            });

            $(".js-search-form input[type=search]").each(function () {
                var $this = $(this);
                var remoteUrl = $this.closest('form').attr('data-url');
                $this.autocomplete({
                    source: remoteUrl
                })
                .autocomplete("instance")._renderItem = function (ul, item) {
                    if ($board.template[item.type](item.card)) {
                        var markup = $board.template[item.type](item.card);
                        return $('<li>').html(markup).appendTo(ul);
                    }

                    return $("<li>").html(item.label + '<div class="pull-right"><button class="btn" title="add to board" onclick="console.log(5);return false;"><i class="fa fa-plus-circle"></i></button></div>')
                      .appendTo(ul);
                };

                $this.autocomplete("instance")._resizeMenu = function () {
                    var ul = this.menu.element;
                    ul.outerWidth($this.closest('.js-search-form').outerWidth());
                };
            });

            return this;
        }
    });

    return BoardView;
});