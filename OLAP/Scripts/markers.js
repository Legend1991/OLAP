var markers = Array();
var blockIsMove = false;

function attachMarkerToCube(measureName, currCube, dim) {
	//	look up the name to mesh
    measureName = measureName.toUpperCase();
    //var markerText = document.getElementById("measureText");
    //markerText.textContent = measureName;

    //var container = document.getElementById( 'visualization' );
	var container = document.getElementById( 'visualization' );
	var template = document.getElementById( 'marker_template' );
	var marker = template.cloneNode(true);

	container.appendChild(marker);
	markers.push(marker);

	marker.measureName = measureName;
	marker.childNodes[0].childNodes[0].childNodes[0].textContent = measureName;

	if (dim == 0) {
	    marker.shiftGroup = currCube.position.x;
	} else if (dim == 1) {
	    marker.shiftGroup = currCube.position.y;
	} else if (dim == 2) {
	    marker.shiftGroup = currCube.position.z;
	}

	marker.currCube = currCube;
    currCube.marker = marker;
    marker.dim = dim;
	marker.selected = false;
	marker.hover = false;
    //if( countryName === selectedCountry.countryName.toUpperCase() )
	//	marker.selected = true;

	marker.setPosition = function(x,y,z){
		this.style.left = x + 'px';
		this.style.top = y + 'px';	
		this.style.zIndex = z;
	}

	marker.setPosition(100, 100, 500);

	marker.setVisible = function( vis ){
		if( ! vis )
			this.style.display = 'none';
		else{
			this.style.display = 'inline';
		}
	}
	var measureLayer = marker.querySelector('#measureText');
	marker.measureLayer = measureLayer;
    //marker.jquery = $(marker);

	marker.update = function(){
		//var matrix = scene.matrixWorld;
		//var abspos = this.currCube.position.applyMatrix4(matrix); //(this.currCube.clone());
		//var screenPos = screenXY(abspos);

	    var abspos = this.currCube.position.clone();
	    //abspos.x += 1;
	    //abspos.y += 0.5;
	    var screenPos = screenXY(abspos);

		// if( this.selected )
			// this.setVisible( true )
		// else
			//this.setVisible( ( abspos.z > 60 ));	

		var zIndex = Math.floor( 1000 - abspos.z);
		if( this.selected || this.hover )
			zIndex = 10000;

		if (this.dim == 0) {
		    this.setPosition(screenPos.x, screenPos.y - 50, zIndex);
		} else if (this.dim  == 1) {
		    this.setPosition(screenPos.x - 75, screenPos.y - 10, zIndex);
		} else if (this.dim == 2) {
		    this.setPosition(screenPos.x + 10, screenPos.y + 15, zIndex);
		}

	    //var debug = document.getElementById("debug");
	    //debug.textContent = "X: " + screenPos.x + " Y: " + screenPos.y + " Z: " + screenPos.z;
	}

	var markerSelect = function (e) {
	    if (blockIsMove == true && checkMovingGroup(this.dim, this.shiftGroup, this.currCube.position.x, this.currCube.position.y, this.currCube.position.z) == false) {
			return;
	    }
	    shift(this.dim, this.shiftGroup);
	};

	marker.addEventListener('click', markerSelect, true);
}		

function removeMarkerFromCube(currCube){
	var country = countryData[countryName];
	if( country === undefined )
		return;
	if( country.marker === undefined )
		return;

	var index = markers.indexOf(country.marker);
	if( index >= 0 )
		markers.splice( index, 1 );
	var container = document.getElementById( 'visualization' );		
	container.removeChild( country.marker );
	country.marker = undefined;		
}

function screenXY(vec3) {
    var vector = projector.projectVector(vec3.clone(), camera);
    var result = new Object();

    result.x = Math.round(vector.x * (WIDTH / 2)) + WIDTH / 2;
    result.y = Math.round((0 - vector.y) * (HEIGHT / 2)) + HEIGHT / 2;
    result.z = vector.z;
    return result;
}