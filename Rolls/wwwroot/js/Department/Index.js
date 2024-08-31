
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
            url: "/Department/GetDepartmentsAsync",
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
                "data": "department_code"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="editPopup(\'' + row.department_id + '\',\'' + row.department_name + '\', \'' + row.department_code + '\')" ><i class="bi bi-pen"></i></a>';
                    t += '  |  <a class="btn btn-sm btn-outline-primary border-0" href="/Bank/Delete/' + row.department_id + '"><i class="bi bi-trash3"></i></a>';
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
    let departmentCode = $('#department_code').val();

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
            department_code: departmentCode
        };

        fetch('/Department/AddDepartment', {
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
                    $('#department_code').val('');
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

function editPopup(departmentId, departmentName, departmentCode) {
    var deptName = document.getElementById("departmentNameSpan");
    deptName.style.color = "green";
    deptName.innerText = departmentName;

    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";

    $("#department_name_edit").val(departmentName);
    $("#department_code_edit").val(departmentCode);
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
    let departmentCode = $('#department_code_edit').val();

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
            department_code: departmentCode,
            department_code_edit: departmentCode

        };

        fetch('/Department/AddDepartment', {
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