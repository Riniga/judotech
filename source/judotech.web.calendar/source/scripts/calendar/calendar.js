const monthNames = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

function filterPage()
{
    var url = "/";
    var year = document.getElementById("filteryear").value;
    var month = document.querySelector('input[name="filtermonth"]:checked');
    var district = document.getElementById("filter_districts").value;
    var type = document.getElementById("filtertype").value;

    var parameters = [];

    if (year > 2022) parameters.push("year=" + year);
    if (district != "all") parameters.push("district=" + district);
    if (type != "all") parameters.push("type=" + type);
    if (month) parameters.push("month=" + month.id);

    var parametersstring = parameters.join('&')
    if (parametersstring.length > 0) url = url + "?" + parametersstring;
    window.location.href = url;
}

function populateFilters(urlParams)
{
    var filter_districts = document.getElementById("filter_districts");
    const selected_district = urlParams.get('district')
    var districts = getWithExpiry('districts');
    if (!districts) location.reload();
    districts.forEach(district => {
        var option = document.createElement('option');
        option.text = district.name;
        option.value = district.name;
        if (option.value == selected_district) option.selected = 'selected';
        filter_districts.add(option);
    });
}
function setFilter(urlParams)
{
    const year = urlParams.get('year')
    const district = urlParams.get('district')
    const type = urlParams.get('type')
    const month = urlParams.get('month')
    
    if (year) document.getElementById("filteryear").value = year;
    if (district) document.getElementById("filter_districts").value = district;
    if (type) document.getElementById("filtertype").value = type;
    if (month) document.getElementById(month).checked = true;
}

(function ()
{
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    populateFilters(urlParams);
    setFilter(urlParams);

    var filtertype = document.getElementById("filtertype").value
    var filter_districts = document.getElementById("filter_districts").value
    var filteryear = document.getElementById("filteryear").value
    var filtermonth = document.querySelector('input[name="filtermonth"]:checked')
    if (!filtermonth) filtermonth = { "id": "all" };

    var table = document.getElementById('calendarTable');

    var events = getWithExpiry('events');
    if (!events) location.reload();

    events.forEach(event => {
        const startdate = new Date(event.start);
        if ((event.type == filtertype || filtertype == "all") &&
            (event.district == filter_districts || filter_districts == "all") &&
            ((startdate.getMonth() == filtermonth.id || filtermonth.id == "all")) &&
            (startdate.getFullYear() == filteryear) 
            )
        { 
            let row = table.insertRow(-1);
            let date = row.insertCell(0);
            let name = row.insertCell(1);
            let type = row.insertCell(2);
            let club= row.insertCell(3);
            let city = row.insertCell(4);

            date.innerText = startdate.toLocaleDateString('sv-se'); ;
            var link = document.createElement('a');
            link.setAttribute('href', 'event.html?requestedEvent=' + event.id);
            link.setAttribute('html', event.name);
            link.appendChild(document.createTextNode(event.name));
            name.appendChild(link);
            type.innerText = event.type;
            club.innerText = event.district;
        }
    });
})();

