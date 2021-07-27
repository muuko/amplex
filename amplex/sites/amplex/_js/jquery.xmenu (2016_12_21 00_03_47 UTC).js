(function($) {

    $.fn.xmenu = function(options) {
        var settings = $.extend({
            menuselector: 'ul'
        }, options || {});

        return this.each(function() {
            var $menu = $(settings.menuselector, $(this));
            $menu.hide();

            $(this).hover(function() {
                $menu.show();
            }, function() {
                $menu.hide();
            });
        });
    };
    

})(jQuery);