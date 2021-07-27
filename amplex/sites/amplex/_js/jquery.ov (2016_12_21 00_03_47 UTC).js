(function($) {
	$.fn.ov = function(options) {


		var settings = $.extend({
			active_class: 'active',
			suffix: '_ov',
			regex: null,
			activatelink: true
		}, options || {});

		return this.each(function() {
			var url = $(this).attr('src');
			var hoverUrl = $.fn.ov.calculateHoverUrl(url, settings);

			var linkActivated = false;

			if (settings.activatelink) {

				var regex = settings.regex;

				if (regex == null) {
					// locate nearest enclosing link
					$a = $(this).closest('a');
					var href = $a.attr('href');
					regex = '(^' + href + '/|^' + href + '$)';
				}
				// ( ^href$|^href/)

				// alert( url + ':' + hoverUrl);
				var inFolder = $.fn.ov.isPageInFolder(location.pathname, regex);

				linkActivated = inFolder;
			}

			if (linkActivated == true) {
				$(this).attr('src', hoverUrl);
			}
			else {
				$(this).hover(function() {
					$(this).attr('src', hoverUrl);
				},
					function() {
						$(this).attr('src', url);
					});

				// preload the hover image
				if (document.images) {
					$('<div style="display:none;"><img src="' + hoverUrl + '" /></div>').appendTo($('body'));
					// var img = new Image();
					// img.src = hoverUrl;
				}
			}



		});

	};

	$.fn.ov.calculateHoverUrl = function(url, settings) {
		var hoverUrl = url;
		if (url != null) {
			var terminatorIndex = url.lastIndexOf(".");
			if ((terminatorIndex != null) && (terminatorIndex > 0)) {
				var filename = url.substring(0, terminatorIndex);
				var ext = url.substring(terminatorIndex + 1);
				hoverUrl = filename + settings.suffix + '.' + ext;
			}
		}

		return hoverUrl;
	};

	$.fn.ov.isPageInFolder = function(url, regex) {
		var match = false;

		if (regex != null) {
			var pattern = new RegExp(regex, 'i');
			if (pattern.test(url)) {
				match = true;
			}
		}

		return match;
	};




})(jQuery);
