define(['jquery', 'backbone', 'modules/views/sidebar', 'modules/views/board'],
    function ($, Backbone, SideBarView, BoardView) {
        var AppView = Backbone.View.extend({
            el: '#app',

            initialize: function (options) {
                this.options = options || {};

                this.Sidebar = new SideBarView();
                this.Board = new BoardView();
            },

            render: function () {
                this.Sidebar.render();
                this.Board.render();

                return this;
            },

            destroy: function () {
                if (this.Sidebar) {
                    this.Sidebar.destroy();
                }
            }
        });

        return AppView;
    });