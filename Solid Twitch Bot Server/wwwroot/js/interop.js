function GetCursorPosFromInput(inputId) {
    var el = document.getElementById(inputId);
    var val = el.value;
    return val.slice(0, el.selectionStart).length;
}