
$(document).ready(function () {
    var table = $('#refereetable').DataTable(
        {
            autoWidth: false,
            ajax: { url: readAllUsersApiUrl, dataSrc: "" },
            columns: [
                { "data": "fullname","fnCreatedCell": function (nTd, sData, oData, iRow, iCol) 
                    {
                        $(nTd).html("<a href='profile.html?email="+oData.email+"'>"+oData.fullname+"</a>");
                    }
                },
                { data: 'license' },
                { data: 'zone' },
                { data: 'club' },
                { data: 'email' },
            ],
        }
    );
});
