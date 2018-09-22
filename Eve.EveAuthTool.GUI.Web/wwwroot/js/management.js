var onEntitySearchBegin = function () {
    $("#entitySearchWait").removeClass("hidden");
};

var onEntitySearchComplete = function () {
    $("#entitySearchWait").addClass("hidden");

};

var onEntitySearchSuccess = function (context) {
    $('#entitySearchResults').html(context);
    bindSearchResults('entitySearchResult', function (el) {
        $.ajax({
            method: "POST",
            url: "/Management/FormEntityRule/",
            data: {
                entityID: $(el).data('entityid'),
                searchType: $(el).data('entitytype'),
                index: (HighestRuleIndex() + 1)
            },
            success: function (data) {
                $('#relationships').append(data);
                $(el).hide();
            }
        });
    });
};

function AddRole(roleID) {
    $.ajax({
        method: "POST",
        url: "/Management/FormRoleRule/",
        data: {
            roleID: roleID,
            location: document.getElementById('roleLocations').ej2_instances[0].value,
            index: (HighestRuleIndex() + 1)
        },
        success: function (data) {
            $('#relationships').append(data);
        }
    });
}
function AddStanding(entityID,entityType,standing) {
    $.ajax({
        method: "POST",
        url: "/Management/FormStandingRule/",
        data: {
            entityID: entityID,
            entityType: entityType,
            standingType: standing,
            index: HighestRuleIndex() + 1
        },
        success: function (data) {
            $('#relationships').append(data);
        }
    });
}

var onEntitySearchFailed = function (context) {
    $("#entitySearchWait").addClass("hidden");
};

function HighestRuleIndex() {
    var highest = -1;
    $('[data-relationship-index]').each(function (index, element) {
        var current = $(element).data('relationship-index');
        if (current > highest) {
            highest = current;
        }
    });
    return highest;
}

function DeleteAuthRule(element, ruleID) {
    $.ajax({
        method: "POST",
        url: "/Management/DeleteRule/",
        data: {
            ruleID: ruleID
        },
        success: function (data) {
            $('[data-rule-id=' + ruleID+']').remove();
        }
    });
}

function OnTitleCorporationSelect() {
    var listObj = document.getElementById('titleEntities').ej2_instances[0];
    $('#titleWait').removeClass('hidden');
    $.ajax({
        url: "/Management/CorporationTitles/",
        data: {
            corporationID: listObj.value
        },
        success: function (data) {
            $('#titleWait').addClass('hidden');
            $('#titleResults').html(data);
        }
    });
}