var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = new DataTable('#tblData',{
        "ajax": "/order/getall",
        "columns": [
            { data: 'orderheaderid', className: 'w-10' },
        ]
    })
}