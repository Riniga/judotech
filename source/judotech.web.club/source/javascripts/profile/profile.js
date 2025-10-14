function loadPersonalInformation(user)
{
    console.log(user);
    document.getElementById("email").value = user.email;
    document.getElementById("fullname").value = user.fullName;
    document.getElementById("age").value = user.age;
    document.getElementById("grade").value = user.grade;
    document.getElementById("club").value = user.club;
    document.getElementById("attendance").value = user.attendance;

    document.getElementById("likes_technique").checked = user.likes_technique == true ;
    document.getElementById("likes_randori").checked = user.likes_randori == true ;
    document.getElementById("likes_compete").checked = user.likes_compete == true ;
    document.getElementById("likes_coach").checked = user.likes_coach == true ;
    document.getElementById("likes_referee").checked = user.likes_referee == true ;


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
    // if (currentLogin.email==user.email) localStorage.setItem('currentUser', JSON.stringify(currentUser) );
    // console.log(updateUserApiUrl + "?token=" + currentLogin.token)
    // var bodyContent = {token: currentLogin.token, user: user};

    var user = new User();
    user.email  = document.getElementById("email").value;
    user.fullname  = document.getElementById("fullname").value;
    user.age = document.getElementById("age").value;
    user.grade = document.getElementById("grade").value;
    user.attendance = document.getElementById("attendance").value;
    user.likes_technique = document.getElementById("likes_technique").checked;
    user.likes_randori = document.getElementById("likes_randori").checked;
    user.likes_compete = document.getElementById("likes_compete").checked;
    user.likes_coach = document.getElementById("likes_coach").checked;
    user.likes_referee = document.getElementById("likes_referee").checked;
    user.club = document.getElementById("club").value;
    user.comment = document.getElementById("comment").value;
    // Gill vill ska sparas också
    
    fetch( updateUserApiUrl+ "?token=" + currentLogin.token,
    {
        method: 'post',
        headers: {'Content-Type': 'application/text' },
        body: JSON.stringify(user)
    })
    .then(response => response.json())
    .then(data =>
    {
        console.log("user data saved!");
        $('#cover-spin').hide(0)
        document.location.reload();
    })
    .catch((error) => {
        console.log(error);
    });

    $('#cover-spin').hide(0)            
}
