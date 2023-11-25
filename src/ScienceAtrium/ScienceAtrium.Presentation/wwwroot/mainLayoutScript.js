
var btn = document.getElementById('shop-btn-redirection');
btn.addEventListener("click", (e) => {
    e.preventDefault();
    window.location.href = "/basket";
})