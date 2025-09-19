function ShowFailedLogin()
{
    $('#cover-spin').hide(0)
    document.getElementById("username").setAttribute('aria-invalid', 'true');
    document.getElementById("password").setAttribute('aria-invalid', 'true');
    document.getElementById("failedlogin").style.visibility = "visible";
}

function ResetLoginUI()
{
    currentUser = localStorage.getItem('currentUser');
    var loginItem = document.getElementById("login");
    var logoutItem = document.getElementById("logout");
    var mainMenu  = document.getElementById("mainMenu");

    if (currentUser)
    {
        document.getElementById("userFirstName").innerHTML=JSON.parse(currentUser).fullname;
        var email = JSON.parse(currentUser).email;
        var md5hash = CryptoJS.MD5(email).toString();

        //document.getElementById("userGravatar").src="https://www.gravatar.com/avatar/" + md5hash;

        loginItem.setAttribute('hidden', 'true');
        logoutItem.removeAttribute('hidden');
        mainMenu.removeAttribute('hidden');
    }
    else
    {
        if (window.location.pathname!='/') window.location.href = "/";
        logoutItem.setAttribute('hidden', 'true');
        mainMenu.setAttribute('hidden', 'true');
        loginItem.removeAttribute('hidden');
    }

    document.getElementById("username").setAttribute('aria-invalid', 'false');
    document.getElementById("password").setAttribute('aria-invalid', 'false');
    document.getElementById("failedlogin").style.visibility = "hidden";
}

ResetLoginUI(); 