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
            url: "/Designation/GetDesignations",
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
                "data": "designation_name"
            },
            {
                "data": "designation_category"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="editPopup(\'' + row.designation_id + '\',\'' + row.designation_name + '\', \'' + row.designation_category + '\')" ><i class="bi bi-pen"></i></a>';
                    t += '  |  <a class="btn btn-sm btn-outline-primary border-0" onclick="deletePopup(\'' + row.designation_id + '\', \'' + row.designation_name + '\')"><i class="bi bi-trash3"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

function InsertData() {
    var cnt = 0;
    var myDiv1 = document.getElementById("error-container");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message");
    spanMessage.textContent = "";
    let designationName = $('#designation_name').val();
    let designationCategory = $('#designation_category').val();

    var chk = minMaxLength(2, 50, designationName);
    if (chk == false) {
        cnt = 1;
    }

    chk = lettersSpaceAnd(designationName);
    if (chk == false) {
        cnt = 2;
    }

    if (cnt == 0) {
        const data = {
            designation_name: designationName,
            designation_category: designationCategory
        };

        fetch('/Designation/AddDesignation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())

            .then(data => {
                if (data.error == 'success') {
                    $('#designation_name').val('');
                    $('#designation_category').val('');
                    $('span#mySpan').css('color', 'green');
                    $('span#mySpan').text('Designation Added');
                    GetData();
                    $('#alertModal').modal('show');
                } else {
                    //*** write to Span */
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
            msg = "Designation Name should be 2 to 50 characters long.";
        } else {
            msg = "Designation Name should contain only Letters, spaces and &.";
        }
        spanMessage.style.color = "red";
        spanMessage.innerText = msg;

    }
}

function editPopup(designationId, designationName, designationCategory) {
    var desgName = document.getElementById("designationNameSpan");
    desgName.style.color = "green";
    desgName.innerText = designationName;

    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";

    $("#designation_name_edit").val(designationName);
    $("#designation_category_edit").val(designationCategory);
    $("#designation_id").val(designationId);
    $("#designation_name_edit").focus();

    $('#editModal').modal('show');
}

function UpdateData() {
    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";
    var cnt = 0;

    let designationId = $('#designation_id').val();
    let designationName = $('#designation_name_edit').val();
    let designationCategory = $('#designation_category_edit').val();

    var chk = minMaxLength(2, 50, designationName);
    if (chk == false) {
        cnt = 1;
    }

    chk = lettersSpaceAnd(designationName);
    if (chk == false) {
        cnt = 2;
    }

    if (cnt == 0) {
        const data = {
            designation_id: designationId,
            designation_name: designationName,
            designation_name_edit: designationName,
            designation_category: designationCategory,
            designation_category_edit: designationCategory

        };

        fetch('/Designation/AddDesignation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())

            .then(data => {
                if (data.error == 'success') {
                    $('#designation_name_edit').val('');
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
            msg = "Designation Name should be 2 to 50 characters long.";
        } else {
            msg = "Designation Name should contain only Letters, spaces and &.";
        }
        spanMessage.style.color = "red";
        spanMessage.innerText = msg;
    }
}

function deletePopup(designationId, designationName) {
    var desgName = document.getElementById("designationNameSpan2");
    desgName.style.color = "red";
    desgName.innerText = designationName;

    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";

    $("#designation_name_delete").val(designationName);
    $("#delete_designation_id").val(designationId);
    $("#designation_name_delete").focus();

    $('#deleteModal').modal('show');
}

function DeleteData() {
    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";
    var cnt = 0;

    let designationId = $('#delete_designation_id').val();

    const data = {
        designation_id: designationId

    };

    fetch('/Designation/DeleteDesignation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#designation_name_delete').val('');
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

function GetDeletedData() {

    var data = {}
    var oTable = $('#tbl_datatable_deleted').DataTable({
        "processing": true,
        "serverSide": true,
        "destroy": true,
        "responsive": false,
        "info": true,
        "ajax": {
            url: "/Designation/DeletedDesignations",
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
                "data": "designation_name"
            },
            {
                "data": "designation_category"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="RestorePopup(\'' + row.designation_id + '\',\'' + row.designation_name + '\')" ><i class="bi bi-bootstrap-reboot"></i></a>';
                    t += '|  <a class="btn btn-sm btn-outline-danger border-0" onclick="PermenentDeletePopup(\'' + row.designation_id + '\',\'' + row.designation_name + '\')" ><i class="bi bi-trash"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

function RestorePopup(designationId, designationName) {
    var dsgName = document.getElementById("designationNameSpan3");
    dsgName.style.color = "green";
    dsgName.innerText = designationName;

    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";

    $("#designation_name_restore").val(designationName);
    $("#restore_designation_id").val(designationId);
    $("#designation_name_restore").focus();

    $('#restoreModal').modal('show');
}

function RestoreData() {
    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";
    var cnt = 0;

    let designationId = $('#restore_designation_id').val();

    const data = {
        designation_id: designationId,

    };

    fetch('/Designation/RestoreDesignation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#designation_name_restore').val('');
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

function PermenentDeletePopup(designationId, designationName) {
    var dsgName = document.getElementById("designationNameSpan4");
    dsgName.style.color = "red";
    dsgName.innerText = designationName;

    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";

    $("#designation_name_p_delete").val(designationName);
    $("#p_delete_designation_id").val(designationId);
    $("#designation_name_p_delete").focus();

    $('#p_deleteModal').modal('show');
}

function PermenentDeleteData() {
    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";
    var cnt = 0;

    let designationId = $('#p_delete_designation_id').val();

    const data = {
        designation_id: designationId

    };

    fetch('/Designation/PermanentDelete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())

        .then(data => {
            if (data.error == 'success') {
                $('#designation_name_p_delete').val('');
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