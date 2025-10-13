function toogleVisibleForm(form)
{
    hideAll();
    document.getElementById(form).style.display = "block"; 
}


function hideAll()
{
    document.getElementById('link_login').style.display = "none";
    document.getElementById('link_user').style.display = "none";
}

function refresh()
{
    currentUser = localStorage.getItem('currentUser');
    if (currentUser)
    {
        toogleVisibleForm('link_user');
        var user = JSON.parse(currentUser);
        document.getElementById('user_firstname').innerHTML = user.firstname;
        document.getElementById('loggedin_welcome').innerHTML = "Välkommen " + user.firstname + " " + user.lastname;
        document.getElementById('loggedin_club').innerHTML = user.club;

    }
    else
    {
        toogleVisibleForm('link_login');
    }
}


function updateDataStore()
{
    var clubsStore = getWithExpiry('clubs');
    if (!clubsStore)
    {
        fetch("/data/clubs.json",
            {
                method: 'get',
                headers: { 'Content-Type': 'application/json' },
            })
            .then(response => response.json())
            .then(response => {
                setWithExpiry('clubs', response.clubs, 5 * 60 * 1000)
            });
    }

    var districtsStore = getWithExpiry('districts');
    if (!districtsStore) {
        fetch("/data/districts.json",
            {
                method: 'get',
                headers: { 'Content-Type': 'application/json' },
            })
            .then(response => response.json())
            .then(response => {
                console.log("Reloaded districts!")
                setWithExpiry('districts', response.districts, 5*60*1000)
            });
    }

    var eventsStore = getWithExpiry('events');  
    if (!eventsStore) {
        fetch("/data/events.json",
            {
                method: 'get',
                headers: { 'Content-Type': 'application/json' },
            })
            .then(response => response.json())
            .then(response => {
                setWithExpiry('events', response.events, 5 * 60 * 1000)
            });
    }
}
function setWithExpiry(key, value, ttl) {
    const now = new Date()
    const item = {
        value: value,
        expiry: now.getTime() + ttl,
    }
    localStorage.setItem(key, JSON.stringify(item))
}

function getWithExpiry(key)
{
    const itemStr = localStorage.getItem(key)
    if (!itemStr) {return null }
    const item = JSON.parse(itemStr)
    const now = new Date()
    if (now.getTime() > item.expiry) {
        localStorage.removeItem(key)
        return null
    }
    return item.value
}

updateDataStore();
refresh();