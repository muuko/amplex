$.generateId = function() {
    return arguments.callee.prefix + arguments.callee.count++;
};
$.generateId.prefix = 'jmenu$';
$.generateId.count = 0;
$.fn.generateId = function() {
    return this.each(function() {
        this.id = $.generateId();
    });
};


$.generateUniqueClass = function() {
    return arguments.callee.prefix + arguments.callee.count++;
};
$.generateUniqueClass.prefix = 'jmenu-';
$.generateUniqueClass.count = 0;
$.fn.generateUniqueClass = function() {
    return this.each(function() {
        $(this).addClass( $.generateUniqueClass());
    });
};


function isHovered($el, hideIfNot) 
{
    var hovered = false;

    if ($el[0] != null) {
    
        // say('checking hovered for: ' + $el[0].id);

        // is this one hovered?
        var thisHovered = $el.data('hovered');
        // say('thisHovered?: ' + thisHovered);
        if (thisHovered != null) {
            if (thisHovered == true) {
                hovered = true;
            }
        }

        var $children = $el.children();
        $children.each(function(n, child) {
            var $child = $(child);
            // say('checking child:' + child.id);
            if ($child.is('li')) {
                var childHovered = isHovered($child, true);
                if (childHovered != null) {
                    if (childHovered == true) {
                        hovered = true;
                    }
                }
            }
        });

        // say('done iterating');
        
        // got a child popup?
        var childClass = $el.data('child-class');
        // say($el[0].id + ' has a popup?:' + childClass);
        if (childClass != undefined) {
            // say('not undefined, hovered:' + hovered + ', hide if not: ' + hideIfNot);

            // var $elChild = $('[' + $.expando + '=' + childId + ']');
            var $elChild = $('.' + childClass );
            // say('elchild:' + $elChild);
            if ($elChild != null) {

                var childHovered = isHovered($elChild, true);
                hovered = hovered || childHovered;

                if (!hovered && hideIfNot) {
                    $el.data('child-class', null);
                    // say('removing: ' + childId);
                    $elChild.remove();

                    //$elChild.hide();
                }
            }
        }
    }

    return hovered;
}

function evalHideMenu($el) {
    // say('-----------<<<<<<< eval hide menu >>>>>>----');
    evalHideChildren(true, $el);
}

function evalHideChildren(bDelay, $el) {
    if (bDelay == true) {
        var el = $el.data('unique-class');
        // say('hiding children of unique class:' + el + ', id: ' + $el[0].id);
        // alert('hiding unique-class: ' + el);

        window.setTimeout(function() {
            evalHideChildrenDelayed(el);
        }, 50);
    }
    else {
        isHovered($el, true); 
    }
}

function evalHideChildrenDelayed(el) {
    //var $el = $('[' + $.expando + '=' + el + ']');

    // say('delay complete, now hiding children of unique-class:' + el);

    var $el = $('.' + el);
    if ($el.length > 0) {
        // say('id of this element is: ' + $el[0].id);
        evalHideChildren(false, $el);
    }
    else {
        // say('element not found');
    }
}

$(function() {

    $('.jmenu ul').hide();
    $('.jmenu > ul').each(function() {
        var uniqueClass = $.generateUniqueClass()
        $(this).data('unique-class', uniqueClass);
        $(this).addClass(uniqueClass);
    });



    $('.jmenu').hover(function() {
        $('ul:first', $(this)).show();
    },
    function() {
        evalHideChildren(false, $(this));
        $('ul:first', $(this)).hide();
    });



    $('.jmenu li').live(
        'hover',
        function(event) {
            if (event.type == 'mouseover') {

                // say('----<<<<  hovering  >>>------');

                var $submenu = $('ul:first', $(this)).clone();
                var $ulParent = $(this).parent();
                var width = $ulParent.outerWidth();

                var $menu = $(this).closest('.jmenu');

                var menuLeft = $menu.offset().left;
                var menuTop = $menu.offset().top;
                var parentLeft = $ulParent.offset().left;

                var ulParentTop = $ulParent.offset().top;
                var top = $(this).position().top;
                top = top + ulParentTop - menuTop;




                if ($submenu.length > 0) {

                    $submenu.css({
                        'float': 'left',
                        'display': 'block',
                        'marginLeft': width + parentLeft - menuLeft,
                        'top': top
                    });

                    //var exp = $('input:first').attr($.expando);
                    //$('input[' + $.expando + '=' + exp + ']')...

                    var currentChildClass = $(this).data('child-class');
                    if (currentChildClass == undefined) {
                        // say('current found: ' + currentChildId);

                        var childClass = $.generateUniqueClass();
                        // say('child class=' + childClass);
                        $submenu.addClass(childClass);
                        $submenu.data('unique-class', childClass);
                        $(this).data('child-class', childClass);



                        // say('setting popup for: ' + $(this)[0].id + ', unique-class:' + $submenu.data('unique-class'));

                        $submenu.insertAfter($(this).parent());
                        $submenu.show();
                    }
                }

                $(this).data('hovered', true);
            }
            else {
                $(this).data('hovered', false);
                var $jmenu = $(this).closest('.jmenu');

                evalHideMenu($('ul:first', $jmenu));

            }
        });
});