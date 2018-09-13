var onComplete = function () {
    $("#pleaseWait").addClass("hidden");
};

var onSuccess = function (context) {
    console.log(context);
    if (context.isSuccess) {
        console.log(context.redirectUrl);
        window.location.replace(context.redirectUrl);

    } else {
        if (context.validation === -1) {
            showValidationError('Error occured creating tenant database');
        }
        else if (context.validation === -2) {
            showValidationError('Invalid ESI Token Guid');

        }
        else {
            handleValidationError(context.validation);
        }

    }
};

var onFailed = function (context) {
    $("#submit").removeClass("hidden");
    results.html('Failed to create the tenant');
};
function validateName(url) {
    $("#pleaseWait").removeClass("hidden");
    $("#submit").addClass("hidden");

    $.ajax({
        url: url,
        data: { name: $("#Name").val(), displayName: $("#DisplayName").val() },
        success: function (data) {
            if (data !== 0) {
                handleValidationError(data);
            } else {
                $('form#mainForm').submit();
            }

        }
    });

};

function handleValidationError(result) {

    if (result === 1) {
        showValidationError('Please enter valid name');
    }
    else if (result === 2) {
        showValidationError('Please enter display name');
    }
    else if (result === 3) {
        showValidationError('Please enter name with length between 4 and 100 characters');
    }
    else if (result === 4) {
        showValidationError('Please enter display name less that 100 characters');
    }
    else if (result === 5) {
        showValidationError('Please enter valid name: must be a valid url');
    }
    else if (result === 6) {
        showValidationError('Sorry but a tennant with this name already exists');
    }
    else if (result === 7) {
        showValidationError('Entity already has a tenant');
    }
}

function showValidationError(message) {
    $("#pleaseWait").addClass("hidden");
    $("#submit").removeClass("hidden");
    $("#errors").html(message);
}