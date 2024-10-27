function openNav() {

    document.querySelector("sidenav").style.width = "250px";

    document.getElementById("main").style.marginLeft = "250px";
    document.querySelector(".closebtn").style.display = "block";
    document.querySelector(".openNav").style.display = "none";
    // document.querySelectorAll(".side-text").forEach(function(element) {
    //     element.style.display = 'block';
    // });
    document.querySelector(".sidenav").style.display = "block";
    document.querySelectorAll(".menu").forEach(function(element) {
        element.style.padding = "8px 30px 8px 20px";
    });
    document.querySelectorAll(".submenu li a").forEach(function(element) {
        element.style.padding = "8px 30px 8px 00px";
    });
    document.querySelectorAll(".d-flex-center").forEach(function(element) {
        element.style.justifyContent = "start";
    });
    document.querySelector(".sidenav").classList.add("width_100");
    document.querySelector(".sidenav").classList.remove("sidenav-collpased");
    $(".side-text").fadeIn(900);
    $(".side-text").removeClass('d-none')
}

function closeNav() {
    $(".side-text").addClass('d-none')
    $(".side-text").fadeOut('fast');
    document.querySelector("sidenav").style.width = "70px";
    document.getElementById("main").style.marginLeft = "70px";
    document.querySelector(".closebtn").style.display = "none";
    document.querySelector(".openNav").style.display = "block";
    // document.querySelectorAll(".side-text").forEach(function(element) {
    //     element.style.display = 'none';
    // });
    document.querySelector(".sidenav").style.display = "flex";
    document.querySelectorAll(".menu").forEach(function(element) {
        element.style.padding = "8px 0px 8px 0px";
    });
    document.querySelectorAll(".d-flex-center").forEach(function(element) {
        element.style.justifyContent = "center";
    });
    document.querySelector(".sidenav").classList.remove("width_100");
    document.querySelector(".sidenav").classList.add("sidenav-collpased");
}





// Add active class to the current button (highlight it)
function navHighlight(elem, home, active) {
    var url = location.href.split('/'),
        loc = url[url.length - 1],
        link = document.querySelectorAll(elem);
    for (var i = 0; i < link.length; i++) {
        var path = link[i].href.split('/'),
            page = path[path.length - 1];
        if (page == loc || page == home && loc == '') {
            link[i].className += ' ' + active;
            document.body.className += ' ' + page.substr(0, page.lastIndexOf('.'));
        }
    }
}

navHighlight('.menu', 'profile.html', 'active');