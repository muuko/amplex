(function($) {
    $.fn.voting = function(options) {
        var settings = $.extend({
            vote: 1,
            voteElement: '#divCurrentVote',
            activeImage: "/scms/modules/submission/public/images/up_ov.gif",
            inactiveImage: "/scms/modules/submission/public/images/up.gif",
            evenImage: "/scms/modules/submission/public/images/even.gif",
            aSelectText: ['very bad', 'bad', 'ok', 'good', 'great']
        }, options || {});


        var holderElement = $(this);
        var images = $('img', $(holderElement));

        $(images).each(function(n) {
            $(this).hover(function() {
                setVote(holderElement, n, settings.aSelectText[n]);
            }
                );
        });

        $(this).hover(
                null,
                function() {
                    setVote(holderElement, settings.vote - 1, settings.votesText);
                }
        );

        setVote(holderElement, settings.vote - 1, settings.votesText);

        function setVote(holderElement, vote, votesText) {
            var images = $('img', $(holderElement));
            var voteAdjusted = vote;


            $(images).each(function(n) {
                var element = $(this)[0];
                var source = null;

                // check middle first
                if ((voteAdjusted >= (n - 0.5)) && (voteAdjusted < n)) {
                    source = settings.evenImage;
                }
                // full
                else if (voteAdjusted >= n) {
                    source = settings.activeImage;
                }
                else {
                    source = settings.inactiveImage;
                }


                element.src = source;


            }
       );

            $(settings.voteElement, $(holderElement).parent()).html(votesText);

        }
    };
})(jQuery);
