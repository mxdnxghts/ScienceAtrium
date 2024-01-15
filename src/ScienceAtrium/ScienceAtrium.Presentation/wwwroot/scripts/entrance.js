let login = document.getElementById("login");
let register = document.getElementById("register");

setEventListenersToBtns();

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