export function getCookies() {
    return document.cookie;
}

export function setCookie(cookieString) {
    document.cookie = cookieString;
}
