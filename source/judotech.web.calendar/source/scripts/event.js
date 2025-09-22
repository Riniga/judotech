function updateData() {

    var events = getWithExpiry('events');
    if (!events) location.reload();

    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const requestedEvent = urlParams.get('requestedEvent')
    
    const currentEvent = events.find(event => event.id == requestedEvent);
    console.log(currentEvent);
    document.getElementById("eventname").innerHTML = currentEvent.name;
    document.getElementById("externalurl").currentEvent = currentEvent.link;
    document.getElementById("location").innerHTML = currentEvent.location;
    document.getElementById("starttime").innerHTML = currentEvent.start;
    document.getElementById("endtime").innerHTML = currentEvent.end;
    document.getElementById("type").innerHTML = currentEvent.type;
}
updateData();







