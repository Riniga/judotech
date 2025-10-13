class User {
    constructor(email, password) {
      this.email = email;
      this.password = password;
      this.fullname = "";
      this.personnumber ="";
      this.adress ="";
      this.postalcode ="";
      this.city ="";
      this.primaryphone ="";
      this.secondaryphone ="";
      this.grade ="";
      this.club ="";
      this.zone ="";
      this.id =email;
      this.roles = [];
    }
}
class Login {
    constructor(email, token) {
      this.email = email;
      this.token = token;
    }
}

$(function() {
    $('form').each(function() {
        $(this).find('input').keypress(function(e) {
            // Enter pressed?
            if(e.which == 10 || e.which == 13) {
                console.log("enter was pressed");
                signIn();
            }
        });
    });
});

function signIn()
{
    $('#cover-spin').show(0)
    var email = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    const userObject = new User(email, password);
    console.log(userObject);
    console.log("loginApiUrl: " + loginApiUrl);
    fetch(loginApiUrl, 
        {
            method: 'post',
            headers: {'Content-Type': 'application/text' },
            body: JSON.stringify(userObject) 
        })
        .then(response => response.json())
        .then(data =>
        {
            console.log(data);
            if (data.token!="00000000-0000-0000-0000-000000000000") SuccessfullLogin(data);
            else ShowFailedLogin();
        })
        .catch((error) => {
            console.log(error);
            ShowFailedLogin();
        });
}

function getClub(club)
{
    $('#cover-spin').show(0)
    var email = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    const userObject = new User(email, password);
    console.log(userObject);
    fetch(loginApiUrl,
        {
            method: 'post',
            headers: {'Content-Type': 'application/text' },
            body: JSON.stringify(userObject) 
        })
        .then(response => response.json())
        .then(data =>
        {
            if (data.token!="00000000-0000-0000-0000-000000000000") SuccessfullLogin(data);
            else ShowFailedLogin();
        })
        .catch((error) => {
            console.log(error);
            ShowFailedLogin();
        });
}

function SuccessfullLogin(login)
{
    localStorage.setItem('currentLogin', JSON.stringify(login) );
    localStorage.setItem('currentClub', JSON.stringify(login) );
    
    setCurrentUser(login.email);
}

function setCurrentUser(email)
{
    const url = new URL(readUserApiUrl);      // ex: "https://.../ReadUser?code=...."
    url.searchParams.set('email', email);     // lägger till/ersätter ?email=...
    console.log("Hämta användare med url: " + url.toString());
    fetch(url,
        {
            method: 'get',
            headers: {'Content-Type': 'application/text' }
        })
        .then(response => response.json())
        .then(data =>
        {
            console.log(data);
            localStorage.setItem('currentUser', JSON.stringify(data) );
            window.location.href = "/club.html";
        })
        .catch((error) => {
            console.log(error);
        });
}

function signOut()
{
    currentLogin = localStorage.getItem('currentLogin');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('currentLogin');

    fetch(logoutApiUrl,
        {
            method: 'post',
            headers: {'Content-Type': 'application/text' },
            body: currentLogin 
        })
        .then(response => response.json())
        .then(data =>
        {
            window.location.href = "/";
        })
        .catch((error) => {
            console.log(error);
        });

    ResetLoginUI();
}

