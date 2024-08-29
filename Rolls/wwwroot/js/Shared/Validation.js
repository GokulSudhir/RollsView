function minLength(value) {
    const minLength = value;
    const regex = new RegExp(`^.{${minLength},}$`);
    if (regex.test(input)) {
        return true;
    } else {
        return false;
    }
}

function minMaxLength(value1, value2, input) {
    const minLength = value1;
    const maxLength = value2;
    const regex = new RegExp(`^.{${minLength},${maxLength}}$`);

    if (regex.test(input)) {
        return true;
    } else {
        return false;
    }
}

function lettersSpaceAnd(value) {
    const regEx = /^[\sa-zA-Z&]*$/;

    if (!value.match(regEx)) {
        return false
    }
    else {
        return true
    }
}


function IsEmpty(value) {

    if (value == '') {
        return true
    }
    else {
        return false
    }
}

function IsNumber(value) {
    var regEx = new RegExp("^[0-9]+$");
    if (!value.toString().match(regEx)) {
        return 0
    }
    else {
        return 1
    }
}

function IsUpperCaseAndNumbers(value) {
    var regEx = new RegExp("^[A-Z0-9]+$");
    if (!value.match(regEx)) {
        return 0
    }
    else {
        return 1
    }
}

function IsUpperCase(value) {
    var regEx = new RegExp("^[A-Z]+$");
    if (!value.match(regEx)) {
        return 0
    }
    else {
        return 1
    }
}

function IsMobile(value) {
    var regEx = new RegExp("^[1-9]+$")
    if (value.length != 10 && !value.toString().match(regEx)) {
        return 0
    }
    else {
        return 1
    }
}

function IsEmail(value) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    if (!emailReg.toString().match(value)) {
        return 0
    } else {
        return 1
    }
}

function IsCorrectLength(textString, value) {
    //return textString;
    if (textString.length == value) {
        return 1
    } else {
        return 0
    }
}

function IsUsername(value) {
    // Minimum eight characters, at least one letter and one number:
    var regex1 = "^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
    //Minimum eight characters, at least one letter, one number and one special character:
    var regex2 = "^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
    //Minimum eight characters, at least one uppercase letter, one lowercase letter and one number
    var regex3 = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";
    //Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character
    var regex4 = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    //Minimum eight and maximum 10 characters, at least one uppercase letter, one lowercase letter, one number and one special character
    var regex5 = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,10}$";

    // 8-30 characters, at least one letter and one number:
    var regex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,30}$/;

    if (!regex.toString().match(value)) {
        return 0
    } else {
        return 1
    }
}

function IsPassword(value) {
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@_$*#])[A-Za-z\d@_$*#]{8,30}$/;
    if (!regex.toString().match(value)) {
        return 0
    } else {
        return 1
    }
}