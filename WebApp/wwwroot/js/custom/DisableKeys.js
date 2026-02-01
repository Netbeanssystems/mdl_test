//disable right click
document.onmousedown = function () {
    if (event.button === 2) {
        alert("Sorry! This action is not allowed.");
    }
};

// disable right click
document.addEventListener('contextmenu', event => event.preventDefault());

function DisableBackButton() {
    window.history.forward();
}

DisableBackButton();

window.onload = DisableBackButton;

window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() };

window.onunload = function () { null };
window.onunload = function () { void (0) };

document.onkeydown = function (e) {
    if (e.keyCode === 123) {// disable F12 key
        return false;
    }else if (e.ctrlKey && e.shiftKey && e.keyCode === 73) {// disable ctrl+shift+I key
        return false;
    }else if (e.ctrlKey && e.shiftKey && e.keyCode == 74) {// disable ctrl+shift+J key
        return false;
    }else if (e.ctrlKey && e.keyCode === 85) {// disable ctrl+U key
        return false;
    }else if (e.ctrlKey && e.keyCode === 67) {// disable ctrl+C key
        return false;
    }else if (e.ctrlKey && e.keyCode === 82) {// disable ctrl+R key
        return false;
    }else if (e.keyCode === 93) {// disable select key
        return false;
    }else if (e.keyCode === 116) {// disable f5 key
        return false;
    }else {
        return true;
    }
}
var message = "Changes you made may not be saved. Are you sure?";
window.onbeforeunload = function (event) {
    var e = event || window.event;
    if (e) {
        e.returnValue = message;
    }
    return message;
};