var regionData = new Array();

(function($) {


	// This will parse a delimited string into an array of
	// arrays. The default delimiter is the comma, but this
	// can be overriden in the second argument.
	var CSVToArray = function(strData, strDelimiter) {
		// Check to see if the delimiter is defined. If not,
		// then default to comma.
		strDelimiter = (strDelimiter || ",");

		var quote_regexp = new RegExp("^\"(.*)\"$");

		// Create an array to hold our data. Give the array
		// a default empty first row.
		var arrData = [];

		var lines = strData.split(new RegExp("\r?[\r\n]"));

		for (var i = 0; i < lines.length; i++) {
			var fields = lines[i].split(strDelimiter);

			for (var ii = 0; ii < fields.length; ii++) {
				fields[ii] = fields[ii].replace(quote_regexp, "$1");
			}

			arrData.push(fields);
		}

		// Return the parsed data.
		return (arrData);
	}

	function getRegionValue(e) {
		var regionValue = null;

		var $area = $(e.target);
		var region = $area.attr('alt');
		if (region != null) {
			regionValue = regionData[region];
			if (regionValue != null) {
				var pattern = new RegExp('^#.*#$');
				if (pattern.test(regionValue.rate)) {
					regionValue.rate = regionData[regionValue.rate].rate;
				}
			}
		}
		return regionValue;
	}


	function showRegionInfo(e) {

		var regionValue = getRegionValue(e);

		if (regionValue != null) {

			var $divRegion = $('#divRegion');
			$divRegion.html(
						'<h2>' + regionValue.name + '</h2>'
						+ '<p><em>Freight Charge</em>: &nbsp;' + regionValue.rate + '</p>'
						);


			var $imgMap = $('#imgMap');
			var left = 0;
			var mapLeft = $imgMap.offset().left;
			var mapTop = $imgMap.offset().top;
			var mapWidth = $imgMap.width();
			var mouseMapDiffRatio = (e.pageX - mapLeft) / mapWidth;
			var divRegionWidth = $divRegion.width();
			//var divRegionLeft = e.pageX - mapLeft - (mouseMapDiffRatio * divRegionWidth); 
			var divRegionLeft = e.pageX - mapLeft + 35;
			var divRegionTop = e.pageY - mapTop - $divRegion.outerHeight() + 60;

			$divRegion.css({
				top: divRegionTop,
				left: divRegionLeft
			});

			$divRegion.show();
		}

	}


	$.ajax({
		url: '/sites/amplex/files/maps/shipping-rates-by-us-region.csv',
		dataType: 'text',
		success: function(data) {

			var aData = CSVToArray(data, ',');
			for (var index = 0; index < aData.length; index++) {
				var row = aData[index];
				if (row != null) {
					if (row.length > 1) {
						regionData[row[0]] = {
							name: row[0],
							rate: row[1]
						};
					}
				}
			}
		}
	});



	$(document).ready(function() {


		$('#imgMap').maphilight({
			strokeColor: '004400'
		});


		$('area').mouseout(function(e) {
			$('#divRegion').text('').hide();
		});



		$('area').mouseover(function(e) {
			showRegionInfo(e);
			e.preventDefault();
		}).click(function(e) {
			showRegionInfo(e);
			e.preventDefault();
		});

	});

})(jQuery);
