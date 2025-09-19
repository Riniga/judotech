function signIn()
{
    document.getElementById("ajaxLoader").style.visibility = "visible";
    var username = document.getElementById('input_email').value;
    var password = document.getElementById('input_password').value;

    fetch("/data/users.json",
        {
            method: 'get',
            headers: {'Content-Type': 'application/json'},
            //body: data
        })
        .then(response => response.json())
        .then(response =>
        {
            var found = false;
            console.log(response)
            response.users.forEach(user => {
                if (username == user.username && password == user.password) {
                    found = true;
                    var userdata = { firstname: user.firstname, lastname: user.lastname, club: user.club };
                    localStorage.setItem('currentUser', JSON.stringify(userdata));
                    window.location.href = "/";
                }
            });
            if (!found) { alert('Det gick inte att logga in (1)'); }
            document.getElementById("ajaxLoader").style.visibility = "hidden";
        }).catch((error) => {
            alert('Det gick inte att logga in (2)');
            document.getElementById("ajaxLoader").style.visibility = "hidden";
        });
}

var button = document.getElementById('login_button');
    button.addEventListener("click", signIn);