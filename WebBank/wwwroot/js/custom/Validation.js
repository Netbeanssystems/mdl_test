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