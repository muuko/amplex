var countyData = new Array();

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



	function showRegionInfo(e) {

		var $area = $(e.target);

		var county = $area.attr('alt');

		if (county != null) {
			var $county = countyData[county];

			var $divCounty = $('#divCounty');
			$divCounty.html(
						'<h2>' + $county.name + '</h2>'
						+ '<p><em>Freight Charge</em>: &nbsp;' + $county.rate + '</p>'
						);


			var $imgFloridaMap = $('#imgFloridaMap');
			var left = 0;
			var mapLeft = $imgFloridaMap.offset().left;
			var mapTop = $imgFloridaMap.offset().top;
			var mapWidth = $imgFloridaMap.width();
			var mouseMapDiffRatio = (e.pageX - mapLeft) / mapWidth;
			var divCountyWidth = $divCounty.width();
			/* var divCountyLeft = e.pageX - mapLeft - (mouseMapDiffRatio * divCountyWidth); */
			var divCountyLeft = e.pageX - mapLeft + 35;
			var divCountyTop = e.pageY - mapTop - $divCounty.outerHeight() + 60;

			$divCounty.css({
				top: divCountyTop,
				left: divCountyLeft
			});

			$divCounty.show();
		}
	}


	$.ajax({
		url: '/sites/amplex/files/maps/shipping-rates-by-county.csv',
		dataType: 'text',
		success: function(data) {

			var aData = CSVToArray(data, ',');
			for (var index = 0; index < aData.length; index++) {
				var row = aData[index];
				if (row != null) {
					if (row.length > 1) {
						countyData[row[0]] = {
							name: row[0],
							rate: row[1]
						};

					}
				}
			}
		}
	});


	$(document).ready(function() {

		
		$('#imgFloridaMap').maphilight({
		strokeColor: '004400'
		});


		$('area').mouseout(function(e) {
			$('#divCounty').text('').hide();
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
