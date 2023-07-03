window.addEventListener("scroll", function () {
   
    var nvabar = this.document.querySelector("header")
    nvabar.classList.toggle("stick", this.window.scrollY > 0);

    var nvabar = this.document.getElementById("filtercont");
    nvabar.classList.toggle("stick1", this.window.scrollY > 0);
});