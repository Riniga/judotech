function loadPersonalInformation(user)
{
    console.log(user);
    // document.getElementById("email").value = user.email;
    document.getElementById("fullname").value = user.fullName;
    document.getElementById("grade").value = user.grade;
    document.getElementById("club").value = user.club;
    document.getElementById("attendance").value = user.attendance;
    // document.getElementById("adress").value = user.adress;
    // document.getElementById("postalcode").value = user.postalcode;
    // document.getElementById("city").value = user.city;
    // document.getElementById("primaryphone").value = user.primaryphone;
    // document.getElementById("secondaryphone").value = user.secondaryphone;
    // document.getElementById("license").value = user.license;
     document.getElementById("club").value = user.club;
    // document.getElementById("zone").value = user.zone;
    document.getElementById("comment").value = "Enligt ålder och närvaro borde graderas till: " +  user.borde + " (Diff: " + user.diff + ")";
}

if (document.getElementById("PersonalInformationTab")) 
{
    
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const  email = urlParams.get("email");
    
    if (email) LoadUser(email);
    else {
        var currentUser = JSON.parse(localStorage.getItem('currentUser'));
        loadPersonalInformation(currentUser); 
    }
}

function LoadUser(email)
{
    const url = new URL(readUserApiUrl);      
    url.searchParams.set('email', email);     
    fetch(url,
        {
            method: 'get',
            headers: {'Content-Type': 'application/text' }
        })
        .then(response => response.json())
        .then(data =>
        {
            var user = JSON.stringify(data);
            loadPersonalInformation(data); 
        })
        .catch((error) => {
            console.log(error);
        });
}

function updateProfile()
{
    $('#cover-spin').show(0)
    const currentLogin = JSON.parse(localStorage.getItem('currentLogin')); 
    var user = new User();
    user.email  = document.getElementById("email").value;
    user.fullname  = document.getElementById("fullname").value;
    user.personnumber = document.getElementById("personnumber").value;
    user.adress = document.getElementById("adress").value;
    user.postalcode = document.getElementById("postalcode").value;
    user.city = document.getElementById("city").value;
    user.primaryphone = document.getElementById("primaryphone").value;
    user.secondaryphone = document.getElementById("secondaryphone").value;
    user.club = document.getElementById("club").value;
    user.license = document.getElementById("license").value;
    user.zone = document.getElementById("zone").value;

    if (currentLogin.email==user.email) localStorage.setItem('currentUser', JSON.stringify(currentUser) );

    console.log(updateUserApiUrl + "?token=" + currentLogin.token)

    var bodyContent = {token: currentLogin.token, user: user};

    
    fetch( updateUserApiUrl+ "?token=" + currentLogin.token,
    {
        method: 'post',
        headers: {'Content-Type': 'application/text' },
        body: JSON.stringify(bodyContent)
    })
    .then(response => response.json())
    .then(data =>
    {
        console.log("user data saved!");
        $('#cover-spin').hide(0)
    })
    .catch((error) => {
        console.log(error);
    });

    $('#cover-spin').hide(0)            
}
