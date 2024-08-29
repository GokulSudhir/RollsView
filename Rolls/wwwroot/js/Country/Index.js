$(document).ready(function () {


  $(function () {
    //$.validator.addMethod('remote', function (value, element, params) {
    //  var additionalFields = {};
    //  $(params.additionalFields).each(function () {
    //    additionalFields[this.name] = $('#' + this.id).val();
    //  });
    //  var result = $.ajax({
    //    url: params.url,
    //    type: 'GET',
    //    data: $.extend({}, { input: value }, additionalFields)
    //  });
    //  return result.then(function (response) {
    //    return response === 'True';
    //  });
    //}, '');

    //$.validator.addMethod('remote', function (value, element, params) {
    //  var additionalFields = {};
    //  $(params.additionalFields).each(function () {
    //    additionalFields[this.name] = $('#' + this.id).val();
    //  });
    //  var url = $(element).attr('data-val-remote');
    //  var result = $.ajax({
    //    url: url,
    //    type: 'GET',
    //    data: $.extend({}, { input: value }, additionalFields)
    //  });
    //  return result.then(function (response) {
    //    return response === 'True';
    //  });
    //}, '');

    $.validator.addMethod('remote', function (value, element, params) {
      var additionalFields = {};
      
      var addn = $(element).attr('data-val-remote-additionalfields');
      var additionalFieldNames = addn.split(',');

      for (var i = 0; i < additionalFieldNames.length; i++) {
        var fieldName = additionalFieldNames[i];
        additionalFields[fieldName] = $('#' + fieldName).val();
      }


      var url = $(element).attr('data-val-remote');
      var result = $.ajax({
        url: url,
        type: 'GET',
        data: $.extend({}, { input: value }, additionalFields)
      });
      return result.then(function (response) {
        if (response === 'True') {
          return true;
        } else {
          return response; // return validation message string
        }
      });
    }, '');


    //$('form').validate();

    $('form').validate({
      rules: {
        Input: {
          remote: true
        }
      },
      messages: {
        Input: {
          remote: function () {
            return 'Invalid input value';
          }
        }
      }
    });

  });

  $('#btn_add').on('click', function () {
    let country = $('#txt_name').val();
    alert(country);
  });


  Init();
});

async function Init() {



  await GetData();
}


function GetData() {
  //alert("henlo");
  var data = {}
  var oTable = $('#tbl_datatable').DataTable({
    "processing": true,
    "serverSide": true,
    "destroy": true,
    "responsive": false,
    "info": true,
    "ajax": {
      url: "/Country/GetCountriesAsync",
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
        "data": "country_code"
      },
      {
        "data": "country_name"
      },

      {
        "data": "action",
        "render": function (data, type, row) {
          var t = '<div class="d-flex">';
          t += '<a class="btn btn-sm btn-outline-primary border-0" href="/Country/Edit/' + row.country_id + '"><i class="bi bi-pen"></i></a>';
          t += '  |  <a class="btn btn-sm btn-outline-primary border-0" href="/Country/Delete/' + row.country_id + '"><i class="bi bi-trash3"></i></a>';
          t += '</div>';
          return t;

        },
        "orderable": false

      }


    ]
  });

  $('#tbl-datatable').children('tr:even').addClass('tbl-row-even');
}