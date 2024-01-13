let loginBtn = document.getElementById("loginBtn");
let registerBtn = document.getElementById("registerBtn");

setEventListenersToBtns();
console.log("Set event listeners");
setScroller();
console.log("Set scroller");
setActiveLinks();
console.log("Set active linkgs");

function setActiveLinks(){
    let navMenu = document.getElementById("navMenu");
    let links = navMenu.getElementsByClassName("link");

    for (const element of links) {
        element.addEventListener("click", function() {
            let current = document.getElementsByClassName("active");
            element.className += " active";
            console.log(element.className)
            if (current === undefined) {
                return;
            }
            current[0].className = current[0].className.replace(" active", "");
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
    //login.style.left = "4px";
    //login.style.opacity = 1;

    //register.style.right = "-520px";
    //register.style.opacity = 0;

    loginBtn.className += " white-btn";
    registerBtn.className = "btn";
}

function changeClickedRegisterBtnStyle() {
    //login.style.left = "-510px";
    //login.style.opacity = 0;

    //register.style.right = "5px";
    //register.style.opacity = 1;

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


