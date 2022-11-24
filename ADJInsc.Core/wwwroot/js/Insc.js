$(function () {
    $('#declaracionModal').modal('toggle');
});

$(function () {

    $("#declaracionClic").click(function () {
        $("#btnBuscar").prop("disabled", false);
        $("#declaracionModal").modal('hide');
    });

    $("#btnHideModalDeclaracion").click(function () {
        $("#btnBuscar").prop("disabled", true);
        $("#declaracionModal").modal('hide');
    });

    $("#btnHideModalRecuperar").click(function () {        
        $("#recuperarModal").modal('hide');
    });

    $("#btnLogin").click(function () {
        // $("#AjaxLoaderLogin").show("fast");
        $("#loginModal").modal('show');
    });

    $("#btnShowDeclaracion").click(function () {
        $('#declaracionModal').modal('show');
    });

    $("#btnShowRecuperar").click(function () {
        $('#recuperarModal').modal('show');
    });

    $("#btnHideModal").click(function () {
        $("#loginModal").modal('hide');
        $.ajax({
            url: '@Url.Action("GetLogin", "Inscripcion")',
            data: {
                userName: userName,
                userPassword: userPassword
            },
            type: "post", // Verbo HTTP
            timeout: 1500,

        }).done(function (data) {

        }).fail(function () {

        });
    });
});



$(function () {
    $("#btnBuscar").click(function (e) {

        var numeroDni = $("#dniInput").val();
        $("#AjaxLoader").show("fast");

        if (numeroDni.length > 6 && numeroDni.length < 9) {

            $("#AjaxLoader").delay(2000).hide("slow");


        } else {

            e.preventDefault();
            e.stopPropagation();

            bootbox.alert({
                size: "small",
                title: "I.V.U.J.",
                message: "Debes ingresar un Dni correcto",
                callback: function () {
                    $("#AjaxLoader").hide("slow");
                }
            });

        }
    });
});