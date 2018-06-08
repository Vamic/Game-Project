function onToken(response) {
    switch (response.status) {
        case 400:
            console.log("400 - Error.")
            console.log(JSON.parse(response.responseText));
            break;
        case 200:
            const data = response.responseJSON;
            console.log("200 - Logged in as " + data.userName);

            // Save the access token in session storage.
            sessionStorage.setItem(tokenKey, data.access_token);
            action("Index");
            action("Navbar", "Home", "navbar-container");
            break;
        default:
            console.log(response)
            break;
    }
}

function onLogout() {
    console.log("Logged out");
    //Remove token from session because we're logged out
    sessionStorage.removeItem(tokenKey);
    action("Navbar", "Home", "navbar-container");
}

function onRegister(response) {
    switch (response.status) {
        case 400:
            console.log("400 - Error.");
            console.log(JSON.parse(response.responseText));
            break;
        case 200:
            console.log("200 - Registered.");
            action("Index");
            break;
        default:
            console.log(response)
            break;
    }
}