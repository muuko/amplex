(function($) {
	$.fn.ticker = function(options) {


		var settings = $.extend({
			speedPixelsPerSecond: 100,
			containerWidth: 702
		}, options || {});

		return this.each(function() {




			var $items = $(this).children();
			// calculate width of each item
			settings._itemsTotalWidth = 0;
			$items.each(function() {
				var width = $(this).outerWidth();
				$(this).data('w', width);

			});

			settings._$items = $items.detach();

			settings._itemsTotalWidth = 0;
			settings._containerWidth = $(this).outerWidth();
			var $parent = $(this).parent();
			settings._containerWidth = $parent.outerWidth();
			settings._containerWidth = $parent.width();
			$(this).data('settings', settings);


			$.fn.ticker.fill($(this));

		});

	};



	$.fn.ticker.fill = function($obj) {

		var settings = $obj.data('settings');

		var done = false;
		var strMarginLeft = $obj.css('marginLeft');
		var marginLeft = parseInt(strMarginLeft);


		/* remove hidden items */
		/* readjust left margin */

		var leftMostWidth = 0;
		var $itemLeft = $obj.children().first();
		var leftMostWidth = $itemLeft.data('w');

		if ((-1 * marginLeft) >= leftMostWidth) {
			marginLeft = 0;
			$obj.css('marginLeft', 0);
			var $item = $obj.children().first().detach();
			var width = $item.data('w');

			if (settings._$items.length > 0) {
				settings._$items.after($item);
			}
			else {
				settings._$items = $item;
			}

			settings._itemsTotalWidth -= width;
		}

		/* append items to right to fill up */
		while (!done) {

			if ((settings.containerWidth - marginLeft) >= settings._itemsTotalWidth) {

				if (settings._$items.length > 0) {
					var $item = settings._$items.first(0);
					settings._$items = settings._$items.slice(1);
					var width = $item.data('w');
					$obj.append($item);
					settings._itemsTotalWidth += width;
				}
				else {
					done = true;
				}
			}
			else {
				done = true;
			}
		}


		// animate margin left until left item falls off
		leftMostWidth = 0;
		$itemLeft = $obj.children().first();
		leftMostWidth = $itemLeft.data('w');
		var leftAnimateRequest = leftMostWidth + marginLeft;


		// or until right item is totally in view
		var rightAnimateRequest = 0;
		if (settings.containerWidth < settings._itemsTotalWidth) {
			// current right dangle
			rightAnimateRequest = settings._itemsTotalWidth - settings.containerWidth + marginLeft;
		}

		var animateRequest = 0;
		if ((leftAnimateRequest > 0) && (rightAnimateRequest > 0)) {
			animateRequest = Math.min(leftAnimateRequest, rightAnimateRequest);
		}
		else {
			if (leftAnimateRequest > 0) {
				animateRequest = leftAnimateRequest;
			}
			else {
				animateRequest = rightAnimateRequest
			}
		}

		var nMilliseconds = 1000;
		nMilliseconds = ((animateRequest * 1000) / settings.speedPixelsPerSecond);

		if (animateRequest > 0) {


			$obj.data('settings', settings);


			var animationSettings =
			{
				duration: nMilliseconds,
				easing: 'linear',
				complete: $.fn.ticker.animationComplete
			};

			// animate the ticker
			$obj.animate({
				marginLeft: (-1 * animateRequest) + marginLeft
			},
			animationSettings);


			// stop on hover
			$obj.hover(function(e) {
				$obj.stop();
				$obj.data('stopped', true);


				e.stopPropogation();
				e.preventDefault();
			},
			function() {
				var stopped = $obj.data('stopped');
				if ((stopped != null) && (stopped == true)) {
					$obj.data('stopped', false);
					$.fn.ticker.fill($obj);
				}
			});
		}
	}

	$.fn.ticker.animationComplete = function() {
		$.fn.ticker.fill($(this));
	}


})(jQuery);
