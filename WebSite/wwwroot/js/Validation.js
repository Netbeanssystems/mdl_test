function myKeyPress(e) {
    var keynum;
    if (window.event) { // IE
        keynum = e.keyCode;
    } else if (e.which) { // Netscape/Firefox/Opera
        keynum = e.which;
    }
    if (keynum == 62 || keynum == 60)
        e.preventDefault();
}

function myKeyPress1(e) {
    var keynum;
    if (window.event) { // IE                    
        keynum = e.keyCode;
    } else if (e.which) { // Netscape/Firefox/Opera                   
        keynum = e.which;
    }

    // Allow digits only
    if (keynum < 48 || keynum > 57) {
        e.preventDefault();  // Prevent non-digit characters
    }

    // Check if the value is "000000000" after the key press
    var currentValue = e.target.value + String.fromCharCode(keynum);
    if (/^0{9,}$/.test(currentValue)) { // Check if it's nine or more zeros
        e.preventDefault();  // Prevent the input if it's "000000000" or similar
    }
}
