window.onload = () => {
    var els = document.getElementsByClassName("navElem");

    for(el of els) {
        addNavClick(el, el.getAttribute("data-goto"));
    }
}

function addNavClick(el, to) {
    el.addEventListener("click", (ev) => {
        window.history.pushState({}, to, '#' + to);

        document.getElementById(to).scrollIntoView({
            "behavior": "smooth"
        });
    });
}