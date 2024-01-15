let loginBtn = document.getElementById("loginBtn");
let registerBtn = document.getElementById("registerBtn");

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

