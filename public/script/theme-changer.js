const documentEl = document.documentElement;
const themeChanger = document.getElementById("theme-changer");
const cookieName = "app-theme";


const darkIcon = '<i class="bi bi-moon"></i>';
const lightIcon = '<i class="bi bi-brightness-high"></i>';

if (localStorage.getItem(cookieName) === null) {
    localStorage.setItem(cookieName, "light");
    documentEl.setAttribute("data-bs-theme", "light");
    themeChanger.innerHTML = lightIcon;
} else {
    if (localStorage.getItem(cookieName) === "light") {
        documentEl.setAttribute("data-bs-theme", "light");
        themeChanger.innerHTML = darkIcon;
    }else {
        documentEl.setAttribute("data-bs-theme", "dark");
        themeChanger.innerHTML = lightIcon;
    }
}

themeChanger.addEventListener("click", e => {
    e.preventDefault()
    if (documentEl.getAttribute("data-bs-theme") === "light") {
        documentEl.setAttribute("data-bs-theme", "dark");
        localStorage.setItem(cookieName, "dark");
        themeChanger.innerHTML = lightIcon;
    } else {
        documentEl.setAttribute("data-bs-theme", "light");
        localStorage.setItem(cookieName, "light");
        themeChanger.innerHTML = darkIcon;
    }
})