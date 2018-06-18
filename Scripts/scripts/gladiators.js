function onCreate(response) {
    console.log(response);
    switch (response.status) {
        case 400:
            console.log("400 - Error.")
            console.log(JSON.parse(response.responseText));
            break;
        case 200:
            action("Index", "Gladiators");
            break;
        default:
            console.log(response)
            break;
    }
}

function onEdit(response) {
    console.log(response);
    switch (response.status) {
        case 400:
            console.log("400 - Error.")
            console.log(JSON.parse(response.responseText));
            break;
        case 200:
            action("Index", "Gladiators");
            break;
        default:
            console.log(response)
            break;
    }
}