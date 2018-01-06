define(['jquery', 'backbone', 'plugins/mpmenu', 'jquery-ui/widgets/accordion'], function($, Backbone, mpmenu){
   
   var SideBarView = Backbone.View.extend({
        el: '#sidebar',
        
        initialize: function(options){
            this.options = options || {};
        },
        
        render: function () {
            mpmenu();
            
            $(function(){
                $('.mp-menu-item.expand').accordion({
                    collapsible: true,
                    icons: false
                }); 
            });
        } 
   });
   
   return SideBarView;
    
});