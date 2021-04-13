// Write your JavaScript code.
var isStart = true;
var tokenKey = "accessToken";
var accountKey = "userAccount";

function GetURLParameter(sParam)
{
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) 
    {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) 
        {
            return sParameterName[1];
        }
    }
}

//function getUrlParameter(sParam) {
//    var sPageURL = window.location.search.substring(1);
//    var sURLVariables = sPageURL.split('&');
    
//    for (i = 0; i < sURLVariables.length; i++) {
//        var sParameterName = sURLVariables[i].split('=');

//        if (sParameterName[0] === sParam) {
//            return typeof sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
//        }
//    }
//    return false;
//}
function checkInvite(code, email, role) {
    
}


function getUserName(account) {
    if (account == null) return null;
    return account.firstName + " " + account.lastName;
}

function redirectHandler(condition, url){

    switch(condition) {
    case 1: window.location = url;
        break;

    case 'value2':  // if (x === 'value2')
        break;

    default:
        break;
}
    //if(account == null){
    //    window.location = url;
    //}else{
    //    return false;
    //}
}
async function getUserAccount() {
    const token = sessionStorage.getItem(tokenKey);
 
    const response = await fetch("/api/account", {
        method: "GET",
        headers: {
            "Accept": "application/json",
            "Authorization": "Bearer " + token  // передача токена в заголовке
        }
    });
    if (response.ok === true) {
                 
        const data = await response.json();
        sessionStorage.setItem(accountKey, JSON.stringify(data));
    }
    else
        console.log("Status: ", response.status);
};

//async function getData(url) {
//    const token = sessionStorage.getItem(tokenKey);
 
//    const response = await fetch(url, {
//        method: "GET",
//        headers: {
//            "Accept": "application/json",
//            "Authorization": "Bearer " + token  // передача токена в заголовке
//        }
//    });
//    if (response.ok === true) {
                 
//        const data = await response.json();
//        //return data;
//        account = data;
//    }
//    else
//        console.log("Status: ", response.status);
//};


function post(path, params, method) {
    method = method || "post"; // Set method to post by default if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for(var key in params) {
        if(params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}

function getAccount() {
    var request = $.getJSON('/api/account', null);
    request.done(function(data) {
        sessionStorage.setItem(accountKey, JSON.stringify(data));
    });
}

$.getJSON = function(url, callback) {
    var token = sessionStorage.getItem(tokenKey);

    return jQuery.ajax({
        headers: { 
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + token
        },
        'type': 'GET',
        'url': url,
        //'data': JSON.stringify(data),
        'dataType': 'json',
        'success': callback
    });
};
$.postJSON = function(url, data, callback) {
    return jQuery.ajax({
        headers: { 
            'Accept': 'application/json',
            'Content-Type': 'application/json' 
        },
        //accepts: {
        //    json: 'application/json'
        //},
        //contentType: 'application/json',
        'type': 'POST',
        'url': url,
        'data': JSON.stringify(data),
        'dataType': 'json',
        'success': callback
    });
};

function postAjax(url, data, callback) {
    return jQuery.ajax({
        headers: { 
            'Accept': 'application/json',
            'Content-Type': 'application/json' 
        },
        accepts: {
            json: 'application/json'
        },
        contentType: 'application/json' ,
        'type': 'POST',
        'url': url,
        'data': JSON.stringify(data),
        'dataType': 'json',
        'success': callback
    });
};

function httpGetAsync(theUrl, callback) {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function() {
        if (xmlHttp.readyState === 4 && xmlHttp.status === 200)
            callback(xmlHttp.responseText);
    }
    xmlHttp.open("GET", theUrl, true); // true for asynchronous 
    xmlHttp.send(null);
}

function httpGet(theUrl) {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", theUrl, false); // false for synchronous request
    xmlHttp.send(null);
    return xmlHttp.responseText;
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return null;
}


