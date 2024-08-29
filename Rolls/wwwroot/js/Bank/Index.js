
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

	$('#btn_restore').on('click', function () {
		RestoreData();
	});
	$('#btn_p_delete').on('click', function () {
		PermenentDeleteData();
	});
	$('#active-tab').on('click', function () {
		GetData()
	});
	$('#deleted-tab').on('click', function () {
		GetDeletedData()
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
			url: "/Bank/GetBanksAsync",
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
				"data": "bank_name"
			},
			{
				"data": "action",
				"render": function (data, type, row) {
					var t = '<div class="d-flex">';
					t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="editPopup(\'' + row.bank_id + '\',\'' + row.bank_name + '\')" ><i class="bi bi-pen"></i></a>';
					t += '  |   <a class="btn btn-sm btn-outline-primary border-0" onclick="deletePopup(\'' + row.bank_id + '\',\'' + row.bank_name + '\')" ><i class="bi bi-trash3"></i></a>';
					//t += '  |  <a class="btn btn-sm btn-outline-primary border-0" href="/Bank/Delete/' + row.bank_id + '"><i class="bi bi-trash3"></i></a>';
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
	let bankName = $('#bank_name').val();

	var chk = minMaxLength(2, 50, bankName);
	if (chk == false) {
		cnt = 1;
	}

	chk = lettersSpaceAnd(bankName);
	if (chk == false) {
		cnt = 2;
	}

	if (cnt == 0) {
		const data = {
			bank_name: bankName
		};

		fetch('/Bank/AddBank', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(data)
		})
			.then(response => response.json())

			.then(data => {
				if (data.error == 'success') {
					$('#bank_name').val('');
					$('span#mySpan').css('color', 'green');
					$('span#mySpan').text('Bank name added');
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
			msg = "Bank Name should be 2 to 50 characters long.";
		} else {
			msg = "Bank Name should contain only Letters, spaces and &.";
		}
		spanMessage.style.color = "red";
		spanMessage.innerText = msg;

	}
}

function editPopup(bankId, bankName) {
	var bnkName = document.getElementById("bankNameSpan");
	bnkName.style.color = "green";
	bnkName.innerText = bankName;

	var myDiv1 = document.getElementById("error-container_edit");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_edit");
	spanMessage.textContent = "";

	$("#bank_name_edit").val(bankName);
	$("#bank_id").val(bankId);
	$("#bank_name_edit").focus();

	$('#editModal').modal('show');
}

function UpdateData() {
	var myDiv1 = document.getElementById("error-container_edit");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_edit");
	spanMessage.textContent = "";
	var cnt = 0;

	let bankId = $('#bank_id').val();
	let bankName = $('#bank_name_edit').val();

	var chk = minMaxLength(2, 50, bankName);
	if (chk == false) {
		cnt = 1;
	}

	chk = lettersSpaceAnd(bankName);
	if (chk == false) {
		cnt = 2;
	}

	if (cnt == 0) {
		const data = {
			bank_id: bankId,
			bank_name: bankName,
			bank_name_edit: bankName
		};

		fetch('/Bank/AddBank', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(data)
		})
			.then(response => response.json())

			.then(data => {
				if (data.error == 'success') {
					$('#bank_name_edit').val('');
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
			msg = "Bank Name should be 2 to 50 characters long.";
		} else {
			msg = "Bank Name should contain only Letters, spaces and &.";
		}
		spanMessage.style.color = "red";
		spanMessage.innerText = msg;
	}
}


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


function deletePopup(bankId, bankName) {
	var bnkName = document.getElementById("bankNameSpan2");
	bnkName.style.color = "red";
	bnkName.innerText = bankName;

	var myDiv1 = document.getElementById("error-container_delete");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_delete");
	spanMessage.textContent = "";

	$("#bank_name_delete").val(bankName);
	$("#delete_bank_id").val(bankId);
	$("#bank_name_delete").focus();

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
			url: "/Bank/GetDeletedBanksAsync",
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
				"data": "bank_name"
			},
			{
				"data": "action",
				"render": function (data, type, row) {
					var t = '<div class="d-flex">';
					t += '<a class="btn btn-sm btn-outline-primary border-0" onclick="RestorePopup(\'' + row.bank_id + '\',\'' + row.bank_name + '\')" ><i class="bi bi-bootstrap-reboot"></i></a>';
					t += '|  <a class="btn btn-sm btn-outline-danger border-0" onclick="PermenentDeletePopup(\'' + row.bank_id + '\',\'' + row.bank_name + '\')" ><i class="bi bi-trash"></i></a>';
					t += '</div>';
					return t;
				},
				"orderable": false
			}
		]
	});

	$('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}


function DeleteData() {
	var myDiv1 = document.getElementById("error-container_delete");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_delete");
	spanMessage.textContent = "";
	var cnt = 0;

	let bankId = $('#delete_bank_id').val();
	//let bankName = $('#bank_name_edit').val();

	//var chk = minMaxLength(2, 50, bankName);
	//if (chk == false) {
	//	cnt = 1;
	//}

	//chk = lettersSpaceAnd(bankName);
	//if (chk == false) {
	//	cnt = 2;
	//}

	/*	if (cnt == 0) {*/
	const data = {
		bank_id: bankId,
		//bank_name: bankName,
		//bank_name_edit: bankName
	};

	fetch('/Bank/DeleteBank', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(data)
	})
		.then(response => response.json())

		.then(data => {
			if (data.error == 'success') {
				$('#bank_name_delete').val('');
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
	//} else {
	//	var msg = '';
	//	if (cnt == 1) {
	//		msg = "Bank Name should be 2 to 50 characters long.";
	//	} else {
	//		msg = "Bank Name should contain only Letters, spaces and &.";
	//	}
	//	spanMessage.style.color = "red";
	//	spanMessage.innerText = msg;
	//}
}

function RestorePopup(bankId, bankName) {
	var bnkName = document.getElementById("bankNameSpan3");
	bnkName.style.color = "green";
	bnkName.innerText = bankName;

	var myDiv1 = document.getElementById("error-container_restore");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_restore");
	spanMessage.textContent = "";

	$("#bank_name_restore").val(bankName);
	$("#restore_bank_id").val(bankId);
	$("#bank_name_restore").focus();

	$('#restoreModal').modal('show');
}

function RestoreData() {
	var myDiv1 = document.getElementById("error-container_restore");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_restore");
	spanMessage.textContent = "";
	var cnt = 0;

	let bankId = $('#restore_bank_id').val();

	const data = {
		bank_id: bankId,

	};

	fetch('/Bank/RestoreBank', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(data)
	})
		.then(response => response.json())

		.then(data => {
			if (data.error == 'success') {
				$('#bank_name_restore').val('');
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

function PermenentDeletePopup(bankId, bankName) {
	var bnkName = document.getElementById("bankNameSpan4");
	bnkName.style.color = "red";
	bnkName.innerText = bankName;

	var myDiv1 = document.getElementById("error-container_p_delete");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_p_delete");
	spanMessage.textContent = "";

	$("#bank_name_p_delete").val(bankName);
	$("#p_delete_bank_id").val(bankId);
	$("#bank_name_p_delete").focus();

	$('#p_deleteModal').modal('show');
}


function PermenentDeleteData() {
	var myDiv1 = document.getElementById("error-container_p_delete");
	myDiv1.innerHTML = "";
	const spanMessage = document.getElementById("message_p_delete");
	spanMessage.textContent = "";
	var cnt = 0;

	let bankId = $('#p_delete_bank_id').val();

	const data = {
		bank_id: bankId,

	};

	fetch('/Bank/PermanentDelete', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(data)
	})
		.then(response => response.json())

		.then(data => {
			if (data.error == 'success') {
				$('#bank_name_p_delete').val('');
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
