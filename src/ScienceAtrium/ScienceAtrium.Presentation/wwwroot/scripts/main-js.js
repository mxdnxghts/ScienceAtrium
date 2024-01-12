let login = document.getElementById("login");
let register = document.getElementById("register");
let loginBtn = document.getElementById("loginBtn");
let registerBtn = document.getElementById("registerBtn");

setEventListenersToBtns();
setScroller();
setActiveLinks();

function setActiveLinks(){
    var navMenu = document.getElementById("navMenu");
    let links = navMenu.getElementsByClassName("link");

    for (var i = 0; i < links.length; i++) {
        links[i].addEventListener("click", function() {
          var current = document.getElementsByClassName("active");
          current[0].className = current[0].className.replace(" active", "");
          links[i].className += " active";
        });
      }
}

function setEventListenersToBtns() {
    loginBtn.addEventListener("click", (e) => {
        e.preventDefault();

        changeClickedLoginBtnStyle();
    });

    registerBtn.addEventListener("click", (e) => {
        e.preventDefault();

        changeClickedRegisterBtnStyle();
    });
}


function changeClickedLoginBtnStyle() {
    login.style.left = "4px";
    login.style.opacity = 1;

    register.style.right = "-520px";
    register.style.opacity = 0;

    loginBtn.className += " white-btn";
    registerBtn.className = "btn";
}

function changeClickedRegisterBtnStyle() {
    login.style.left = "-510px";
    login.style.opacity = 0;

    register.style.right = "5px";
    register.style.opacity = 1;

    loginBtn.className = "btn";
    registerBtn.className += " white-btn";
}

function login() {
    changeClickedLoginBtnStyle();
}

function register() {
    changeClickedRegisterBtnStyle();
}

function setScroller() {
    if (document.documentElement.clientWidth < 539) {
        return;
    }
    
    document.addEventListener('DOMContentLoaded', () => {
        const main = document.querySelector('.main');
        const header = document.querySelector('.header');
        const headerFixed = () => {
            let scrollTop = window.scrollY;
            let mainCenter = main.offsetHeight / 2;
            if (scrollTop >= mainCenter) {
                header.classList.add('fixed')
                main.style.marginTop = `${header.offsetHeight}px`;
                header.style.background = `rgba(178, 196, 188, 1)`;
            } else {
                header.classList.remove('fixed')
                main.style.marginTop = `0px`;
            }
        };

        headerFixed();
        window.addEventListener('scroll', () => {
            headerFixed();
        });
    });
}


