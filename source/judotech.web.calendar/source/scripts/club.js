
currentUser = localStorage.getItem('currentUser');
if (!currentUser) { window.location.href = "/"; } 

function updateData()
{
    var user = JSON.parse(currentUser);
    var clubs = getWithExpiry('clubs');
    if (!clubs) location.reload();

    const currentClub = clubs.find(club => club.name == user.club);
    document.getElementById("clubname").innerHTML = currentClub.name;
    document.getElementById("cluburl").innerHTML = currentClub.url;
    document.getElementById("clubcontact").innerHTML = currentClub.contact;
    document.getElementById("clubphone").innerHTML = currentClub.phone;
    document.getElementById("gmap_canvas").src = currentClub.googlemaps;
}
updateData();






