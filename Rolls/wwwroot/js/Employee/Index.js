$(function () {
    Init();
});

async function Init() {

    await GetDepartmentsDropDown();

    await GetDesignationsDropDown();

    await GetData();

    $('#active-tab').on('click', function () {
        GetData()
    });

    $('#btn_add').on('click', function () {
        InsertData();
    });

    $('#btn_update').on('click', function () {
        UpdateData();
    });

    $('#btn_delete').on('click', function () {
        DeleteData();
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

async function GetDepartmentsDropDown(options = {}) {

    try {
        const response = await fetch('/Departments/DepartmentDropDown', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        const dropdownId = options.dropdownId || 'department_id';
        const dropdown = document.getElementById(dropdownId);

        dropdown.innerHTML = '';

        if (!options.hideDefault) {
            const defaultOption = document.createElement('option');
            defaultOption.text = 'Select Department';
            defaultOption.value = '';
            defaultOption.disabled = true;
            defaultOption.selected = true;
            dropdown.add(defaultOption);
        }

        data.forEach(department => {
            const option = document.createElement('option');
            option.text = department.department_name;
            option.value = department.department_id;

            if (String(options.selectedDepartmentId) === String(department.department_id)) {
                option.selected = true;
            }

            dropdown.add(option);
        });

    } catch (error) {
        console.error('Error fetching department data:', error);
    }
}

async function GetDesignationsDropDown(options = {}) {

    try {
        const response = await fetch('/Designation/DesignationDropDown', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        const dropdownId = options.dropdownId || 'designation_id';
        const dropdown = document.getElementById(dropdownId);

        dropdown.innerHTML = '';

        if (!options.hideDefault) {
            const defaultOption = document.createElement('option');
            defaultOption.text = 'Select Designation';
            defaultOption.value = '';
            defaultOption.disabled = true;
            defaultOption.selected = true;
            dropdown.add(defaultOption);
        }

        data.forEach(designation => {
            const option = document.createElement('option');
            option.text = designation.designation_name;
            option.value = designation.designation_id;

            if (Number(options.selectedDesignationId) === designation.designation_id) {
                option.selected = true;
            }

            dropdown.add(option);
        });

    } catch (error) {
        console.error('Error fetching designation data:', error);
    }
}

async function InsertData() {

    var cnt = 0;
    var myDiv1 = document.getElementById("error-container");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message");
    spanMessage.textContent = "";
    let firstName = $('#first_name').val();
    let middleName = $('#middle_name').val();
    let lastName = $('#last_name').val();
    let emailId = $('#email').val();
    let mobilE = $('#mobile').val();
    let departmentId = $('#department_id').val();
    let designationId = $('#designation_id').val();

    const data = {

        first_name: firstName,
        middle_name: middleName,
        last_name: lastName,
        email_id: emailId,
        mobile: mobilE,
        department_id: departmentId,
        designation_id: designationId
    };

    try {

        const response = await fetch('/Employee/AddEmployee', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.statusCode === 200) {
            $('#first_name').val('');
            $('#middle_name').val('');
            $('#last_name').val('');
            $('#email').val('');
            $('#mobile').val('');
            $('#department_id').val('');
            $('#designation_id').val('');
            $('span#mySpan').css('color', 'green');
            $('span#mySpan').text('Employee Added');
            $('#alertModal').modal('show');
            GetData();
        } else {
            const spanElement = document.getElementById("message");
            spanElement.style.color = "red";
            spanElement.innerText = result.errors.join('\n');
        }
    }

    catch (error) {

        alert('Error sending data!');
        console.log('Error sending data:', error);
    }
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
            url: "/Employee/GetEmployees",
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
                "data": "first_name"
            },
            {
                "data": "middle_name"
            },
            {
                "data": "last_name"
            },
            {
                "data": "email_id"
            },
            {
                "data": "mobile"
            },
            {
                "data": "department_name"
            },
            {
                "data": "designation_name"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="editPopup(\'' + row.employee_id + '\',\'' + row.first_name + '\', \'' + row.middle_name + '\', \'' + row.last_name + '\', \'' + row.email_id + '\', \'' + row.mobile + '\', \'' + row.department_id + '\', \'' + row.designation_id + '\')" ><i class="bi bi-pen"></i></a>';
                    t += '  |  <a class="btn btn-sm btn-outline-primary border-0" onclick="deletePopup(\'' + row.employee_id + '\', \'' + row.mobile + '\')"><i class="bi bi-trash3"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

function editPopup(employeeId, firstName, middleName, lastName, emailId, mobilE, departmentId, designationId) {

    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";

    $("#employee_id").val(employeeId);
    $("#first_name_edit").val(firstName);
    $("#middle_name_edit").val(middleName);
    $("#last_name_edit").val(lastName);
    $("#email_id_edit").val(emailId);
    $("#mobile_edit").val(mobilE);

    GetDepartmentsDropDown({ dropdownId: 'department_id_edit', hideDefault: true, selectedDepartmentId: departmentId });
    GetDesignationsDropDown({ dropdownId: 'designation_id_edit', hideDefault: true, selectedDesignationId: designationId });

    $("#first_name_edit").focus();

    $('#editModal').modal('show');
}

async function UpdateData() {

    var myDiv1 = document.getElementById("error-container_edit");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_edit");
    spanMessage.textContent = "";
    var cnt = 0;

    let employeeId = $('#employee_id').val();
    let firstName = $('#first_name_edit').val();
    let middleName = $('#middle_name_edit').val();
    let lastName = $('#last_name_edit').val();
    let emailId = $('#email_id_edit').val();
    let mobilE = $('#mobile_edit').val();
    let departmentId = $('#department_id_edit').val();
    let designationId = $('#designation_id_edit').val();

    const data = {
        employee_id: employeeId,
        first_name: firstName,
        first_name_edit: firstName,
        middle_name: middleName,
        middle_name_edit: middleName,
        last_name: lastName,
        last_name_edit: lastName,
        email_id: emailId,
        email_id_edit: emailId,
        mobile: mobilE,
        mobile_edit: mobilE,
        department_id: departmentId,
        department_id_edit: departmentId,
        designation_id: designationId,
        designation_id_edit: designationId
    };

    try {
        const response = await fetch('/Employee/AddEmployee', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.statusCode === 200) {
            $('#first_name_edit').val('');
            GetData();
            var modal = $('#editModal');
            modal.modal('hide');
        } else {
            var myDiv = document.getElementById("error-container_edit");
            myDiv.style.color = "red";
            result.errors.forEach(error => {
                var textNode = document.createTextNode(error);
                var lineBreak = document.createElement("br");
                myDiv.appendChild(textNode);
                myDiv.appendChild(lineBreak);
            });
        }
    } catch (error) {
        $('span#message_edit').css('color', 'red');
        $('span#message_edit').text(error);
        $('#alertModal').modal('show');
    }
}

function deletePopup(employeeId, mobilE) {

    var empMobile = document.getElementById("employeeNameSpan2");
    empMobile.style.color = "red";
    empMobile.innerText = mobilE;

    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";

    //$("#designation_name_delete").val(designationName);
    $("#delete_employee_id").val(employeeId);
   //$("#designation_name_delete").focus();

    $('#deleteModal').modal('show');
}

async function DeleteData() {

    var myDiv1 = document.getElementById("error-container_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_delete");
    spanMessage.textContent = "";

    let employeeId = $('#delete_employee_id').val();

    const data = {

        employee_id: employeeId
    };

    try {
        const response = await fetch('/Employee/DeleteEmployee', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.statusCode === 200) {
            GetData();
            var modal = $('#deleteModal');
            modal.modal('hide');
        } else {
            var myDiv = document.getElementById("error-container_delete");
            myDiv.style.color = "red";
            myDiv.innerHTML = '';  // Clear previous errors
            for (let i = 0; i < result.errors.length; i++) {
                var textNode = document.createTextNode(result.errors[i]);
                var lineBreak = document.createElement("br");
                myDiv.appendChild(textNode);
                myDiv.appendChild(lineBreak);
            }
        }
    } catch (error) {
        $('span#message_delete').css('color', 'red');
        $('span#message_delete').text(error);
        $('#alertModal').modal('show');
    }
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
            url: "/Employee/DeletedEmployees",
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
                "data": "first_name"
            },
            {
                "data": "middle_name"
            },
            {
                "data": "last_name"
            },
            {
                "data": "email_id"
            },
            {
                "data": "mobile"
            },
            {
                "data": "department_name"
            },
            {
                "data": "designation_name"
            },
            {
                "data": "action",
                "render": function (data, type, row) {
                    var t = '<div class="d-flex">';
                    t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="RestorePopup(\'' + row.employee_id + '\',\'' + row.mobile + '\')" ><i class="bi bi-bootstrap-reboot"></i></a>';
                    t += '|  <a class="btn btn-sm btn-outline-danger border-0" onclick="PermenentDeletePopup(\'' + row.employee_id + '\',\'' + row.mobile + '\')" ><i class="bi bi-trash"></i></a>';
                    t += '</div>';
                    return t;
                },
                "orderable": false
            }
        ]
    });

    $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}

function RestorePopup(employeeId, mobilE) {
    var empMobile = document.getElementById("employeeNameSpan3");
    empMobile.style.color = "green";
    empMobile.innerText = mobilE;

    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";

    $("#restore_employee_id").val(employeeId);

    $('#restoreModal').modal('show');
}

async function RestoreData() {

    var myDiv1 = document.getElementById("error-container_restore");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_restore");
    spanMessage.textContent = "";

    let employeeId = $('#restore_employee_id').val();

    const data = {

        employee_id: employeeId,
    };

    try {
        const response = await fetch('/Employee/RestoreEmployee', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.statusCode === 200) {
            GetDeletedData();
            var modal = $('#restoreModal');
            modal.modal('hide');
        } else {
            var myDiv = document.getElementById("error-container_restore");
            myDiv.style.color = "red";
            myDiv.innerHTML = '';  
            for (let i = 0; i < result.errors.length; i++) {
                var textNode = document.createTextNode(result.errors[i]);
                var lineBreak = document.createElement("br");
                myDiv.appendChild(textNode);
                myDiv.appendChild(lineBreak);
            }
        }
    } catch (error) {
        $('span#message_restore').css('color', 'red');
        $('span#message_restore').text(error);
        $('#alertModal').modal('show');
    }

}

function PermenentDeletePopup(employeeId, mobilE) {

    var empMobile = document.getElementById("employeeNameSpan4");
    empMobile.style.color = "red";
    empMobile.innerText = mobilE;

    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";

    $("#p_delete_employee_id").val(employeeId);

    $('#p_deleteModal').modal('show');
}

async function PermenentDeleteData() {

    var myDiv1 = document.getElementById("error-container_p_delete");
    myDiv1.innerHTML = "";
    const spanMessage = document.getElementById("message_p_delete");
    spanMessage.textContent = "";

    let employeeId = $('#p_delete_employee_id').val();

    const data = {

        employee_id: employeeId
    };

    try {
        const response = await fetch('/Employee/PermanentDelete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.statusCode === 200) {
            GetDeletedData();
            var modal = $('#p_deleteModal');
            modal.modal('hide');
        } else {
            var myDiv = document.getElementById("error-container_p_delete");
            myDiv.style.color = "red";
            myDiv.innerHTML = '';  // Clear previous errors
            for (let i = 0; i < result.errors.length; i++) {
                var textNode = document.createTextNode(result.errors[i]);
                var lineBreak = document.createElement("br");
                myDiv.appendChild(textNode);
                myDiv.appendChild(lineBreak);
            }
        }
    } catch (error) {
        $('span#message_p_delete').css('color', 'red');
        $('span#message_p_delete').text(error);
        $('#alertModal').modal('show');
    }

}

