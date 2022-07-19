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

var visible = true;

function toggleNav() {
    if(visible) {
        document.getElementById("nav").style.display = "none";
        document.getElementById("navBtn").style.left = "0px";
        document.getElementById("navBtn").innerText = ">";
        document.getElementById("content").style.marginLeft = "32px";
        document.getElementById("content").style.width = "width: calc(100vw - 4vw)";
        visible = false;
    }else {
        document.getElementById("nav").style.display = "";
        document.getElementById("navBtn").style.left = "max(200px, 15vw)";
        document.getElementById("navBtn").innerText = "<";
        document.getElementById("content").style.marginLeft = "16vw";
        document.getElementById("content").style.width = "width: calc(100vw - max(200px, 16vw) - 4vw)";
        visible = true;
    }
}