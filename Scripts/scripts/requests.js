//Javascript version of clicking an Ajax.ActionLink
function action(action, controller = "Home", target = "page-content") {
    var token = sessionStorage.getItem(tokenKey);
    var headers = {};
    if (token) {
        headers.Authorization = 'Bearer ' + token;
    }
    $.ajax({
        type: "GET",
        url: `/${controller}/${action}`,
        headers: headers,
        success: (data) => {
            $('#' + target).html(data);
        }
    })
}