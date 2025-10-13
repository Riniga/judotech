
$(document).ready(function () {
    var table = $('#refereetable').DataTable(
        {
            autoWidth: false,
            ajax: { url: readAllUsersApiUrl, dataSrc: "" },
            columns: [
                { "data": "fullName","fnCreatedCell": function (nTd, sData, oData, iRow, iCol) 
                    {
                        $(nTd).html("<a href='profile.html?email="+oData.email+"'>"+oData.fullName+"</a>");
                    }
                },
                { data: 'age' },
                { data: 'attendance' },
                { data: 'grade' },
                { data: 'borde' },
            ],
        }
    );
});
