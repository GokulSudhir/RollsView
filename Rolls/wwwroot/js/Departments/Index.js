
$(function () {
    Init();
});

async function Init() {
    await GetData();

    $('#btn_add').on('click', function () {
        InsertData();
    });

    $('#btn_update').on('click', function () {
        UpdateData();
    });

    $('#btn_delete').on('click', function () {
        DeleteData();
    });

    $('#active-tab').on('click', function () {
        GetData()
    });

    $('#deleted-tab').on('click', function () {
        GetDeletedData()
    });

    $('#btn_restore').on('click', function () {
        RestoreData();
    });

    $('#btn_p_delete').on('click', function () {
        PermenentDeleteData();
    });
}

function GetData() {
    var data = {}
    var oTable = $('#tbl_datatable').DataTable({
        "processing": true,
        "serverSide": true,
        "destroy": true,
        "responsive": false,
        "info": true,
        "ajax": {
            url: "/Departments/GetDepartmentsAsync",
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
        },
        "initComplete": function () {
            $('#tbl-datatable_filter input').unbind();
            $('#tbl-datatable_filter input').bind('keyup', function (e) {
                if (e.keyCode == 13) {
                    oTable.search(this.value).draw();
                }
            });
        },
        "columns": [
            {
                "data": "department_name"
            },
            {
                "data": "department_classification"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="editPopup(\'' + row.department_id + '\',\'' + row.department_name + '\', \'' + row.department_classification + '\')" ><i class="bi bi-pen"></i></a>';
                    t += '  |  <a class="btn btn-sm btn-outline-primary border-0" onclick="deletePopup(\'' + row.department_id + '\', \'' + row.department_name + '\')"><i class="bi bi-trash3"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

//form controller
$(function () {
    $('#myForm').submit(function (event) {
        event.preventDefault(); // Prevent the default form submission
        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: $(this).serialize(),
            success: function (result) {
                // Handle the response from the server
            }
        });
    });

    $('#myForm1').submit(function (event) {
        event.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: $(this).serialize(),
            success: function (result) {

            }
        });
    });


});

function InsertData() {
    var cnt = 0;
    var myDiv1 = document.getElementById("error-container");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message");
    spanMessage.textContent = "";
    let departmentName = $('#department_name').val();
    let departmentClassification = $('#department_classification').val();

    var chk = minMaxLength(2, 50, departmentName);
    if (chk == false) {
        cnt = 1;
    }

    chk = lettersSpaceAnd(departmentName);
    if (chk == false) {
        cnt = 2;
    }

    if (cnt == 0) {
        const data = {
            department_name: departmentName,
            department_classification: departmentClassification
        };

        fetch('/Departments/AddDepartment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())

            .then(data => {
                if (data.error == 'success') {
                    $('#department_name').val('');
                    $('#department_classification').val('');
                    $('span#mySpan').css('color', 'green');
                    $('span#mySpan').text('Department Added');
                    GetData();
                    $('#alertModal').modal('show');
                } else {
                    //*** writ to Span */
                    var spanElement = document.getElementById("message");
                    spanElement.style.color = "red";
                    for (const element of data.errors) {
                        spanElement.innerText = element;
                    }
                }
            })
            .catch(error => {
                console.error('Error sending data:', error);
                // Display an error message
                alert('Error sending data!');
            })

    } else {
        var msg = '';
        if (cnt == 1) {
            msg = "Department Name should be 2 to 50 characters long.";
        } else {
            msg = "Department Name should contain only Letters, spaces and &.";
        }
        spanMessage.style.color = "red";
        spanMessage.innerText = msg;

    }
}

function editPopup(departmentId, departmentName, departmentClassification) {
    var deptName = document.getElementById("departmentNameSpan");
    deptName.style.color = "green";
    deptName.innerText = departmentName;

    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";

    $("#department_name_edit").val(departmentName);
    $("#department_classification_edit").val(departmentClassification);
    $("#department_id").val(departmentId);
    $("#department_name_edit").focus();

    $('#editModal').modal('show');
}

function UpdateData() {
    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";
    var cnt = 0;

    let departmentId = $('#department_id').val();
    let departmentName = $('#department_name_edit').val();
    let departmentClassification = $('#department_classification_edit').val();

    var chk = minMaxLength(2, 50, departmentName);
    if (chk == false) {
        cnt = 1;
    }

    chk = lettersSpaceAnd(departmentName);
    if (chk == false) {
        cnt = 2;
    }

    if (cnt == 0) {
        const data = {
            department_id: departmentId,
            department_name: departmentName,
            department_name_edit: departmentName,
            department_classification: departmentClassification,
            department_classification_edit: departmentClassification

        };

        fetch('/Departments/AddDepartment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())

            .then(data => {
                if (data.error == 'success') {
                    $('#department_name_edit').val('');
                    GetData();
                    var modal = $('#editModal');
                    modal.modal('hide');

                } else {
                    var myDiv = document.getElementById("error-container_edit");
                    myDiv.style.color = "red";
                    for (var i = 0; i < data.errors.length; i++) {
                        var textNode = document.createTextNode(data.errors[i]);
                        var lineBreak = document.createElement("br");
                        myDiv.appendChild(textNode);
                        myDiv.appendChild(lineBreak);
                    }

                }
            })
            .catch(error => {
                $('span#message_edit').css('color', 'red');
                $('span#message_edit').text(error);
                $('#alertModal').modal('show');
            });
    } else {
        var msg = '';
        if (cnt == 1) {
            msg = "Department Name should be 2 to 50 characters long.";
        } else {
            msg = "Department Name should contain only Letters, spaces and &.";
        }
        spanMessage.style.color = "red";
        spanMessage.innerText = msg;
    }
}

function DeleteData() {
    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";
    var cnt = 0;

    let departmentId = $('#delete_department_id').val();

    /*	if (cnt == 0) {*/
    const data = {
        department_id: departmentId

    };

    fetch('/Departments/DeleteDepartment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#department_name_delete').val('');
                GetData();
                var modal = $('#deleteModal');
                modal.modal('hide');

            } else {
                var myDiv = document.getElementById("error-container_delete");
                myDiv.style.color = "red";
                for (var i = 0; i < data.errors.length; i++) {
                    var textNode = document.createTextNode(data.errors[i]);
                    var lineBreak = document.createElement("br");
                    myDiv.appendChild(textNode);
                    myDiv.appendChild(lineBreak);
                }

            }
        })
        .catch(error => {
            $('span#message_delete').css('color', 'red');
            $('span#message_delete').text(error);
            $('#alertModal').modal('show');
        });
}

function deletePopup(departmentId, departmentName) {
    var deptName = document.getElementById("departmentNameSpan2");
    deptName.style.color = "red";
    deptName.innerText = departmentName;

    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";

    $("#department_name_delete").val(departmentName);
    $("#delete_department_id").val(departmentId);
    $("#department_name_delete").focus();

    $('#deleteModal').modal('show');
}

function GetDeletedData() {
    var data = {}
    var oTable = $('#tbl_datatable_deleted').DataTable({
        "processing": true,
        "serverSide": true,
        "destroy": true,
        "responsive": false,
        "info": true,
        "ajax": {
            url: "/Departments/GetDeletedDepartments",
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
        },
        "initComplete": function () {
            $('#tbl-datatable_filter input').unbind();
            $('#tbl-datatable_filter input').bind('keyup', function (e) {
                if (e.keyCode == 13) {
                    oTable.search(this.value).draw();
                }
            });
        },
        "columns": [
            {
                "data": "department_name"
            },
            {
                "data": "department_classification"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="RestorePopup(\'' + row.department_id + '\',\'' + row.department_name + '\')" ><i class="bi bi-bootstrap-reboot"></i></a>';
                    t += '|  <a class="btn btn-sm btn-outline-danger border-0" onclick="PermenentDeletePopup(\'' + row.department_id + '\',\'' + row.department_name + '\')" ><i class="bi bi-trash"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

function RestoreData() {
    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";
    var cnt = 0;

    let departmentId = $('#restore_department_id').val();

    const data = {
        department_id: departmentId,

    };

    fetch('/Departments/RestoreDepartment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#department_name_restore').val('');
                GetDeletedData();
                var modal = $('#restoreModal');
                modal.modal('hide');

            } else {
                var myDiv = document.getElementById("error-container_restore");
                myDiv.style.color = "red";
                for (var i = 0; i < data.errors.length; i++) {
                    var textNode = document.createTextNode(data.errors[i]);
                    var lineBreak = document.createElement("br");
                    myDiv.appendChild(textNode);
                    myDiv.appendChild(lineBreak);
                }

            }
        })
        .catch(error => {
            $('span#message_restore').css('color', 'red');
            $('span#message_restore').text(error);
            $('#alertModal').modal('show');
        });

}

function RestorePopup(departmentId, departmentName) {
    var deptName = document.getElementById("departmentNameSpan3");
    deptName.style.color = "green";
    deptName.innerText = departmentName;

    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";

    $("#department_name_restore").val(departmentName);
    $("#restore_department_id").val(departmentId);
    $("#department_name_restore").focus();

    $('#restoreModal').modal('show');
}

function PermenentDeletePopup(departmentId, departmentName) {
    var deptName = document.getElementById("departmentNameSpan4");
    deptName.style.color = "red";
    deptName.innerText = departmentName;

    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";

    $("#department_name_p_delete").val(departmentName);
    $("#p_delete_department_id").val(departmentId);
    $("#department_name_p_delete").focus();

    $('#p_deleteModal').modal('show');
}

function PermenentDeleteData() {
    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";
    var cnt = 0;

    let departmentId = $('#p_delete_department_id').val();

    const data = {
        department_id: departmentId

    };

    fetch('/Departments/PermanentDelete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#department_name_p_delete').val('');
                GetDeletedData();
                var modal = $('#p_deleteModal');
                modal.modal('hide');

            } else {
                var myDiv = document.getElementById("error-container_p_delete");
                myDiv.style.color = "red";
                for (var i = 0; i < data.errors.length; i++) {
                    var textNode = document.createTextNode(data.errors[i]);
                    var lineBreak = document.createElement("br");
                    myDiv.appendChild(textNode);
                    myDiv.appendChild(lineBreak);
                }

            }
        })
        .catch(error => {
            $('span#message_p_delete').css('color', 'red');
            $('span#message_p_delete').text(error);
            $('#alertModal').modal('show');
        });

}