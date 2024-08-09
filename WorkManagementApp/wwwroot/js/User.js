var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/User/getall' },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'userName', "width": "20%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'emailConfirmed', "width": "10%" },
            {
                data: { id: "companyId", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                        <div class="text-center">
                        <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor: pointer; width:100px;" 
                             <i class="bi bi-unlock-fill"></i> UnLock
                          </a>
                        </div>
                    `
                    }
                    else {
                        return `
                        <div class="text-center">
                           <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor: pointer; width:100px;" 
                             <i class="bi bi-lock-fill"></i> Lock
                          </a>
                        </div>
                    `
                    }

                },
                "width": "25%"
            }
        ]
    });
}



function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnLock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}
