(function($) {


	$(document).ready(function() {

		// enable script if not mobile
		var bScriptEnabled = false;
		bScriptEnabled = true;

		if ((_scms != undefined) && (_scms != null)) {

			if ((_scms.Browser != undefined) && (_scms.Browser != null)) {
				bScriptEnabled = (_scms.Browser.IsMobileDevice == false);
			}
		}

		// alert('IsMobileDevice: ' + _scms.Browser.IsMobileDevice.toString());


		if ( bScriptEnabled == true) {

			$('.ov').ov({
				suffix: '-ov'
			});


			$('.full #ticker #ticker-top .ticker-contents ul').ticker({
				speedPixelsPerSecond: 120
			});

			$('.full #ticker #ticker-bottom .ticker-contents ul').ticker({
				speedPixelsPerSecond: 60
			});



			$('.wide #ticker #ticker-top .ticker-contents ul').ticker({
				speedPixelsPerSecond: 120,
				containerWidth: 1200

			});

			$('.wide #ticker #ticker-bottom .ticker-contents ul').ticker({
				speedPixelsPerSecond: 60,
				containerWidth: 1200
			});


			$('.xmenu').xmenu();



			$('#pov').cycle({
				fx: 'fade',
				speed: 1500,
				timeout: 9000, // choose your transition type, ex: fade, scrollHorz, scrollUp, shuffle, etc...
				pager: '#pov-buttons',
				activePagerClass: 'active',
				pause: 1,
				pauseOnPagerHover: 1,
				pagerAnchorBuilder: function(idx, slide) {
					// return selector string for existing anchor 
					return '#pov-buttons a:eq(' + idx + ')';
				}
			});

			$('#pov-buttons').show();
			/* remove ^ and uncomment for behavior where buttons are hidden until hover
			$('#pov-wrap').hover(function() {
			$('#pov-buttons').show();
			},
			function() {
			$('#pov-buttons').hide();
			}
			);
			*/


			$('.featured-items').cycle({
				fx: 'scrollHorz',
				timeout: 0, // choose your transition type, ex: fade, scrollUp, shuffle, etc...
				prev: '.featured-left-button',
				next: '.featured-right-button',
				speed: 700,
				nowrap: true
			});
		}

	});
})(jQuery);